<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Microsoft.Web.Mvc.Html" %>
<script runat="server">
	private object FormattedValue {
		get {
			if (ViewData.TemplateInfo.FormattedModelValue == ViewData.ModelMetadata.Model) {
				return String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:0.00}", ViewData.ModelMetadata.Model);
			}
			return ViewData.TemplateInfo.FormattedModelValue;
		}
	}
</script>
<div class="clearfix<%= !Html.IsValid(model => model) ? " error" : string.Empty %>">
	<%: Microsoft.Web.Mvc.Html.HtmlHelperExtensions.LabelFor(Html, model => model)%>
	<div class="input">
		<%= Html.TextBoxFor(m => m, cssClass: "large", maxLength: Html.MaxLength(m => m))%>
		<%: Html.ValidationMessageFor(model => model, null, new {@class = "error"})%>
		<% if (!string.IsNullOrWhiteSpace(ViewData.ModelMetadata.Description)) {%>
		<span class="help-block">
			<%= ViewData.ModelMetadata.Description%>
		</span>
		<% } %>
	</div>
</div>