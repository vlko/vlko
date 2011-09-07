using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core.Action;
using vlko.core.Authentication;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.BlogModule.Roots;
using vlko.core.Roots;
using vlko.core.Testing;

namespace vlko.BlogModule.Tests.Model
{

	public abstract class UserAuthenticationBaseActions : LocalTest
	{
		private StaticText[] _testData;

		private User _user;

		public UserAuthenticationBaseActions(ITestProvider testProvider) : base(testProvider)
		{
		}

		public virtual void Init()
		{
			TestProvider.SetUp();
		}

		public virtual void Cleanup()
		{
			TestProvider.TearDown();
		}

		public virtual void Create_user()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var authentication = RepositoryFactory.Command<IUserAuthentication>();

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

		public virtual void Validation_token_to_name()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var authentication = RepositoryFactory.Command<IUserAuthentication>();

				string userName = "resolve_token";

				string token;
				// create user
				var result = authentication.CreateUser(userName, "test1", "test@test.sk", out token);
				Assert.AreEqual(CreateUserStatus.Success, result);
				Assert.IsNotNull(token);
				TestProvider.WaitForIndexing();

				// verify if token exists
				var tokenUser = authentication.VerifyTokenToUser(token);
				Assert.AreEqual(userName, tokenUser);
			}
		}

		public virtual void Full_registration_process()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var authentication = RepositoryFactory.Command<IUserAuthentication>();

				string userName = "register_user";

				string token;
				// create user
				var result = authentication.CreateUser(userName, "test1", "test2@test.sk", out token);
				Assert.AreEqual(CreateUserStatus.Success, result);
				Assert.IsNotNull(token);
				TestProvider.WaitForIndexing();

				// confirm registration
				bool registered = authentication.ConfirmRegistration(token);
				Assert.AreEqual(true, registered);
				TestProvider.WaitForIndexing();

				// verify token not exist
				var tokenUser = authentication.VerifyTokenToUser(token);
				Assert.AreEqual(null, tokenUser);

				// test real db data
				var userInDB = TestProvider.AsQueryable<User>().FirstOrDefault(user => user.Name == userName);
				Assert.AreEqual(true, userInDB.Verified);

				// now test confirm registration on already verified user
				userInDB.VerifyToken = token;
				TestProvider.Create(userInDB);
				registered = authentication.ConfirmRegistration(token);
				Assert.AreEqual(false, registered);
			}
		}

		public virtual void Test_user_verify()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var authentication = RepositoryFactory.Command<IUserAuthentication>();

				string userName = "register_user";
				string password = "some_password";

				string token;
				// create user
				var result = authentication.CreateUser(userName, password, "test2@test.sk", out token);
				Assert.AreEqual(CreateUserStatus.Success, result);
				Assert.IsNotNull(token);
				TestProvider.WaitForIndexing();

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

		public virtual void Test_password_reset()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var authentication = RepositoryFactory.Command<IUserAuthentication>();

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
				TestProvider.WaitForIndexing();

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

		public virtual void Deny_two_reset_request_in_one_hour()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var authentication = RepositoryFactory.Command<IUserAuthentication>();

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
				TestProvider.WaitForIndexing();

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

		public virtual void Change_password()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var authentication = RepositoryFactory.Command<IUserAuthentication>();

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
				TestProvider.WaitForIndexing();

				// confirm registration);
				bool registered = authentication.ConfirmRegistration(token);
				Assert.AreEqual(true, registered);
				TestProvider.WaitForIndexing();

				// validation now should pass
				var verifyStatus = authentication.ValidateUser(userName, password);
				Assert.AreEqual(ValidateUserStatus.Success, verifyStatus);

				var newPassword = "new_pswd;";

				// wrong original password should fail
				var changeStatus = authentication.ChangePassword(userName, "wrong_password", newPassword);
				Assert.AreEqual(false, changeStatus);

				// this should go
				changeStatus = authentication.ChangePassword(userName, password, newPassword);
				Assert.AreEqual(true, changeStatus);

				// validation with old password should fail
				verifyStatus = authentication.ValidateUser(userName, password);
				Assert.AreEqual(ValidateUserStatus.InvalidPassword, verifyStatus);

				// and with new password should pass
				verifyStatus = authentication.ValidateUser(userName, newPassword);
				Assert.AreEqual(ValidateUserStatus.Success, verifyStatus);
			}
		}

		public virtual void Is_in_role()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				string username = "test";
				User newUser = new User()
				{
					Id = Guid.NewGuid(),
					Name = username,
					Email = "test@test.sk",
					LastSeen = DateTime.Now,
					Verified = true,
					Roles = "admin,superuser"
				};
				using (var tran = RepositoryFactory.StartTransaction())
				{
					TestProvider.Create(newUser);
					tran.Commit();
				}
				TestProvider.WaitForIndexing();

				var user = IoC.Resolve<IUserCommands>().GetByName(username);
				var authentication = new UserPrincipal(user);

				// valid user and roles
				Assert.IsTrue(authentication.IsInRole("admin"));
				Assert.IsTrue(authentication.IsInRole("superuser"));

				// unknown group should fail
				Assert.IsFalse(authentication.IsInRole("helpdesk"));

			}
		}
	}
}
