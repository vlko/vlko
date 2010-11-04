<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<vlko.model.TwitterStatus>" %>
<article class="twitter_status">
	<div class="twitter_info">
		<% if (string.IsNullOrEmpty(Model.RetweetUser)) {%>
		<a href="http://twitter.com/<%: Model.User %>" class="twitter_user">@<%: Model.User %></a>
		<% } else { %>
		<a href="http://twitter.com/<%: Model.RetweetUser %>" class="twitter_user twitter_retweet ">RT @<%: Model.RetweetUser%></a>
		<% }%>
		<a href="http://twitter.com/<%: Model.User %>/status/<%: Model.TwitterId %>" class="twitter_date"><%:String.Format("{0:g}", Model.CreatedDate)%></a>
	</div>
	<div class="twitter_text"><%= Model.Text %></div>
</article>
