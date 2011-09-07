using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.Tests.Model;
using vlko.core.Authentication;
using vlko.core.InversionOfControl;
using vlko.core.RavenDB.Repository;
using vlko.core.RavenDB.Testing;
using vlko.core.Repository;
using vlko.BlogModule.Roots;
using vlko.core.Roots;

namespace vlko.BlogModule.RavenDB.Tests.Model
{

	[TestClass]
	public class UserAuthenticationActions : UserAuthenticationBaseActions
	{
		public UserAuthenticationActions()
			: base(new RavenDBTestProvider())
		{
		}

		[TestInitialize]
		public override void Init()
		{
			base.Init();
		}

		[TestCleanup]
		public override void Cleanup()
		{
			base.Cleanup();
		}

		[TestMethod]
		public override void Create_user()
		{
			base.Create_user();
		}

		[TestMethod]
		public override void Validation_token_to_name()
		{
			base.Validation_token_to_name();
		}

		[TestMethod]
		public override void Full_registration_process()
		{
			base.Full_registration_process();
		}

		[TestMethod]
		public override void Test_user_verify()
		{
			base.Test_user_verify();
		}

		[TestMethod]
		public override void Test_password_reset()
		{
			base.Test_password_reset();
		}

		[TestMethod]
		public override void Deny_two_reset_request_in_one_hour()
		{
			base.Deny_two_reset_request_in_one_hour();
		}

		[TestMethod]
		public override void Change_password()
		{
			base.Change_password();
		}

		[TestMethod]
		public override void Is_in_role()
		{
			base.Is_in_role();
		}
	}
}
