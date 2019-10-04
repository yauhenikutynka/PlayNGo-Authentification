using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Authentication;
using Playngo.Modules.Authentication.TwoFactor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Playngo.Modules.Authentication
{

    /// <summary>
    /// Google TwoFactor QR Scan Page
    /// </summary>
    public partial class View_GoogleQR : BaseModule
    {



        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                if (!IsPostBack)
                {


                    if (!(Request.UrlReferrer != null && !String.IsNullOrEmpty(Request.UrlReferrer.ToString())))
                    {
                        Response.Redirect(Globals.LoginURL(UrlReturn, false));
                    }
                    else if (TwoFactorUserItem.LastModifiedOnDate.Ticks != TwoFactorUserTicks)
                    {
                        Response.Redirect(Request.RawUrl);
                    }

                    //Generate Random Secret Key
                    hfSecretKey.Value = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);

                    //
                    var setupInfo = TwoFactor.GenerateSetupCode("Google Two Factor", TwoFactorUserItem.Username, hfSecretKey.Value, 150, 150);

                    string qrCodeImageUrl = setupInfo.QrCodeSetupImageUrl;
                    string manualEntrySetupCode = setupInfo.ManualEntryKey;

                    this.imgQrCode.ImageUrl = qrCodeImageUrl;
                    this.lblManualSetupCode.Text = manualEntrySetupCode;
                }
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        protected void btnValidate_Click(object sender, EventArgs e)
        {

            String PinCode = this.txtCode.Text.Trim();

            if (!String.IsNullOrEmpty(PinCode))
            {
                var result = TwoFactor.ValidatePIN(hfSecretKey.Value, PinCode);

                if (result)
                {
                    this.lblValidationResult.Text = PinCode + " is a valid 3FA code at time " + xUserTime.LocalTime();
                    this.lblValidationResult.ForeColor = System.Drawing.Color.Green;

                    if (TwoFactorUserItem != null && TwoFactorUserItem.UserID > 0)
                    {
                        TwoFactorUserItem.Profile.SetProfileProperty("TwoFactorCode", hfSecretKey.Value);

                        DataCache.ClearPortalCache(PortalId, true);
                        DataCache.ClearCache();

                        //save profile
                        UserController.UpdateUser(PortalId, TwoFactorUserItem);
                        ProfileController.UpdateUserProfile(TwoFactorUserItem);
                        //User Login
                        UserController.UserLogin(PortalId, TwoFactorUserItem, PortalSettings.PortalName, WebHelper.UserHost, false);
                        //Redirect return url or index url
                        try { Response.Redirect(GetRedirectUrl()); }
                        finally { }
                    }
                }
                else
                {
                    this.lblValidationResult.Text = PinCode + " is not a valid 3FA code at time " + xUserTime.LocalTime();
                    this.lblValidationResult.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                this.lblValidationResult.Text = "Please enter 3FA code.";
                this.lblValidationResult.ForeColor = System.Drawing.Color.Red;
            }


 
        }
    }
}
