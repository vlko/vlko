<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.core.Models.Action.ActionModel.StaticTextActionModel>" %>
<%@ Import Namespace="vlko.web.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<h2>Details</h2>
	<div class="ajax_content">
		<fieldset>
			<legend>Fields</legend>
		
			<%= Html.DisplayFor(model => model.Id) %>
			<%= Html.DisplayFor(model => model.FriendlyUrl)%>
			<%= Html.DisplayFor(model => model.Title)%>
			<%= Html.DisplayFor(model => model.Text)%>
			<%= Html.DisplayFor(model => model.ChangeDate)%>
			<%= Html.DisplayFor(model => model.PublishDate)%>
			<%= Html.DisplayFor(model => model.AllowComments)%>
		
		</fieldset>
	</div>
	<p>
		<%: Html.ActionLink<StaticPageController>(c => c.Edit(Model.Id), "Edit")%> |
		<%: Html.ActionLink("Back to List", "Index") %>
	</p>

</asp:Content>

