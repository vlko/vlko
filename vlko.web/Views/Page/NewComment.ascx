<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<vlko.core.Models.Action.ActionModel.CommentActionModel>" %>
<%@ Import Namespace="vlko.web.Controllers" %>

	<% 
		Html.ScriptInclude("~/Scripts/jquery.validate.js"); 
		Html.ScriptInclude("~/Scripts/MicrosoftMvcJQueryValidation.js"); 
		Html.EnableClientValidation(); 
	%>
	
	<% using (Html.BeginRouteForm("PageComment",
			new RouteValueDictionary( new { friendlyUrl = Html.ViewContext.RouteData.GetRequiredString("friendlyUrl")} )
			))
	{%>
		<fieldset>
			<legend>Add comment</legend>

			<%: Html.Hidden("Page", Html.ViewContext.RequestContext.HttpContext.Request.QueryString["Page"]) %>
			<%: Html.HiddenFor(model => model.ContentId) %>
			<%: Html.HiddenFor(model => model.ParentId) %>	
			<% if (Model.ChangeUser == null) { %>     
				<%: Html.EditorFor(model => model.AnonymousName) %>	 
			<% } %>    	     
			<% else { %>  
				<%: Html.DisplayFor(model => model.ChangeUser.Name) %>
			<% } %>
			<%: Html.EditorFor(model => model.Name) %>	     
			<%: Html.EditorFor(model => model.Text) %>	          

			<input type="submit" value="Add" />
		</fieldset>
	<% } %>


