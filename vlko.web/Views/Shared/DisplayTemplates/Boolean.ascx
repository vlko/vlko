<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Microsoft.Web.Mvc.Html" %>
<script runat="server">
	private List<SelectListItem> TriStateValues {
		get {
			return new List<SelectListItem> {
				new SelectListItem { Text = "Not Set", Value = String.Empty, Selected = !Value.HasValue },
				new SelectListItem { Text = "True", Value = "true", Selected = Value.HasValue && Value.Value },
				new SelectListItem { Text = "False", Value = "false", Selected = Value.HasValue && !Value.Value },
			};
		}
	}
	private bool? Value {
		get {
			if (ViewData.Model == null) {
				return null;
			}
			return Convert.ToBoolean(ViewData.Model, System.Globalization.CultureInfo.InvariantCulture);
		}
	}
</script>
<div class="clearfix">
	<% if (ViewData.ModelMetadata.IsNullableValueType) { %>
		<%: Microsoft.Web.Mvc.Html.HtmlHelperExtensions.LabelFor(Html, model => model)%>
	<% } %>
	<div class="input">
		<% if (ViewData.ModelMetadata.IsNullableValueType) { %>
			<ul class="inputs-list"><li><%= Html.DropDownList("", TriStateValues, new { @class = "large", @disabled = "disabled" })%></li></ul>
		<% } else { %>
		<ul class="inputs-list">
			<li><label>
				<%= Html.CheckBox("", Value ?? false, new { @disabled = "disabled" })%>
				<span><%: ViewData.ModelMetadata.DisplayName ?? ViewData.ModelMetadata.PropertyName%></span>
			</label></li>
		</ul>
		<% } %>
	</div>
</div>