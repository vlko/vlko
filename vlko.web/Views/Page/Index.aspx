<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.core.Components.PagedModel<vlko.core.Models.Action.ViewModel.StaticTextViewModel>>" %>
<%@ Import Namespace="vlko.web.Controllers" %>
<%@ Import Namespace="vlko.core.Components" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<h2>Pages list</h2>
	<% Html.ScriptInclude("~/Scripts/Grid.js"); %>

	<% foreach (var item in Model) { %>
		<div class="article">
			<h3><%= Html.ActionLink(item.Title, item.FriendlyUrl, "Page") %></h3>
			<div class="article_info">
				<span class="creator"><%: item.Creator.Name %></span>
				<span class="publish_date"><span>Published </span><%: String.Format("{0:g}", item.PublishDate) %></span>
				<span class="last_change_date"><span>Created </span><%: String.Format("{0:g}", item.ChangeDate) %></span>
			</div>
			<div>
				<%: item.Description %>
			</div><% 
			if (item.AllowComments) 
			{%>
			<div class="article_comment"><span>Comments </span><%: item.CommentCounts %></div>
			<% } %>
		</div>
	<% } %>
	<p><% Html.RenderPartial("~/Views/Shared/Pager.ascx", new PagerModel(Model, "Index")); %></p>
	<% Html.ScriptInlineInclude(() => {%>
	<script type="text/javascript">
		$(function () {
			$("#content").ajaxGrid();

			// links
			$("#content h3 a")
				.click(function () {
					createLoading();
					var nextUrl = $(this).attr("href");
					$.ajax({
						url: nextUrl + "?ajaxTime=" + new Date().getTime(),
						success: function (data) {
							var edit = createContentDialog({
								title: 'Article',
								data: data,
								prevUrl: getCurrentHistoryUrl()
								});
							closeLoading();
							edit.dialog("option", "title", $("h2", edit).html());
							edit.dialog("open");
							addToHistory(nextUrl);
						},
						error: ajaxException
					});
					return false;
				});
		});
	</script> 
	<% }); %>	
</asp:Content>

