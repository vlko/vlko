using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Castle.ActiveRecord.Testing;
using Castle.Windsor;
using GenericRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core;
using vlko.core.Components;
using vlko.core.IoC;
using vlko.core.Models.Action;
using vlko.core.Models.Action.ActionModel;
using vlko.core.Models.Action.ViewModel;
using vlko.web.Areas.Admin.Controllers;
using vlko.web.Controllers;

namespace vlko.web.Tests.Controllers
{
	[TestClass]
	public class PageControllerTest : InMemoryTest
	{
		public static int NumberOfGeneratedItems = 100;
		private IUnitOfWork session;
		[TestInitialize]
		public void Init()
		{
			IoC.InitializeWith(new WindsorContainer());
			ApplicationInit.InitializeRepositories();
			base.SetUp();
			using (var tran = RepositoryFactory.StartTransaction())
			{

				IoC.Resolve<IUserAction>().CreateAdmin("vlko", "vlko@zilina.net", "test");
				var admin = IoC.Resolve<IUserAction>().GetByName("vlko");
				for (int i = 0; i < NumberOfGeneratedItems; i++)
				{
					IoC.Resolve<IStaticTextCrud>().Create(
						new StaticTextActionModel
							{
								AllowComments = false,
								Creator = admin,
								Title = "StaticPage" + i,
								FriendlyUrl = "staticpage" + (i == NumberOfGeneratedItems - 1 ? string.Empty : i.ToString()),
								ChangeDate = DateTime.Now,
								PublishDate = DateTime.Now.AddDays(-i),
								Text = "Static page" + i
							});
				}
				tran.Commit();
			}
			session = RepositoryFactory.StartUnitOfWork();
		}

		[TestCleanup]
		public void Cleanup()
		{
			session.Dispose();
			TearDown();
		}

		public override Type[] GetTypes()
		{
			return ApplicationInit.ListOfModelTypes();
		}

		[TestMethod]
		public void Index()
		{
			// Arrange
			PageController controller = new PageController();
			controller.MockRequest();
			// Act
			ViewResult result = controller.Index(new PagedModel<StaticTextViewModel>()) as ViewResult;

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));

			ViewResult viewResult = (ViewResult)result;
			var model = (PagedModel<StaticTextViewModel>)viewResult.ViewData.Model;

			Assert.AreEqual(NumberOfGeneratedItems, model.Count);

			int i = 0;
			foreach (var staticText in model)
			{
				Assert.AreEqual("StaticPage" + i, staticText.Title);
				Assert.AreEqual("staticpage" + i, staticText.FriendlyUrl);
				++i;
			}
			Assert.AreEqual(PagedModel<StaticTextViewModel>.DefaultPageItems, i);
		}

		[TestMethod]
		public void ViewPage()
		{
			// Arrange
			PageController controller = new PageController();
			controller.MockRequest();
			// Act
			ViewResult result = controller.ViewPage("staticpage2", new PagedModel<CommentViewModel>()) as ViewResult;

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));

			ViewResult viewResult = (ViewResult)result;
			var model = (StaticTextWithFullTextViewModel)viewResult.ViewData.Model;

			Assert.AreEqual("StaticPage2", model.Title);
			Assert.AreEqual("staticpage2", model.FriendlyUrl);
			Assert.AreEqual("Static page2", model.Text);
		}
	}
}
