<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.core.Components.PagedModel<vlko.core.Models.Action.ViewModel.CommentForAdminViewModel>>" %>
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
			<th rowspan="2"></th>
			<th><%= vlko.core.ModelResources.Name %></th>
			<th><%= vlko.core.ModelResources.CreatedDate %></th>
			<th><%= vlko.core.ModelResources.Changed %></th>
			<th><%= vlko.core.ModelResources.UserName %></th>
			<th><%= vlko.core.ModelResources.ClientIp %></th>
		</tr>
		<tr>
			<th colspan="5"><%= vlko.core.ModelResources.Text %></th>
		</tr>
	<% foreach (var item in Model) { %>
		<tr>
			<td rowspan="2">
				<%: Html.ActionLink<CommentController>(c => c.Edit(item.Id), "Edit", new { @class = "grid_edit", title = "Edit" })%>&nbsp;
				<%: Html.ActionLink<CommentController>(c => c.Details(item.Id), "Details", new { @class = "grid_details", title = "Details" })%>&nbsp;
				<%: Html.ActionLink<CommentController>(c => c.Delete(item.Id), "Delete", new { @class = "grid_delete", title = "Delete" })%>             </td>
			<td><%: item.Name %></td>
			<td><%: String.Format("{0:g}", item.CreatedDate) %></td>
			<td><%: item.Changed ? "ver. " + item.Version : item.Changed.ToString() %></td>
			<td><%: (item.Owner == null) ? "anonymous-" + item.AnonymousName : item.Owner.Name %></td>
			<td><%: item.ClientIp %></td>
		</tr>
		<tr>
			<td colspan="5"><%= item.Text %></td>
		</tr>
	<% } %>

	</table>
	<p><% Html.RenderPartial("~/Views/Shared/Pager.ascx", new PagerModel(Model, "Index")); %></p>
	<% Html.ScriptInlineInclude(() => {%>
	<script type="text/javascript">
		$(function () {
			$("#content").ajaxGrid();
		});
	</script> 
	<% }); %>	
</asp:Content>

