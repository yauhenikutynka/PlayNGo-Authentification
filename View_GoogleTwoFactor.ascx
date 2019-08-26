<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View_GoogleTwoFactor.ascx.cs" Inherits="Playngo.Modules.Authentication.View_GoogleTwoFactor" %>
<div class="pass-form pass-form-signIn">
    <div class="pass-form-title">Sign in Two-factor authentication with Google</div>
    <div class="pass-form-signIn-item">
        <span class="item-title">User Name:</span><span><asp:Label runat="server" ID="lblUserName"></asp:Label></span>
    </div>
    <div class="pass-form-item">
        <div class="item-title">Enter 3FA code from the app:</div>
        <asp:TextBox runat="server" ID="txtCode"></asp:TextBox>
    </div>
    <div class="pass-form-button clearfix">
        <asp:Button runat="server" ID="btnValidate" CssClass="btn cancel" Text="Verify" OnClick="btnValidate_Click" />
    </div>
   <asp:Label runat="server" Font-Bold="true" ID="lblValidationResult"></asp:Label>    
</div>

<script type="text/javascript">
(function ($, Sys) {

      $(document).ready(function () {

            $('.pass-form-signIn').on('keydown', function (e) {
                if (e.keyCode === 13) {
                    $("#<%=btnValidate.ClientID%>").click();
                }
            });
        });
}(jQuery, window.Sys));
</script>