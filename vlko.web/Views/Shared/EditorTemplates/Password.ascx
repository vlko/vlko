<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div class="editor-label">
	<%: Html.LabelFor(model => model)%>
</div>
<div class="editor-field">
	<%= Html.Password("", ViewData.Model, cssClass: "text-box single-line password")%>
	<%: Html.ValidationMessageFor(model => model)%>
</div>