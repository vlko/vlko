<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<vlko.model.Action.ViewModel.CommentSearchViewModel>" %>
<%@ Import Namespace="vlko.core.Models" %>
<%@ Import Namespace="vlko.core.Tools" %>     
<article class="comment">
	<div class="comment_user">
		<span class="creator"><%:(Model.Owner == null) ? "anonymous-" + Model.AnonymousName : Model.Owner.Name%></span>
		<span><%:String.Format("{0:g}", Model.CreatedDate)%><%:Model.Changed ? " ver. " + Model.Version : string.Empty%></span>
	</div>
	<div class="comment_title"><%:Html.RouteLink(Model.Name, "PageView", 
									 routeValues: new {
														friendlyUrl = ((StaticText)Model.Content).FriendlyUrl,
														sort = "tree"
													})%></div>
	<div class="comment_text"><%=Model.Text.RemoveTags().Shorten(200)%></div>
</article>
