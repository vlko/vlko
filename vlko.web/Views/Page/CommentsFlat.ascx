<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<vlko.core.Components.PagedModel<vlko.core.Models.Action.ViewModel.CommentViewModel>>" %>
<%@ Import Namespace="vlko.web.Controllers" %>
<%@ Import Namespace="vlko.core.Components" %>
<% Html.ScriptInclude("~/Scripts/Grid.js"); %>
<div class="flat_comments">
<% foreach (var item in Model) { %>
	<div class="comment">
		<div class="comment_user">
			<span class="creator"><%: (item.Owner == null) ? "anonymous-" + item.AnonymousName : item.Owner.Name %></span>
			<span><%: String.Format("{0:g}", item.CreatedDate) %><%: item.Changed ? " ver. " + item.Version : string.Empty %></span>
		</div>
		<div class="comment_title"><%: item.Name %></div>
		<div class="comment_text"><%= item.Text %></div>
	</div>
<% } %>
</div>
<p><% Html.RenderPartial("~/Views/Shared/Pager.ascx", new PagerModel(Model, Html.ViewContext.RouteData.GetRequiredString("friendlyUrl")), new ViewDataDictionary(new { content = "page_view_content" })); %></p>

