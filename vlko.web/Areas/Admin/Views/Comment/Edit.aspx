<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.model.Action.CRUDModel.CommentCRUDModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<h2>Edit</h2>

	<% 
		Html.ScriptInclude("~/Scripts/jquery.validate.min.js"); 
		Html.ScriptInclude("~/Scripts/MicrosoftMvcJQueryValidation.js"); 
		Html.EnableClientValidation(); 
	%>
	
	<% using (Html.BeginForm(cssClass:"ajax_container")) {%>
	<div class="ajax_content">
		<%: Html.ValidationSummary(cssClass: "ui-state-error ui-corner-all") %>
		
		<fieldset>
			<legend>Fields</legend>
			
			<%: Html.HiddenFor(model => model.Id) %>	     
			<%: Html.DisplayFor(model => model.ContentId) %>	     
			<%: Html.EditorFor(model => model.Name) %>	     
			<%: Html.EditorFor(model => model.Text)%>	
			<%: Html.DisplayFor(model => model.AnonymousName)%>				     
			<%: Html.DisplayFor(model => model.ChangeDate)%>	     
			<%: Html.DisplayFor(model => model.ParentId)%>	     
			<%: Html.DisplayFor(model => model.ChangeUser.Name)%>     
			<%: Html.DisplayFor(model => model.ClientIp)%>	     
			<%: Html.DisplayFor(model => model.UserAgent)%>	     

		</fieldset>
	</div>
	<div>
		<input type="submit" value="Save" />
	</div>
	<% } %>

	<div>
		<%: Html.ActionLink("Back to List", "Index") %>
	</div>

</asp:Content>

