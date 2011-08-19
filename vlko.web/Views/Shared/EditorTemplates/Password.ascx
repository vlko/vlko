<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div class="editor-label">
	<%: Html.LabelFor(model => model)%>
</div>
<div class="editor-field">
	<%= Html.Password("", ViewData.Model, new { @class = "text-box single-line password" })%>
	<%: Html.ValidationMessageFor(model => model)%>
	<% if (!string.IsNullOrWhiteSpace(ViewData.ModelMetadata.Description)) {%>
	<span class="editor-hint">
		<%= ViewData.ModelMetadata.Description%>
	</span>
	<% } %>
</div>
