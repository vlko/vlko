<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div class="editor-label">
	<%: Html.LabelFor(model => model)%>
</div>
<div class="editor-field">
	<%= Html.TextArea("", (string)ViewData.Model, cssClass: "text-box multi-line")%>
	<%: Html.ValidationMessageFor(model => model)%>

</div>
<% 
	Html.ScriptInclude("~/Scripts/ckeditor/ck_mvc_integration.js");
	Html.ScriptInclude("~/Scripts/ckeditor/ckeditor.js");
	Html.ScriptInclude("~/Scripts/ckeditor/adapters/jquery.js"); 
%>
<% Html.ScriptInlineInclude(() => {%>
<script type="text/javascript">
	$(function () {
		$("#<%= ViewData.ModelMetadata.PropertyName%>").ckeditor();
	});
</script> 
<% }); %>