<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.model.Action.CRUDModel.CommentCRUDModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Delete
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<h2>Delete</h2>

	<div>
		<%: Html.ValidationSummary(excludePropertyErrors: true, cssClass: "ui-state-error ui-corner-all")%>

		<h3>Are you sure you want to delete this?</h3>
		<fieldset>
			<legend>Fields</legend>
		
			<%: Html.DisplayFor(model => model.Id) %>
			<%: Html.DisplayFor(model => model.ContentId) %>
			<%: Html.DisplayFor(model => model.Name) %>
			<%: Html.DisplayFor(model => model.Text) %>
			<%: Html.DisplayFor(model => model.ChangeDate) %>
			<%: Html.DisplayFor(model => model.ParentId) %>
			<%: Html.DisplayFor(model => model.AnonymousName) %>
			<%: Html.DisplayFor(model => model.ClientIp) %>
			<%: Html.DisplayFor(model => model.UserAgent) %>

	</fieldset>
	</div>
	<% using (Html.BeginForm()) { %>
		<div class="ajax_ignore">
			<%: Html.HiddenFor(model => Model.Id)%> |
		
			<input type="submit" value="Delete" /> |
			<%: Html.ActionLink("Back to List", "Index") %>
		</div>
	<% } %>

</asp:Content>

