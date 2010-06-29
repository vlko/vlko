<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.web.ViewModel.Page.PageViewModel>" %>
<%@ Import Namespace="vlko.web.Controllers" %>
<%@ Import Namespace="vlko.web.ViewModel.Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	View
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="page_view_content" class="ajax_container">
	<h2><%: Model.StaticText.Title %></h2>
	<div class="article ajax_content">
		
		<div class="article_info">
			<span class="creator"><%: Model.StaticText.Creator.Name%></span>
			<span class="publish_date"><span>Published </span><%: String.Format("{0:g}", Model.StaticText.PublishDate)%></span>
			<span class="last_change_date"><span>Created </span><%: String.Format("{0:g}", Model.StaticText.ChangeDate)%></span>
		</div>
		<div>
			<%= Model.StaticText.Text%>
		</div><% 
		if (Model.StaticText.AllowComments) {%>
		<div class="article_comment"><span>Comments </span><%: Model.StaticText.CommentCounts%></div>
		<% } %>
	</div>
	<div>
		<%: Html.ActionLink("Back to List", "Index") %>
	</div>
	<div class="ajax_content">
	<%:Html.RouteLink("flat", "PageView", cssClass: "comment_sort flat",
									 routeValues: new {
														friendlyUrl = Html.ViewContext.RouteData.GetRequiredString("friendlyUrl"),
														sort = "flat"
													})%>
	<%:Html.RouteLink("flat desc", "PageView", cssClass: "comment_sort desc",
									 routeValues: new {
														friendlyUrl = Html.ViewContext.RouteData.GetRequiredString("friendlyUrl"),
														sort = "desc"
													})%>
	<%:Html.RouteLink("tree", "PageView", cssClass: "comment_sort tree",
									 routeValues: new {
														friendlyUrl = Html.ViewContext.RouteData.GetRequiredString("friendlyUrl"),
														sort = "tree"
													})%>
	<% if (Model.CommentViewType == CommentViewTypeEnum.FlatDesc) {%>
		<% Html.RenderPartial("NewComment", Model.NewComment); %>
	<% } %>

	<% if (Model.CommentViewType != CommentViewTypeEnum.Tree) {%>
		<% Html.RenderPartial("CommentsFlat", Model.FlatComments); %>
	<% } %>
	<% else { %>
		<% Html.RenderPartial("CommentsTree", Model.TreeComments); %>
	<% } %>

	<% if (Model.CommentViewType != CommentViewTypeEnum.FlatDesc) {%>
		<% Html.RenderPartial("NewComment", Model.NewComment); %>
	<% } %>
	</div>
</div>
<% Html.ScriptInlineInclude(() => {%>
<script type="text/javascript">
	$(function () {
		$("#page_view_content .comment_sort")
				.click(function () {
					var nextUrl = $(this).attr("href");
					$.ajax({
						url: nextUrl + "?ajaxTime=" + new Date().getTime(),
						success: function (data) {
							var content = $("#page_view_content");
							content.html(data);
							closeLoading();
							updateEffect(content);
							addToHistory(nextUrl);
						},
						error: ajaxException
					});

					return false;
				});
		$("#page_view_content .reply_link")
				.click(function (){
					var form = $("#page_view_content form");

					$("#ParentId").val($(this).attr("rel"));
					$("#Name").val("Re: " + $(this).closest(".comment").find(".comment_title").text());

					return false;
				});
		$("#page_view_content form")
				.submit(function () {
					createLoading();
					var form = $(this);
					$.ajax({
						url: form.attr("action") + "?ajaxTime=" + new Date().getTime(),
						type: "POST",
						data: form.serialize(),
						success: function (data) {
							if (data.actionName) {
								var nextUrl = (data.area ? "/" + data.area : "") + "/" + data.controllerName + "/" + data.actionName;
								$.ajax({
									url: nextUrl + "?ajaxTime=" + new Date().getTime(),
									success: function (data) {
										var content = $("#page_view_content");
										content.html(data);
										closeLoading();
										updateEffect(content);
									},
									error: ajaxException
								});
							}
							else {
								var content = $("#page_view_content");
								fillContentWithData(content, data);
								closeLoading();
								updateEffect(content);
							}
						},
						error: ajaxException
					});
					return false;
				}); ;
	});
</script> 
<% }); %>
</asp:Content>

