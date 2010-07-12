using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace vlko.core.Components
{
	public class PagerModel
	{
		public const string PageRouteConst = "Page";
		private readonly RouteValueDictionary _routeValues;
		/// <summary>
		/// Initializes a new instance of the <see cref="PagerModel"/> class.
		/// </summary>
		/// <param name="pageModel">The page model.</param>
		/// <param name="action">The action.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="routeValues">The route values.</param>
		public PagerModel(IPagedModel pageModel, string action, string controller = null, object routeValues = null)
		{
			Controller = controller;
			_routeValues = new RouteValueDictionary(routeValues);
			_routeValues.Add(PageRouteConst, 0);
			Action = action;
			CurrentPage = pageModel.CurrentPage;
			PagesNumber = pageModel.PagesNumber;
			TotalCount = pageModel.Count;

			StartItemNumber = (pageModel.CurrentPage - 1)*pageModel.PageItems + 1;
			if (StartItemNumber < 0)
			{
				StartItemNumber = 0;
			}

			EndItemNumber = StartItemNumber + pageModel.PageItems - 1;
			if (EndItemNumber > TotalCount)
			{
				EndItemNumber = TotalCount;
			}

			if (StartItemNumber > EndItemNumber)
			{
				StartItemNumber = EndItemNumber;
			}
		}

		/// <summary>
		/// Gets or sets the current page.
		/// </summary>
		/// <value>The current page.</value>
		public int CurrentPage { get; private set; }

		/// <summary>
		/// Gets the pages number.
		/// </summary>
		/// <value>The pages number.</value>
		public int PagesNumber { get; private set; }

		/// <summary>
		/// Gets or sets the controller.
		/// </summary>
		/// <value>The controller.</value>
		public string Controller { get; private set; }

		/// <summary>
		/// Gets or sets the action.
		/// </summary>
		/// <value>The action.</value>
		public string Action { get; private set; }

		/// <summary>
		/// Gets or sets the start item number.
		/// </summary>
		/// <value>The start item number.</value>
		public int StartItemNumber { get; private set; }

		/// <summary>
		/// Gets or sets the end item number.
		/// </summary>
		/// <value>The end item number.</value>
		public int EndItemNumber { get; private set; }

		/// <summary>
		/// Gets or sets the total items.
		/// </summary>
		/// <value>The total items.</value>
		public int TotalCount { get; private set; }

		/// <summary>
		/// Gets the page routes.
		/// </summary>
		/// <param name="pageNumber">The page number.</param>
		/// <returns>Route values for specified page number.</returns>
		public RouteValueDictionary GetPageRoutes(int pageNumber)
		{
			_routeValues[PageRouteConst] = pageNumber;
			return _routeValues;
		}
	}
}