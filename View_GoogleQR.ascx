<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View_GoogleQR.ascx.cs" Inherits="Playngo.Modules.Authentication.View_GoogleQR" %>

<div class="pass-form pass-form-signUp">
    <asp:HiddenField ID="hfSecretKey" runat="server" />


    <div class="pass-form-title">Sign up Two-factor authentication with Google</div>
    <div class="pass-form-item">
        <div class="item-title">1. Download and install</div>
        <div class="download">
            <a href="https://itunes.apple.com/us/app/google-authenticator/id388497605" target="_blank"><img src="<%=ModulePath %>Resource/images/appStore.png" alt="App Store"/></a>
            <a href="https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2" target="_blank"><img src="<%=ModulePath %>Resource/images/googlePlay.png" alt="Google Play"/></a>
        </div>
    </div>
    <div class="pass-form-item">
        <div class="item-title">2. Scan QR-code</div>
        <div class="scan">
            <asp:Image ID="imgQrCode" runat="server" />
        </div>
        <div class="item-info">Your second-factor backup code is: <asp:Label runat="server" ID="lblManualSetupCode"></asp:Label></div>
        <div class="item-info">This can be used to recover your account in case your mobile phone is lost or stolen.</div>
    </div>
    <div class="pass-form-item">
        <div class="item-title">3. Enter 3FA code from the app:</div>
        <asp:TextBox runat="server" ID="txtCode"></asp:TextBox>
    </div>
    <div class="pass-form-button clearfix">
        <asp:Button runat="server" ID="btnValidate" Text="Verify" CssClass="btn cancel" OnClick="btnValidate_Click" />
    </div>

    <asp:Label runat="server" Font-Bold="true" ID="lblValidationResult"></asp:Label>
</div>

<script type="text/javascript">
(function ($, Sys) {

      $(document).ready(function () {

            $('.pass-form-signUp').on('keydown', function (e) {
                if (e.keyCode === 13) {
                    $("#<%=btnValidate.ClientID%>").click();
                }
            });
        });
}(jQuery, window.Sys));
</script>







 