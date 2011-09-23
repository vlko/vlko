<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div class="clearfix">
	<label><%: ViewData.ModelMetadata.GetDisplayName()%></label>
	<div class="input">
		<ul class="inputs-list"><li><%: ViewData.TemplateInfo.FormattedModelValue%></li></ul>
	</div>
</div>