using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MvcContrib.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.web;
using vlko.web.Controllers;

namespace vlko.web.Tests.Controllers
{
	[TestClass]
	public class HomeControllerTest
	{

		[TestMethod]
		public void About()
		{
			// Arrange
			HomeController controller = new HomeController();

			// Act
			ActionResult result = controller.About();

			// Assert
			result.AssertViewRendered();
		}
	}
}
