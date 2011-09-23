<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Microsoft.Web.Mvc.Html" %>
<div class="clearfix<%= !Html.IsValid(model => model) ? " error" : string.Empty %>">
	<%: Microsoft.Web.Mvc.Html.HtmlHelperExtensions.LabelFor(Html, model => model)%>
	<div class="input">
		<%= Html.PasswordFor(m => m, cssClass: "xlarge", maxLength: Html.MaxLength(m => m))%>
		<%: Html.ValidationMessageFor(model => model, null, new { @class = "help-inline" })%>
		<% if (!string.IsNullOrWhiteSpace(ViewData.ModelMetadata.Description)) {%>
		<span class="help-block">
			<%= ViewData.ModelMetadata.Description%>
		</span>
		<% } %>
	</div>
</div>