using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GenericRepository;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.InversionOfControl;
using vlko.core.Models.Action;
using vlko.core.Search;
using vlko.core.ValidationAtribute;
using vlko.web.ViewModel.Search;

namespace vlko.web.Controllers
{
	public class SearchController : BaseController
	{
		[HttpGet]
		[AntiXss]
		public ActionResult Index(string query, PagedModel<object> pageModel)
		{
			if (!string.IsNullOrEmpty(query))
			{
				using (var session = RepositoryFactory.StartUnitOfWork(IoC.Resolve<SearchContext>()))
				{
					// test search for user name
					var searchResult = IoC.Resolve<ISearchAction>().Search(session, query);
					pageModel.LoadData(searchResult);
				}
			}
			else
			{
				pageModel = null;
			}

			return ViewWithAjax(new SearchViewModel()
			                    	{
										Query = query,
										SearchResults = pageModel
			                    		
			                    	});
		}

		[HttpGet]
		[AntiXss]
		public ActionResult Date(string query, PagedModel<object> pageModel)
		{
			if (!string.IsNullOrEmpty(query))
			{
				using (var session = RepositoryFactory.StartUnitOfWork(IoC.Resolve<SearchContext>()))
				{
					// test search for user name
					var searchResult = IoC.Resolve<ISearchAction>().SearchByDate(session, query);
					pageModel.LoadData(searchResult);
				}
			}
			else
			{
				pageModel = null;
			}

			return ViewWithAjax("Index", new SearchViewModel()
			{
				Query = query,
				SearchResults = pageModel

			});
		}
	}
}