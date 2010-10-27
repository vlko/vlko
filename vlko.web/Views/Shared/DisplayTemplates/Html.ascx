<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div class="display-label"><%: ViewData.ModelMetadata.GetDisplayName()%></div>
<div class="display-field"><%= ViewData.TemplateInfo.FormattedModelValue%></div>