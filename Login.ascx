<%@ Control Language="C#" Inherits="Playngo.Modules.Authentication.Login" AutoEventWireup="false" CodeBehind="Login.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<div class="dnnForm dnnLoginService dnnClear">
    <div class="dnnFormItem">
        <div class="dnnLabel">
            <asp:Label ID="plUsername" AssociatedControlID="txtUsername" runat="server" CssClass="dnnFormLabel" />
        </div>
        <asp:TextBox ID="txtUsername" runat="server" />
    </div>
    <div class="dnnFormItem">
        <div class="dnnLabel">
            <asp:Label ID="plPassword" AssociatedControlID="txtPassword" runat="server" resourcekey="Password" CssClass="dnnFormLabel" ViewStateMode="Disabled" />
        </div>
        <asp:TextBox ID="txtPassword" TextMode="Password" runat="server" />
    </div>
    <div class="dnnFormItem" id="divCaptcha1" runat="server" visible="false">
        <asp:Label ID="plCaptcha" AssociatedControlID="ctlCaptcha" runat="server" resourcekey="Captcha" CssClass="dnnFormLabel" />
    </div>
    <div class="dnnFormItem dnnCaptcha" id="divCaptcha2" runat="server" visible="false">
        <dnn:CaptchaControl ID="ctlCaptcha" CaptchaWidth="130" CaptchaHeight="40" runat="server" ErrorStyle-CssClass="dnnFormMessage dnnFormError dnnCaptcha" ViewStateMode="Disabled" />
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblLogin" runat="server" AssociatedControlID="cmdLogin" CssClass="dnnFormLabel" ViewStateMode="Disabled" />
        <asp:LinkButton ID="cmdLogin" resourcekey="cmdLogin" CssClass="dnnPrimaryAction cmdLogin" Text="Login" runat="server" />
        <asp:HyperLink ID="cancelLink" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" CausesValidation="false" />

    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblLoginRememberMe" runat="server" AssociatedControlID="cmdLogin" CssClass="dnnFormLabel" />
        <span class="dnnLoginRememberMe">
            <asp:CheckBox ID="chkCookie" resourcekey="Remember" runat="server" /></span>
    </div>
    <div class="dnnFormItem">
        <label class="dnnFormLabel">&nbsp;</label>
        <div class="dnnLoginActions">
            <ul class="dnnActions dnnClear">
                <li id="liRegister" runat="server">
                    <asp:HyperLink ID="registerLink" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdRegister" ViewStateMode="Disabled" /></li>
                <li id="liPassword" runat="server">
                    <asp:HyperLink ID="passwordLink" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdPassword" ViewStateMode="Disabled" /></li>
            </ul>
        </div>
    </div>
</div>
<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function setUpLogin() {
            var actionLinks = $("a#dnn_ctr<%#ModuleId%>_Login_Login_DNN_cmdLogin");
            actionLinks.click(function () {
                if ($(this).hasClass("dnnDisabledAction")) {
                    return false;
                }

                actionLinks.addClass("dnnDisabledAction");
            });
        }

        $(document).ready(function () {

            $('.dnnLoginService').on('keydown', function (e) {
                if (e.keyCode === 13) {
                    var $loginButton = $('.cmdLogin');
                    if ($loginButton.hasClass("dnnDisabledAction")) {
                        return false;
                    }

                    $loginButton.addClass("dnnDisabledAction");
                    eval($loginButton.attr('href'));
                    e.preventDefault();
                    return false;
                }
            });

            setUpLogin();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                setUpLogin();
            });
        });
    }(jQuery, window.Sys));
</script>
