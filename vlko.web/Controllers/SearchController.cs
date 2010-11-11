using System.Web.Mvc;
using Microsoft.Security.Application;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.core.ValidationAtribute;
using vlko.model.Action;
using vlko.model.Search;
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
				var searchQuery = HttpContext.Request.QueryString["query"];
				using (var session = RepositoryFactory.StartUnitOfWork(IoC.Resolve<SearchContext>()))
				{
					// test search for user name
					var searchResult = RepositoryFactory.Action<ISearchAction>().Search(session, searchQuery);
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
					var searchResult = RepositoryFactory.Action<ISearchAction>().SearchByDate(session, query);
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