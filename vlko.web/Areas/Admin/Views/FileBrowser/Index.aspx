<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.web.Areas.Admin.ViewModel.FileBrowser.FileBrowserViewModel>" %>
<%@ Import Namespace="vlko.web.Areas.Admin.Controllers" %>
<%@ Import Namespace="vlko.web.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="FileBrowser" class="ajax_content">
	<% 
		Html.ScriptInclude("~/Scripts/Grid.js");
		Html.ScriptInclude("~/Scripts/upload/ajaxupload.js");
		Html.ScriptInclude("~/Scripts/jquery.validate.min.js");
		Html.ScriptInclude("~/Scripts/MicrosoftMvcJQueryValidation.js");
		Html.EnableClientValidation(); 
	%>
	<% Html.ScriptInlineInclude(() => {%>
	<script type="text/javascript">
		$(function () {
			var fileInput = $("#File");
			var form = fileInput.parents("form");
			$("input[type=submit]", form).hide();
			new AjaxUpload(fileInput, {
				action: form.attr("action"),
				name: 'File',
				onSubmit: function () {
					if (form.valid()) {
						createLoading();
						this.disable();
						var fileName = $("#<%=  Html.IdFor(model => model.Ident)%>");
						var ident = fileName.attr("name");
						this.setData({
							"X_REQUESTED_WITH": "XMLHttpRequest",
							ident: fileName.val()
						});
					}
				},
				onComplete: function (file, responseDoc) {
					var content = $("#FileBrowser");
					var html = $("#FileBrowser", responseDoc).html();
					$("script:not(:empty)", responseDoc).each(function () {
						debugger;
						html += "<" + this.tagName + ' type="text/javascript">' + $(this).html() + "</" + this.tagName + ">";
					});
					console.log(html);
					content.html(html);
					closeLoading();
					updateEffect(content);
				}
			});
		});
	</script> 
	<% }); %>

	<h2>User uploaded files</h2>
	<% using (Html.BeginForm("Upload", encType: "multipart/form-data")) {%>
		<%: Html.ValidationSummary(cssClass: "ui-state-error ui-corner-all")%>
	<div>	
		<fieldset>
			<legend>Upload new file</legend>
			<%: Html.EditorFor(model => model.Ident) %>
			<div class="editor-label">
				<label  for="file">Filename:</label>
			</div>
			<div class="editor-field">
				<input  type="file" name="File" id="File"  />
			</div>
			<input type="submit" value="Upload" />
		</fieldset>
	<% } %>
	<table>
		<tr>
			<th></th>
			<th><%= vlko.core.ModelResources.Url  %></th>
			<th><%= vlko.core.ModelResources.Size  %></th>
		</tr>

	<% foreach (var item in Model.UserFiles) { %>
	
		<tr>
			<td><%: Html.ActionLink<FileBrowserController>(c => c.Delete(item.Ident), "Delete", new { @class = "grid_delete", title = "Delete"})%> </td>
			<td><a href="<%: item.Url %>" target="Preview"><%: item.Url %></a></td>
			<td><%: item.Size %></td>
		</tr>
	
	<% } %>

	</table>
</div>
<% Html.ScriptInlineInclude(() => {%>
<script type="text/javascript">
	$(function () {
		$("#FileBrowser").ajaxGrid();
	});
</script> 
<% }); %>
</asp:Content>

