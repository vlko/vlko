<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.model.Action.CRUDModel.StaticTextCRUDModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% 
		Html.ScriptInclude("~/Scripts/jquery.validate.min.js"); 
		Html.ScriptInclude("~/Scripts/MicrosoftMvcJQueryValidation.js"); 
		Html.EnableClientValidation(); 
	%>

	<h2>Edit</h2>
	<% using (Html.BeginForm()) {%>
	<div>
		<%: Html.ValidationSummary(cssClass: "ui-state-error ui-corner-all")%>
		
		<fieldset>
			<legend>Fields</legend>

			<%: Html.HiddenFor(model => model.Id) %>			
			<%: Html.EditorFor(model => model.FriendlyUrl) %>
			<%: Html.EditorFor(model => model.Title) %>
			<%: Html.EditorFor(model => model.Text) %>
			<%: Html.EditorFor(model => model.PublishDate) %>
			<%: Html.EditorFor(model => model.AllowComments) %>

		</fieldset>
	</div>
	<div class="ajax_ignore">
		<input type="submit" value="Save" />
	</div>
	<% } %>
	<div class="ajax_ignore">
		<%: Html.ActionLink("Back to List", "Index") %>
	</div>
</asp:Content>

