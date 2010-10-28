<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.model.Action.ViewModel.FileViewModel>" %>

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
			<%= Html.DisplayFor(model => model.Ident)%>
			<%= Html.DisplayFor(model => model.Url)%>
			<%= Html.DisplayFor(model => model.Size)%>
		</fieldset>
	</div>
	<% using (Html.BeginForm()) { %>
		<div  class="ajax_ignore">
			<%: Html.HiddenFor(model => Model.Ident)%>	
			<input type="submit" value="Delete" /> |
			<%: Html.ActionLink("Back to List", "Index") %>
		</div>
	<% } %>

</asp:Content>

