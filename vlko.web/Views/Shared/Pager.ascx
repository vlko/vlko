<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<vlko.core.Components.PagerModel>" %>
<div class="pager">
<%
	Html.ScriptInclude("~/Scripts/Pager.js");
	const int pagerRange = 4;
	int pagesNumber = Model.PagesNumber;
	int currentPage = Model.CurrentPage;
	int lowestPage = currentPage - pagerRange;
	int highestPage = currentPage + pagerRange;
	bool lowestRangeDelimiterDisplayed = false;
	bool highesRangeDelimiterDisplayed = false;

	if (currentPage > 1)
	{
%><%= Html.ActionLink("<<", Model.Action, routeValues: new { Page = currentPage - 1}, cssClass:"pager_prev" ) %><%
	}
	else
	{
%><span class="pager_prev">&lt;&lt;</span>&nbsp;<%		
	}
%>&nbsp;<%
	if (currentPage < Model.PagesNumber)
	{
%><%= Html.ActionLink(">>", Model.Action, routeValues: new { Page = currentPage + 1}, cssClass:"pager_next" ) %><%
	}
	else
	{
%><span class="pager_next">&gt;&gt;</span>&nbsp;<%		
	}                                                                                              	
%>&nbsp;
<%= Html.ActionLink("1", Model.Action, routeValues: new { Page = 1} ) %>&nbsp;
<%
	for (int i = 2; i < pagesNumber; i++)
 {
	 if (i < lowestPage)
	 {
		 if (!lowestRangeDelimiterDisplayed)
		 {
%>...&nbsp;<%
			 lowestRangeDelimiterDisplayed = true;
		 }
	 }
	 if (i >= lowestPage && i <= highestPage)
	 {
%><%=Html.ActionLink(i.ToString(), Model.Action, Model.Controller, routeValues: new {Page = i})%>&nbsp;<%
	 }
	 if (i > highestPage)
	 {
		 if (!highesRangeDelimiterDisplayed)
		 {
%>...&nbsp;<%
			 highesRangeDelimiterDisplayed = true;
		 }
	 }
 }
 if (pagesNumber > 1)
 {
%>
<%=Html.ActionLink(pagesNumber.ToString(), Model.Action, routeValues: new {Page = pagesNumber})%>
<%
 }%>
<span class="pager_info"><%= string.Format("{0} - {1} of {2}", Model.StartItemNumber, Model.EndItemNumber, Model.TotalCount) %></span>
</div>

<%
 var contentId = ViewData["content"] as string;
 if (string.IsNullOrEmpty(contentId))
 {
	contentId = "content";
 }
%>

<% Html.ScriptInlineInclude(() => {%>
<script type="text/javascript">
	$(function () {
		$("#<%= contentId %>").ajaxPager();
	});
</script> 
<% }); %>