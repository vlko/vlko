<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div class="editor-label">
	<%: Html.LabelFor(model => model)%>
</div>
<div class="editor-field">
	<%= Html.TextBox("", ViewData.Model, cssClass: "text-box single-line")%>
	<%: Html.ValidationMessageFor(model => model)%>
</div>