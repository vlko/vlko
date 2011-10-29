<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<script runat="server">
	bool ShouldShow(ModelMetadata metadata) {
		return metadata.ShowForEdit
			&& !metadata.IsComplexType
			&& !ViewData.TemplateInfo.Visited(metadata);
	}
</script>
<% if (ViewData.TemplateInfo.TemplateDepth > 1) { %>
	<% if (Model == null) { %>
		<%= ViewData.ModelMetadata.NullDisplayText %>
	<% } else { %>
		<%= ViewData.ModelMetadata.SimpleDisplayText %>
	<% } %>
<% } else { %>
	<% foreach (var prop in ViewData.ModelMetadata.Properties.Where(pm => ShouldShow(pm))) { %>
		<% if (prop.HideSurroundingHtml) { %>
			<%= Html.Editor(prop.PropertyName) %>
		<% } else { %>
			<div class="clearfix<%= !Html.IsValid(model => model) ? " error" : string.Empty %>">
				<% if (!String.IsNullOrEmpty(Html.Label(prop.PropertyName).ToHtmlString())) { %>
					<%= Html.Label(prop.PropertyName) %>
				<% } %>
				<div class="input">
					<%= Html.Editor(prop.PropertyName) %>
					<%= Html.ValidationMessage(prop.PropertyName, "*", new {@class = "error"}) %>
					<% if (!string.IsNullOrWhiteSpace(ViewData.ModelMetadata.Description)) {%>
						<span class="help-block">
							<%= ViewData.ModelMetadata.Description%>
						</span>
					<% } %>
				</div>
			</div>
		<% } %>
	<% } %>
<% } %>