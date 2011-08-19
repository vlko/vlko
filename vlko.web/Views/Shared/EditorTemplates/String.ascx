<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Microsoft.Web.Mvc.Html" %>
<div class="editor-label">
	<%: Microsoft.Web.Mvc.Html.HtmlHelperExtensions.LabelFor(Html, model => model)%>
</div>
<div class="editor-field">
	<%= Html.TextBoxFor(m => m, cssClass: "text-box single-line", maxLength: Html.MaxLength(m => m))%>
	<%: Html.ValidationMessageFor(model => model)%>
	<% if (!string.IsNullOrWhiteSpace(ViewData.ModelMetadata.Description)) {%>
	<span class="editor-hint">
		<%= ViewData.ModelMetadata.Description%>
	</span>
	<% } %>
</div>
