<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<vlko.model.Action.ViewModel.CommentTreeViewModel>>" %>
<%@ Import Namespace="vlko.web.Controllers" %>
<%@ Import Namespace="vlko.core.Components" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="vlko.web.ViewModel.Page" %>
<% Html.ScriptInclude("~/Scripts/Grid.js"); %>
<% foreach (var item in Model)
   {%>
	<div class="comment" style="margin-left:<%= (10 * Math.Log(2.8 + item.Level) - 10).ToString(CultureInfo.InvariantCulture) %>ex;">
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
	<% Html.RenderPartial("CommentsTree", item.ChildNodes); %>
<%
   }%>

