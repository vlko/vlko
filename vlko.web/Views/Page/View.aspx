<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.core.Models.Action.ViewModel.StaticTextWithFullTextViewModel>" %>
<%@ Import Namespace="vlko.web.Controllers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	View
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<h2><%: Model.Title %></h2>
	<div class="article ajax_content">
		
		<div class="article_info">
			<span class="creator"><%: Model.Creator.Name %></span>
			<span class="publish_date"><span>Published </span><%: String.Format("{0:g}", Model.PublishDate)%></span>
			<span class="last_change_date"><span>Created </span><%: String.Format("{0:g}", Model.ChangeDate)%></span>
		</div>
		<div>
			<%= Model.Text%>
		</div><% 
		if (Model.AllowComments) 
		{%>
		<div class="article_comment"><span>Comments </span><%: Model.CommentCounts%></div>
		<% } %>
	</div>

	<p>
		<%: Html.ActionLink("Back to List", "Index") %>
	</p>

</asp:Content>

