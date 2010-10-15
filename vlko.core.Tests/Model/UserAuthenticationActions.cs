using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Linq;
using Castle.ActiveRecord.Testing;
using Castle.Windsor;
using GenericRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core.Models;
using vlko.core.Models.Action;

namespace vlko.core.Tests.Model
{

    [TestClass]
    public class UserAuthenticationActions : InMemoryTest
    {
        private StaticText[] _testData;

        private User _user;

        [TestInitialize]
        public void Init()
        {
            InversionOfControl.IoC.InitializeWith(new WindsorContainer());
            ApplicationInit.InitializeRepositories();
            base.SetUp();
        }

        [TestCleanup]
        public void Cleanup()
        {
            TearDown();
        }

        public override Type[] GetTypes()
        {
            return ApplicationInit.ListOfModelTypes();
        }

        [TestMethod]
        public void Create_user()
        {
            using (RepositoryFactory.StartUnitOfWork())
            {
                var authentication = RepositoryFactory.GetRepository<User>().GetAction<IUserAuthentication>();

                // test success
                string token;
                var result = authentication.CreateUser("create_test", "test1", "test@test.sk", out token);
                Assert.AreEqual(CreateUserStatus.Success, result);
                Assert.IsNotNull(token);


                // test duplicate user
                result = authentication.CreateUser("create_test", "test1", "test@test.sk", out token);
                Assert.AreEqual(CreateUserStatus.DuplicateUserName, result);
                Assert.IsNull(token);


                // test duplicate email
                result = authentication.CreateUser("create_test2", "test1", "test@test.sk", out token);
                Assert.AreEqual(CreateUserStatus.DuplicateEmail, result);
                Assert.IsNull(token);
            }
        }

        [TestMethod]
        public void Validation_token_to_name()
        {
            using (RepositoryFactory.StartUnitOfWork())
            {
                var authentication = RepositoryFactory.GetRepository<User>().GetAction<IUserAuthentication>();

                string userName = "resolve_token";

                string token;
                // create user
                var result = authentication.CreateUser(userName, "test1", "test@test.sk", out token);
                Assert.AreEqual(CreateUserStatus.Success, result);
                Assert.IsNotNull(token);

                // verify if token exists
                var tokenUser = authentication.VerifyTokenToUser(token);
                Assert.AreEqual(userName, tokenUser);
            }
        }

        [TestMethod]
        public void Full_registration_process()
        {
            using (RepositoryFactory.StartUnitOfWork())
            {
                var authentication = RepositoryFactory.GetRepository<User>().GetAction<IUserAuthentication>();

                string userName = "register_user";

                string token;
                // create user
                var result = authentication.CreateUser(userName, "test1", "test2@test.sk", out token);
                Assert.AreEqual(CreateUserStatus.Success, result);
                Assert.IsNotNull(token);

                // confirm registration
                bool registered = authentication.ConfirmRegistration(token);
                Assert.AreEqual(true, registered);

                // verify token not exist
                var tokenUser = authentication.VerifyTokenToUser(token);
                Assert.AreEqual(null, tokenUser);

                // test real db data
				var userInDB = ActiveRecordLinqBase<User>.Queryable.FirstOrDefault(user => user.Name == userName);
                Assert.AreEqual(true, userInDB.Verified);

                // now test confirm registration on already verified user
                userInDB.VerifyToken = token;
                ActiveRecordMediator<User>.Update(userInDB);
                registered = authentication.ConfirmRegistration(token);
                Assert.AreEqual(false, registered);
            }
        }

        [TestMethod]
        public void Test_user_verify()
        {
            using (RepositoryFactory.StartUnitOfWork())
            {
                var authentication = RepositoryFactory.GetRepository<User>().GetAction<IUserAuthentication>();

                string userName = "register_user";
                string password = "some_password";

                string token;
                // create user
                var result = authentication.CreateUser(userName, password, "test2@test.sk", out token);
                Assert.AreEqual(CreateUserStatus.Success, result);
                Assert.IsNotNull(token);

                // validation now should fail, because we didn't finish registration process
                var verifyStatus = authentication.ValidateUser(userName, password);
                Assert.AreEqual(ValidateUserStatus.NotVerified, verifyStatus);

                // confirm registration
                bool registered = authentication.ConfirmRegistration(token);
                Assert.AreEqual(true, registered);

                // validation now should pass
                verifyStatus = authentication.ValidateUser(userName, password);
                Assert.AreEqual(ValidateUserStatus.Success, verifyStatus);

                // test not existing user 
                verifyStatus = authentication.ValidateUser(userName + "not_existing", password);
                Assert.AreEqual(ValidateUserStatus.NotExists, verifyStatus);

                // test wrong password
                verifyStatus = authentication.ValidateUser(userName, password + "not_valid");
                Assert.AreEqual(ValidateUserStatus.InvalidPassword, verifyStatus);
            }
        }

        [TestMethod]
        public void Test_password_reset()
        {
            using (RepositoryFactory.StartUnitOfWork())
            {
                var authentication = RepositoryFactory.GetRepository<User>().GetAction<IUserAuthentication>();

                string userName = "reset_user";
                string password = "some_password";
                string email = "reset@test.sk";


                //  reset password should failed on not existed user
                var resetResult = authentication.ResetPassword(userName, string.Empty, password);
                Assert.AreEqual(false, resetResult);

                string token;
                // create user
                var result = authentication.CreateUser(userName, password, email, out token);
                Assert.AreEqual(CreateUserStatus.Success, result);
                Assert.IsNotNull(token);

                string resettoken;

                // reset password now should fail, because we didn't finish registration process
                // reset password now should fail, because we didn't finish registration process
                var resetStatus = authentication.GetResetPasswordToken(email, out resettoken);
                Assert.AreEqual(ResetUserPasswordStatus.UserNotVerified, resetStatus);
                Assert.AreEqual(null, resettoken);

                // also reset password should failed
                resetResult = authentication.ResetPassword(userName, token, password);
                Assert.AreEqual(false, resetResult);

                // confirm registration);
                bool registered = authentication.ConfirmRegistration(token);
                Assert.AreEqual(true, registered);

                // validation now should pass
                var verifyStatus = authentication.ValidateUser(userName, password);
                Assert.AreEqual(ValidateUserStatus.Success, verifyStatus);

                // get reset password token
                resetStatus = authentication.GetResetPasswordToken(email, out token);
                Assert.AreEqual(ResetUserPasswordStatus.Success, resetStatus);

                // finally reset password
                password = "new_password";
                resetResult = authentication.ResetPassword(userName, token, password);
                Assert.AreEqual(true, resetResult);

                // validation now should pass
                verifyStatus = authentication.ValidateUser(userName, password);
                Assert.AreEqual(ValidateUserStatus.Success, verifyStatus);

                // password already was reset and now should failed
                resetResult = authentication.ResetPassword(userName, resettoken, password);
                Assert.AreEqual(false, resetResult);
            }
        }

        [TestMethod]
        public void Deny_two_reset_request_in_one_hour()
        {
            using (RepositoryFactory.StartUnitOfWork())
            {
                var authentication = RepositoryFactory.GetRepository<User>().GetAction<IUserAuthentication>();

                string userName = "reset_user";
                string password = "some_password";
                string email = "reset@test.sk";


                //  reset password should failed on not existed user
                var resetResult = authentication.ResetPassword(userName, string.Empty, password);
                Assert.AreEqual(false, resetResult);

                string token;
                // create user
                var result = authentication.CreateUser(userName, password, email, out token);
                Assert.AreEqual(CreateUserStatus.Success, result);
                Assert.IsNotNull(token);

                // confirm registration);
                bool registered = authentication.ConfirmRegistration(token);
                Assert.AreEqual(true, registered);

                // validation now should pass
                var verifyStatus = authentication.ValidateUser(userName, password);
                Assert.AreEqual(ValidateUserStatus.Success, verifyStatus);

                // first reset should go
                var resetStatus = authentication.GetResetPasswordToken(email, out token);
                Assert.AreEqual(ResetUserPasswordStatus.Success, resetStatus);
                Assert.IsNotNull(token);

                // second reset should fail
                resetStatus = authentication.GetResetPasswordToken(email, out token);
                Assert.AreEqual(ResetUserPasswordStatus.WaitOneHour, resetStatus);
                Assert.IsNull(token);
            }
        }

        [TestMethod]
        public void Change_password()
        {
            using (RepositoryFactory.StartUnitOfWork())
            {
                var authentication = RepositoryFactory.GetRepository<User>().GetAction<IUserAuthentication>();

                string userName = "reset_user";
                string password = "some_password";
                string email = "reset@test.sk";


                //  reset password should failed on not existed user
                var resetResult = authentication.ResetPassword(userName, string.Empty, password);
                Assert.AreEqual(false, resetResult);

                string token;
                // create user
                var result = authentication.CreateUser(userName, password, email, out token);
                Assert.AreEqual(CreateUserStatus.Success, result);
                Assert.IsNotNull(token);

                // confirm registration);
                bool registered = authentication.ConfirmRegistration(token);
                Assert.AreEqual(true, registered);

                // validation now should pass
                var verifyStatus = authentication.ValidateUser(userName, password);
                Assert.AreEqual(ValidateUserStatus.Success, verifyStatus);

                var newPassword = "new_pswd;";

                // wrong original password should fail
                var changeStatus = authentication.ChangePassword(userName, "wrong_password", newPassword);
                Assert.AreEqual(false, changeStatus);

                // this should go
                changeStatus = authentication.ChangePassword(userName, password, newPassword);
                Assert.AreEqual(false, changeStatus);

                // validation with old password should fail
                verifyStatus = authentication.ValidateUser(userName, password);
                Assert.AreEqual(ValidateUserStatus.InvalidPassword, verifyStatus);

                // and with new password should pass
                verifyStatus = authentication.ValidateUser(userName, newPassword);
                Assert.AreEqual(ValidateUserStatus.Success, verifyStatus);
            }
        }

        [TestMethod]
        public void Is_in_role()
        {
            using (RepositoryFactory.StartUnitOfWork())
            {
                string username = "test";
                User newUser = new User()
                {
                    Name = "test",
                    Email = "test@test.sk",
                    LastSeen = DateTime.Now,
                    Verified = true,
                    Roles = "admin,superuser"
                };
                using (var tran = RepositoryFactory.StartTransaction())
                {
                    ActiveRecordMediator<User>.Create(newUser);
                    tran.Commit();
                }

                var authentication = RepositoryFactory.GetRepository<User>().GetAction<IUserAuthentication>();

                // valid user and roles
                Assert.IsTrue(authentication.IsUserInRole(username, "admin"));
                Assert.IsTrue(authentication.IsUserInRole(username, "superuser"));

                // unknown group should fail
                Assert.IsFalse(authentication.IsUserInRole(username, "helpdesk"));

                // unknown user should fail
                Assert.IsFalse(authentication.IsUserInRole("not_valid_user", "admin"));

            }
        }
    }
}
