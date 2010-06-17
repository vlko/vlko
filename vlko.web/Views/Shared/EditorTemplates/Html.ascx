<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="vlko.web.Areas.Admin.Controllers" %>
<%@ Import Namespace="vlko.web.Controllers" %>
<div class="editor-label">
	<%: Html.LabelFor(model => model)%>
</div>
<div class="editor-field">
<%
	string fileBrowserLink = "FileBrowserLink" + Html.IdForModel();
	if (Request.IsAuthenticated) {
%>
		<div><%= Html.ActionLink<FileBrowserController>(c => c.Index(), "Browse user files", new { id = fileBrowserLink, target="FileBrowser" })%></div>
<%
	}
%>
	<%= Html.TextArea("", (string)ViewData.Model, cssClass: "text-box multi-line")%>
	<%: Html.ValidationMessageFor(model => model)%>
</div>
<% 
	Html.ScriptInclude("~/Scripts/ckeditor/ck_mvc_integration.js");
	Html.ScriptInclude("~/Scripts/ckeditor/ckeditor.js");
	Html.ScriptInclude("~/Scripts/ckeditor/adapters/jquery.js");
	Html.ScriptInclude("~/Scripts/ckeditor/jquery.ui.dialog-patch.js");
%>
<% Html.ScriptInlineInclude(() => {%>
<script type="text/javascript">
	$(function () {
		$("#<%= ViewData.ModelMetadata.PropertyName%>").ckeditor();
		$("#<%= fileBrowserLink %>")
				.click(function () {
					createLoading();
					$.ajax({
						url: this.href + "?ajaxTime=" + new Date().getTime(),
						success: function (data) {
							var fileBrowser = createContentDialog('File browser', data, null, 'file_browser_dialog');
							closeLoading();
							fileBrowser.dialog("open");
						},
						error: ajaxException
					});
					return false;
				}); ;
	});
</script> 
<% }); %>