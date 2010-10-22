<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<vlko.model.Action.CRUDModel.CommentCRUDModel>" %>
<%@ Import Namespace="vlko.web.Controllers" %>
<%@ Import Namespace="vlko.web.ViewModel.Page" %>

	<% 
		Html.ScriptInclude("~/Scripts/jquery.validate.js"); 
		Html.ScriptInclude("~/Scripts/MicrosoftMvcJQueryValidation.js"); 
		Html.EnableClientValidation(); 
	%>
	
	<% using (Html.BeginRouteForm("PageComment",
			new RouteValueDictionary( new
			                          	{
			                          		friendlyUrl = Html.ViewContext.RouteData.GetRequiredString("friendlyUrl"),
											sort = Html.ViewContext.RouteData.GetRequiredString("sort")
			                          	} )
			))
	{%>
		<fieldset>
			<legend>Add comment</legend>

			<%: Html.Hidden("Page", Html.ViewContext.RequestContext.HttpContext.Request.QueryString["Page"]) %>
			<%: Html.HiddenFor(model => model.ContentId) %>
			<%: Html.HiddenFor(model => model.ParentId) %>
			<div class="new_comment_user">
			<% if (Model.ChangeUser == null) { %>     
				<span>Anonymous name:</span>
				<%= Html.TextBoxFor(model => model.AnonymousName, cssClass: "text-box single-line")%>
				<%: Html.ValidationMessageFor(model => model.AnonymousName)%>
			<% } %>    	     
			<% else { %>  
				<%: Model.ChangeUser.Name %>
			<% } %>
			</div> 
			<div class="new_comment_title">Title: <%= Html.TextBoxFor(model => model.Name, cssClass: "text-box single-line")%><%: Html.ValidationMessageFor(model => model.Name)%></div>
			<div class="new_comment_text">
				<%= Html.TextAreaFor(model => model.Text, cssClass: "text-box multi-line")%>
				<% if (Model.ChangeUser != null) { %>     
					<% 
						Html.ScriptInclude("~/Scripts/ckeditor/ck_mvc_integration.js");
						Html.ScriptInclude("~/Scripts/ckeditor/ckeditor.js");
						Html.ScriptInclude("~/Scripts/ckeditor/adapters/jquery.js");
						Html.ScriptInclude("~/Scripts/ckeditor/jquery.ui.dialog-patch.js");
					%>
					<% Html.ScriptInlineInclude(() => {%>
					<script type="text/javascript">
						$(function () {
							$("#<%= Html.NameFor(model => model.Text) %>").ckeditor();
						});
					</script> 
					<% }); %>
				<% } %>
				<%: Html.ValidationMessageFor(model => model.Text)%>
			</div>          

			<input type="submit" value="Add" />
		</fieldset>
	<% } %>


