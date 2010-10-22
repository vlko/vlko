<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<vlko.model.Action.ViewModel.StaticTextViewModel>" %>
	<div class="ajax_content">      
			<%: Html.DisplayFor(model => model.Title) %>        
			<%: Html.DisplayFor(model => model.Description) %>        
			<%: Html.DisplayFor(model => model.ChangeDate) %>        
			<%: Html.DisplayFor(model => model.Creator.Name) %>             
	</div>


