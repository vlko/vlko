<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.model.Action.CRUDModel.RssFeedCRUDModel>" %>

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
	
	<% using (Html.BeginForm()) {%>
	<div>
		<%: Html.ValidationSummary(cssClass: "ui-state-error ui-corner-all") %>
		
		<fieldset>
			<legend>Fields</legend>
			
			<%: Html.HiddenFor(model => model.Id)%>	     
			<%: Html.EditorFor(model => model.Name) %>	     
			<%: Html.EditorFor(model => model.Url) %>	     
			<%: Html.EditorFor(model => model.AuthorRegex) %>	     
			<%: Html.EditorFor(model => model.GetDirectContent) %>	     
			<%: Html.EditorFor(model => model.DisplayFullContent) %>	     
			<%: Html.EditorFor(model => model.ContentParseRegex) %>	 
			<div class="editor-label"></div><div class="editor-field"><button id="test_button" value="Test">Test</button></div>    

		</fieldset>
	</div>
	<div class="ajax_ignore">
		<input type="submit" value="Save" />
	</div>
	<% } %>

	<div class="ajax_ignore">
		<%: Html.ActionLink("Back to List", "Index") %>
	</div>

	<div id="test_results"></div>

	<% Html.ScriptInlineInclude(() => {%>
	<script type="text/javascript">
		$(function () {
			$("#test_button").click(function (event) {
				var form = $(this).parents("form");
				createLoading();
				$.ajax({
					type: "POST",
					url: "<%: Url.Action("TestFeed") %>",
					data: form.serialize(),
					success: function (data) {
						var content = $("#test_results");
						content.html(data);
						closeLoading();
						updateEffect(content);
					},
					error: ajaxException
				});
				event.preventDefault();
			});
		});
	</script> 
	<% }); %>	

</asp:Content>

