<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.model.Action.CRUDModel.RssItemCRUDModel[]>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Feed test results
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<h2>Test results:</h2>

	<%: Html.ValidationSummary(cssClass: "ui-state-error ui-corner-all") %>
	<% if (Model != null) {%>
	<table>
		<tr>
			<th>Feed id</th>
			<th>Publish date</th>
			<th>Author</th>
			<th>Title</th>
			<th>Url</th>
		</tr>
		<tr>
			<th colspan="5">Text</th>
		</tr>
	<% foreach (var item in Model) { %>
		<tr>
			<td><%: item.FeedItemId %></td>
			<td><%: String.Format("{0:g}", item.Published) %></td>
			<td><%: item.Author %></td>
			<td><%: item.Title %></td>
			<td><%: item.Url %></td>
		</tr>
		<tr>
			<td colspan="5"><%= item.Text %></td>
		</tr>
	<% } %>

	</table>
	<% } %>
</asp:Content>
