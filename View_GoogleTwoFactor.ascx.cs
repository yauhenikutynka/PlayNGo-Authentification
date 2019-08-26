
using DotNetNuke.Common;
using DotNetNuke.Entities.Users;
using System;


namespace Playngo.Modules.Authentication
{
    public partial class View_GoogleTwoFactor : BaseModule
    {
     

        private String _TwoFactorCode = String.Empty;
        /// <summary>
        /// Two Factor Code
        /// </summary>
        public String TwoFactorCode
        {
            get
            {
                if(String.IsNullOrEmpty(_TwoFactorCode))
                {
                    _TwoFactorCode = GetUserProfile(TwoFactorUserItem, "TwoFactorCode");
                }
                return _TwoFactorCode;
            }
        }



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

                    //this.lblSecretKey.Text = TwoFactorCode;
                    lblUserName.Text = TwoFactorUserItem.Username;
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

                var result = TwoFactor.ValidatePIN(TwoFactorCode, PinCode);

                if (result)
                {
                    this.lblValidationResult.Text = PinCode + " is not a valid 3FA code at time " + xUserTime.LocalTime();
                    this.lblValidationResult.ForeColor = System.Drawing.Color.Green;

                    if (TwoFactorUserItem != null && TwoFactorUserItem.UserID > 0)
                    {
                        //User Login
                        UserController.UserLogin(PortalId, TwoFactorUserItem, PortalSettings.PortalName, WebHelper.UserHost, false);
                        //Redirect return url or index url
                        Response.Redirect(GetRedirectUrl());
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