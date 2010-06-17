<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<vlko.core.Models.Action.ViewModel.StaticTextViewModel>" %>
	<div class="ajax_content">      
			<%: Html.DisplayFor(model => model.Title) %>        
			<%: Html.DisplayFor(model => model.Text) %>        
			<%: Html.DisplayFor(model => model.ChangeDate) %>        
			<%: Html.DisplayFor(model => model.Creator.Name) %>             
	</div>


