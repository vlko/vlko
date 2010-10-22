<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<vlko.core.Components.PagedModel<vlko.model.Action.ViewModel.CommentViewModel>>" %>
<%@ Import Namespace="vlko.web.Controllers" %>
<%@ Import Namespace="vlko.core.Components" %>
<%@ Import Namespace="vlko.web.ViewModel.Page" %>
<% Html.ScriptInclude("~/Scripts/Grid.js"); %>
<div class="flat_comments">
<% foreach (var item in Model)
   {%>
	<div class="comment">
		<div class="comment_user">
			<span class="creator"><%:(item.Owner == null) ? "anonymous-" + item.AnonymousName : item.Owner.Name%></span>
			<span><%:String.Format("{0:g}", item.CreatedDate)%><%:item.Changed ? " ver. " + item.Version : string.Empty%></span>
			<%:Html.RouteLink("reply", "PageCommentReply", cssClass: "reply_link", rel: item.Id.ToString(),
									 routeValues: new
													{
														friendlyUrl = Html.ViewContext.RouteData.GetRequiredString("friendlyUrl"),
														parentId = item.Id,
														sort = Html.ViewContext.RouteData.GetRequiredString("sort")
													})%>
		</div>
		<div class="comment_title"><%=item.Name%></div>
		<div class="comment_text"><%=item.Text%></div>
	</div>
<%
   }%>
</div>
<p><% Html.RenderPartial("~/Views/Shared/Pager.ascx", 
		new PagerModel(
			Model,
			Html.ViewContext.RouteData.GetRequiredString("friendlyUrl") + "/" + Html.ViewContext.RouteData.GetRequiredString("sort")), 
			new ViewDataDictionary(new { content = "page_view_content" })); %></p>

