<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<vlko.model.Action.ViewModel.RssItemViewModel>" %>
<article class="rss_item">
	<div class="rss_info">
		<a class="rss_title" href="<%: Model.Url %>"><%: Model.Title %></a>
		<span class="rss_date"><%: String.Format("{0:g}", Model.Published) %></span>
	</div>
	<div class="rss_text"><%= Model.Description %></div>
</article>