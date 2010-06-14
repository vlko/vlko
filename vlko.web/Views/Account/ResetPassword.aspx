<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<vlko.web.ViewModel.Account.ResetPasswordModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Reset password
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Reset password</h2>
    <p>
        Use the form below to specify email address to start reset password process. 
    </p>

    <% using (Html.BeginForm()) { %>
        <%= Html.ValidationSummary(excludePropertyErrors: true, message: "Reset password was unsuccessful. Please correct the errors and try again.", cssClass: "ui-state-error ui-corner-all")%>
        <div>
            <fieldset>
                <legend>Account email</legend>
                <div class="editor-label">
                    <%= Html.LabelFor(m => m.Email) %>
                </div>
                <div class="editor-field">
                    <%= Html.TextBoxFor(m => m.Email) %>
                    <%= Html.ValidationMessageFor(m => m.Email) %>
                </div>
                
                <p>
                    <input type="submit" value="Reset password" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>
