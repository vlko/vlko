<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.model.Action.CRUDModel.RssFeedCRUDModel>" %>
<%@ Import Namespace="vlko.web.Areas.Admin.Controllers" %>
<%@ Import Namespace="vlko.web.Controllers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<h2>Details</h2>

	<div>
		<fieldset>
			<legend>Fields</legend>
		
			<%: Html.DisplayFor(model => model.Id) %>
			<%: Html.DisplayFor(model => model.Name) %>
			<%: Html.DisplayFor(model => model.Url) %>
			<%: Html.DisplayFor(model => model.AuthorRegex) %>
			<%: Html.DisplayFor(model => model.GetDirectContent) %>
			<%: Html.DisplayFor(model => model.DisplayFullContent) %>
			<%: Html.DisplayFor(model => model.ContentParseRegex) %>

		</fieldset>
	</div>
	<div class="ajax_ignore">
		<%: Html.ActionLink<RssFeedController>(c => c.Edit(Model.Id), "Edit")%> |
		<%: Html.ActionLink("Back to List", "Index") %>
	</div>

</asp:Content>

