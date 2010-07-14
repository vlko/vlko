<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Globalization" %>
<div class="editor-label">
	date <%: Html.LabelFor(model => model)%>
</div>
<div class="editor-field">
	<%= Html.TextBox("", Html.FormatDateValue(ViewData.Model), cssClass: "text-box single-line")%>
	<%: Html.ValidationMessageFor(model => model)%>
	<% if (string.IsNullOrWhiteSpace(ViewData.ModelMetadata.Description)) {%>
	<span class="editor-hint">
		<%= ViewData.ModelMetadata.Description%>
	</span>
	<% } %>
</div>
<% Html.ScriptInlineInclude(() => {%>
<script type="text/javascript">
$(function() {
	$("#<%= ViewData.ModelMetadata.PropertyName%>").datepicker({dateFormat: "<%= Html.GetCurrentJsDatePattern() %>", showAnim:"fadeIn"});
});
</script> 
<% }); %>