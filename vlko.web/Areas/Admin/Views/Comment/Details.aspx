<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.model.Action.CRUDModel.CommentCRUDModel>" %>
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
			<%: Html.DisplayFor(model => model.ContentId) %>        
			<%: Html.DisplayFor(model => model.Name) %>        
			<%: Html.DisplayFor(model => model.Text) %>        
			<%: Html.DisplayFor(model => model.ChangeDate) %>        
			<%: Html.DisplayFor(model => model.ParentId) %>        
			<%: Html.DisplayFor(model => model.AnonymousName) %>        
			<%: Html.DisplayFor(model => model.ClientIp) %>        
			<%: Html.DisplayFor(model => model.UserAgent) %>        

		</fieldset>
	</div>
	<p class="ajax_ignore">
		<%: Html.ActionLink<CommentController>(c => c.Edit(Model.Id), "Edit")%> |
		<%: Html.ActionLink("Back to List", "Index") %>
	</p>

</asp:Content>

