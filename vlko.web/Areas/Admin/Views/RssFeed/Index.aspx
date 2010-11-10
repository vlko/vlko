<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.core.Components.PagedModel<vlko.model.Action.ViewModel.RssFeedViewModel>>" %>
<%@ Import Namespace="vlko.web.Areas.Admin.Controllers" %>
<%@ Import Namespace="vlko.web.Controllers" %>
<%@ Import Namespace="vlko.core.Components" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<h2>Index</h2>

	<% Html.ScriptInclude("~/Scripts/Grid.js"); %>
	<table>
		<tr>
			<th></th>
			<th><%= vlko.model.ModelResources.Name %></th>
			<th><%= vlko.model.ModelResources.FeedUrl %></th>
			<th>Show full content</th>
			<th>Number of feed items</th>
		</tr>
	<% foreach (var item in Model) { %>
		<tr>
			<td>
				<%: Html.ActionLink<RssFeedController>(c => c.Edit(item.Id), "Edit", new { @class = "grid_edit", title = "Edit" })%>&nbsp;
				<%: Html.ActionLink<RssFeedController>(c => c.Details(item.Id), "Details", new { @class = "grid_details", title = "Details" })%>&nbsp;
				<%: Html.ActionLink<RssFeedController>(c => c.Delete(item.Id), "Delete", new { @class = "grid_delete", title = "Delete" })%>
			</td>
			<td><%: item.Name %></td>
			<td><%: item.FeedUrl %></td>
			<td><%: item.DisplayFullContent %></td>
			<td><%: item.FeedItemCount %></td>
		</tr>
	<% } %>

	</table>
	<div><% Html.RenderPartial("~/Views/Shared/Pager.ascx", new PagerModel(Model, "Index")); %></div>
	<div>
		<%: Html.ActionLink<RssFeedController>(c => c.Create(), "Create new", new { @class = "grid_create", title = "Create new" })%>
	</div>
	<% Html.ScriptInlineInclude(() => {%>
	<script type="text/javascript">
		$(function () {
			$("#content").ajaxGrid();
		});
	</script> 
	<% }); %>	
</asp:Content>

