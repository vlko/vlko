<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<vlko.core.Models.Action.ViewModel.StaticTextViewModel>" %>
<%@ Import Namespace="vlko.core.Tools" %>  
<article class="comment">
	<div class="comment_user">
		<span class="creator"><%: Model.Creator.Name%></span>
		<span><%:String.Format("{0:g}", Model.ChangeDate)%></span>
	</div>
	<div class="comment_title"><%=Html.ActionLink(Model.Title, Model.FriendlyUrl, "Page")%></div>
	<div class="comment_text"><%=Model.Description.Shorten(250)%></div>
</article>


