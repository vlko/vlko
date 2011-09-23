<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div class="clearfix">
	<label><%: ViewData.ModelMetadata.GetDisplayName()%></label>
	<div class="input">
		<ul class="inputs-list"><li><a href="mailto:<%= Html.AttributeEncode(Model) %>"><%= Html.Encode(ViewData.TemplateInfo.FormattedModelValue) %></a></li></ul>
	</div>
</div>
