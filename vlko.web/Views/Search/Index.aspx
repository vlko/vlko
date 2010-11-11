<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.web.ViewModel.Search.SearchViewModel>" %>
<%@ Import Namespace="vlko.model" %>
<%@ Import Namespace="vlko.model.Action.ViewModel" %>
<%@ Import Namespace="vlko.core.Components" %>
<%@ Import Namespace="vlko.model.Roots" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Search
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<h2>Search</h2>
	<section id="search_from">
		<% using (Html.BeginForm(ViewContext.RouteData.GetRequiredString("action"), "Search", method: FormMethod.Get)) {%>
			<%: Html.Hidden("Page", Html.ViewContext.RequestContext.HttpContext.Request.QueryString["Page"]) %>
			<%: Html.TextBoxFor(model => model.Query) %>
			<input type="submit" value="Search" />
	<% } %>
	</section>
	<% if (Model.SearchResults != null) { %>
	<section id="search_results">
	<%= Html.ActionLink("sort by relevance", "Index", routeValues: new { Model.Query })%>
	<%= Html.ActionLink("sort by date", "Date", routeValues: new { Model.Query })%>
	<%  foreach (var searchResult in Model.SearchResults)
		{
			if (searchResult is CommentSearchViewModel 
				&& ((CommentSearchViewModel)searchResult).Content is StaticText)
			{
				Html.RenderPartial("~/Views/Search/Comment.ascx", searchResult as CommentViewModel);
			}
			if (searchResult is StaticTextViewModel)
			{
				Html.RenderPartial("~/Views/Search/StaticText.ascx", searchResult as StaticTextViewModel);
			}
			if (searchResult is TwitterStatus)
			{
				Html.RenderPartial("~/Views/Search/TwitterStatus.ascx", searchResult as TwitterStatus);
			}
			if (searchResult is RssItemViewModel)
			{
				Html.RenderPartial("~/Views/Search/RssItem.ascx", searchResult as RssItemViewModel);
			}
		}%>

	<div><% Html.RenderPartial("~/Views/Shared/Pager.ascx", new PagerModel(Model.SearchResults, ViewContext.RouteData.GetRequiredString("action"), routeValues: new { Model.Query })); %></div>
	</article>
	<% } %>
</asp:Content>

