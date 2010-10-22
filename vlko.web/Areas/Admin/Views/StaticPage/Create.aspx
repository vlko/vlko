<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.model.Action.CRUDModel.StaticTextCRUDModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% 
		Html.ScriptInclude("~/Scripts/jquery.validate.min.js"); 
		Html.ScriptInclude("~/Scripts/MicrosoftMvcJQueryValidation.js"); 
		Html.EnableClientValidation(); 
	%>
	
	<h2>Create</h2>
	<% using (Html.BeginForm(cssClass:"ajax_container")) {%>
	<div class="ajax_content">
		<%: Html.ValidationSummary(cssClass: "ui-state-error ui-corner-all") %>

		<fieldset>
			<legend>Fields</legend>
			
			<%: Html.EditorFor(model => model.FriendlyUrl) %>
			<%: Html.EditorFor(model => model.Title) %>
			<%: Html.EditorFor(model => model.Text) %>
			<%: Html.EditorFor(model => model.PublishDate) %>
			<%: Html.EditorFor(model => model.AllowComments) %>

		</fieldset>
	</div>
	<div>
		<input type="submit" value="Create" />
	</div>
	<% } %>

	<div>
		<%: Html.ActionLink("Back to List", "Index") %>
	</div>

</asp:Content>

