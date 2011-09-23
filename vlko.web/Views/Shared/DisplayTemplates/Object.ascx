<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<script runat="server">
    bool ShouldShow(ModelMetadata metadata) {
        return metadata.ShowForDisplay
            && metadata.ModelType != typeof(System.Data.EntityState)
            && !metadata.IsComplexType
            && !ViewData.TemplateInfo.Visited(metadata);
    }
</script>
<% if (Model == null) { %>
    <%= ViewData.ModelMetadata.NullDisplayText %>
<% } else if (ViewData.TemplateInfo.TemplateDepth > 1) { %>
    <%= ViewData.ModelMetadata.SimpleDisplayText %>
<% } else { %>
    <% foreach (var prop in ViewData.ModelMetadata.Properties.Where(pm => ShouldShow(pm))) { %>
        <% if (prop.HideSurroundingHtml) { %>
            <%= Html.Display(prop.PropertyName) %>
        <% } else { %>
			<div class="clearfix">
				<% if (!String.IsNullOrEmpty(prop.GetDisplayName())) { %>
					<label><%= prop.GetDisplayName() %></label>
				<% } %>
				<div class="input">
					<ul class="inputs-list"><li><%= Html.Display(prop.PropertyName) %></li></ul>
				</div>
			</div>
        <% } %>
    <% } %>
<% } %>