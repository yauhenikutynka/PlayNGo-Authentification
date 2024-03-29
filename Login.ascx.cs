#region Copyright
// 
// DotNetNuke?- http://www.dotnetnuke.com
// Copyright (c) 2002-2014
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
#region Usings

using System;
using System.Linq;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;


using Globals = DotNetNuke.Common.Globals;

#endregion

namespace Playngo.Modules.Authentication
{
    using Host = DotNetNuke.Entities.Host.Host;

	/// <summary>
	/// The Login AuthenticationLoginBase is used to provide a login for a registered user
	/// portal.
	/// </summary>
	/// <remarks>
	/// </remarks>
	public partial class Login : AuthenticationLoginBase
	{
		private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof (Login));

		#region Protected Properties

		/// <summary>
		/// Gets whether the Captcha control is used to validate the login
		/// </summary>
		protected bool UseCaptcha
		{
			get
			{
				return AuthenticationConfig.GetConfig(PortalId).UseCaptcha;
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Check if the Auth System is Enabled (for the Portal)
		/// </summary>
		/// <remarks></remarks>
		public override bool Enabled
		{
			get
			{
				return AuthenticationConfig.GetConfig(PortalId).Enabled;
			}
		}
		
		#endregion

		#region Event Handlers

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			cmdLogin.Click += OnLoginClick;

			cancelLink.NavigateUrl = GetRedirectUrl(false);

			ClientAPI.RegisterKeyCapture(Parent, cmdLogin, 13);

            if (PortalSettings.UserRegistration == (int)Globals.PortalRegistrationType.NoRegistration)
            {
                liRegister.Visible = false;
            }
            lblLogin.Text = Localization.GetSystemMessage(PortalSettings, "MESSAGE_LOGIN_INSTRUCTIONS");

            if (!string.IsNullOrEmpty(Response.Cookies["USERNAME_CHANGED"].Value))
            {
                txtUsername.Text = Response.Cookies["USERNAME_CHANGED"].Value;
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, Localization.GetSystemMessage(PortalSettings, "MESSAGE_USERNAME_CHANGED_INSTRUCTIONS"), ModuleMessage.ModuleMessageType.BlueInfo);
            }

            var returnUrl = Globals.NavigateURL();
            string url;
            if (PortalSettings.UserRegistration != (int)Globals.PortalRegistrationType.NoRegistration)
            {
                if (!string.IsNullOrEmpty(UrlUtils.ValidReturnUrl(Request.QueryString["returnurl"])))
                {
                    returnUrl = Request.QueryString["returnurl"];
                }
                returnUrl = HttpUtility.UrlEncode(returnUrl);

                url = Globals.RegisterURL(returnUrl, Null.NullString);
                registerLink.NavigateUrl = url;
                if (PortalSettings.EnablePopUps && PortalSettings.RegisterTabId == Null.NullInteger
                    && !HasSocialAuthenticationEnabled())
                {
                    registerLink.Attributes.Add("onclick", "return " + UrlUtils.PopUpUrl(url, this, PortalSettings, true, false, 600, 950));
                }
            }
            else
            {
                registerLink.Visible = false;
            }

            //see if the portal supports persistant cookies
            chkCookie.Visible = Host.RememberCheckbox;



            // no need to show password link if feature is disabled, let's check this first
            if (MembershipProviderConfig.PasswordRetrievalEnabled || MembershipProviderConfig.PasswordResetEnabled)
            {
                url = Globals.NavigateURL("SendPassword", "returnurl=" + returnUrl);
                passwordLink.NavigateUrl = url;
                if (PortalSettings.EnablePopUps)
                {
                    passwordLink.Attributes.Add("onclick", "return " + UrlUtils.PopUpUrl(url, this, PortalSettings, true, false, 300, 650));
                }
            }
            else
            {
                passwordLink.Visible = false;
            }


            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["verificationcode"]) && PortalSettings.UserRegistration == (int) Globals.PortalRegistrationType.VerifiedRegistration)
                {
                    if (Request.IsAuthenticated)
                    {
                        Controls.Clear();
                    }

                    var verificationCode = Request.QueryString["verificationcode"];


                    try
                    {
                        UserController.VerifyUser(verificationCode.Replace(".", "+").Replace("-", "/").Replace("_", "="));

						var redirectTabId = PortalSettings.Registration.RedirectAfterRegistration;

	                    if (Request.IsAuthenticated)
	                    {
                            Response.Redirect(Globals.NavigateURL(redirectTabId > 0 ? redirectTabId : PortalSettings.HomeTabId, string.Empty, "VerificationSuccess=true"), true);
	                    }
	                    else
	                    {
                            if (redirectTabId > 0)
                            {
                                var redirectUrl = Globals.NavigateURL(redirectTabId, string.Empty, "VerificationSuccess=true");
                                redirectUrl = redirectUrl.Replace(Globals.AddHTTP(PortalSettings.PortalAlias.HTTPAlias), string.Empty);
                                Response.Cookies.Add(new HttpCookie("returnurl", redirectUrl) { Path = (!string.IsNullOrEmpty(Globals.ApplicationPath) ? Globals.ApplicationPath : "/") });
                            }

                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("VerificationSuccess", LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess);
	                    }
                    }
                    catch (UserAlreadyVerifiedException)
                    {
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("UserAlreadyVerified", LocalResourceFile), ModuleMessage.ModuleMessageType.YellowWarning);
                    }
                    catch (InvalidVerificationCodeException)
                    {
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("InvalidVerificationCode", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
                    }
                    catch (UserDoesNotExistException)
                    {
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("UserDoesNotExist", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
                    }
                    catch (Exception)
                    {
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("InvalidVerificationCode", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
                    }
                }
            }

			if (!Request.IsAuthenticated)
			{
				if (!Page.IsPostBack)
				{
					try
					{
						if (Request.QueryString["username"] != null)
						{
							txtUsername.Text = Request.QueryString["username"];
						}
					}
					catch (Exception ex)
					{
						//control not there 
						Logger.Error(ex);
					}
				}
				try
				{
					Globals.SetFormFocus(string.IsNullOrEmpty(txtUsername.Text) ? txtUsername : txtPassword);
				}
				catch (Exception ex)
				{
					//Not sure why this Try/Catch may be necessary, logic was there in old setFormFocus location stating the following
					//control not there or error setting focus
					Logger.Error(ex);
				}
			}

			var registrationType = PortalSettings.Registration.RegistrationFormType;
		    bool useEmailAsUserName;
            if (registrationType == 0)
            {
				useEmailAsUserName = PortalSettings.Registration.UseEmailAsUserName;
            }
            else
            {
				var registrationFields = PortalSettings.Registration.RegistrationFields;
                useEmailAsUserName = !registrationFields.Contains("Username");
            }

		    plUsername.Text = LocalizeString(useEmailAsUserName ? "Email" : "Username");
		    divCaptcha1.Visible = UseCaptcha;
			divCaptcha2.Visible = UseCaptcha;
		}

        private void OnLoginClick(object sender, EventArgs e)
        {
            if ((UseCaptcha && ctlCaptcha.IsValid) || !UseCaptcha)
            {
                var loginStatus = UserLoginStatus.LOGIN_FAILURE;
                string userName = new PortalSecurity().InputFilter(txtUsername.Text,
                                        PortalSecurity.FilterFlag.NoScripting |
                                        PortalSecurity.FilterFlag.NoAngleBrackets |
                                        PortalSecurity.FilterFlag.NoMarkup);

                //DNN-6093
                //check if we use email address here rather than username
                if (PortalController.GetPortalSettingAsBoolean("Registration_UseEmailAsUserName", PortalId, false))
                {
                    var testUser = UserController.GetUserByEmail(PortalId, userName); // one additonal call to db to see if an account with that email actually exists
                    if (testUser != null)
                    {
                        userName = testUser.Username; //we need the username of the account in order to authenticate in the next step
                    }
                }

                var objUser = UserController.ValidateUser(PortalId, userName, txtPassword.Text, "DNN", string.Empty, PortalSettings.PortalName, IPAddress, ref loginStatus);
                var authenticated = Null.NullBoolean;
                var message = Null.NullString;
                if (loginStatus == UserLoginStatus.LOGIN_USERNOTAPPROVED)
                {
                    message = "UserNotAuthorized";
                }
                else
                {
                    authenticated = (loginStatus != UserLoginStatus.LOGIN_FAILURE);
                }

                if (loginStatus != UserLoginStatus.LOGIN_FAILURE && PortalController.GetPortalSettingAsBoolean("Registration_UseEmailAsUserName", PortalId, false))
                {
                    //make sure internal username matches current e-mail address
                    if (objUser.Username.ToLower() != objUser.Email.ToLower())
                    {
                        UserController.ChangeUsername(objUser.UserID, objUser.Email);
                    }

                    Response.Cookies.Remove("USERNAME_CHANGED");
                }

                //login success
                if (loginStatus != UserLoginStatus.LOGIN_FAILURE)
                {
                    //Administrator does not execute
                    if (!objUser.IsSuperUser)
                    {
                        //Get whether the user's two factor is enable
                        var EnableTwoFactor = ShowUserProfile(objUser, "EnableTwoFactor");
                        if (String.IsNullOrEmpty(EnableTwoFactor) || EnableTwoFactor.ToLower() == "true")
                        {
                            //go two factor page
                            try { Response.Redirect(LoginURL(objUser.UserID, objUser.LastModifiedOnDate)); }
                            finally { }
                        }
                    }
                }
 

                //Raise UserAuthenticated Event
                var eventArgs = new UserAuthenticatedEventArgs(objUser, userName, loginStatus, "DNN")
                {
                    Authenticated = authenticated,
                    Message = message,
                    RememberMe = chkCookie.Checked
                };
                OnUserAuthenticated(eventArgs);



            }
        }

        private bool HasSocialAuthenticationEnabled()
        {
            return (from a in AuthenticationController.GetEnabledAuthenticationServices()
                    let enabled = (a.AuthenticationType == "Facebook"
                                     || a.AuthenticationType == "Google"
                                     || a.AuthenticationType == "Live"
                                     || a.AuthenticationType == "Twitter")
                                  ? PortalController.GetPortalSettingAsBoolean(a.AuthenticationType + "_Enabled", PortalSettings.PortalId, false)
                                  : !string.IsNullOrEmpty(a.LoginControlSrc) && (LoadControl("~/" + a.LoginControlSrc) as AuthenticationLoginBase).Enabled
                    where a.AuthenticationType != "DNN" && enabled
                    select a).Any();
        }
		
		#endregion

		#region Private Methods

		protected string GetRedirectUrl(bool checkSettings = true)
		{
			var redirectUrl = "";
			var redirectAfterLogin = PortalSettings.Registration.RedirectAfterLogin;
			if (checkSettings && redirectAfterLogin > 0) //redirect to after registration page
			{
				redirectUrl = Globals.NavigateURL(redirectAfterLogin);
			}
			else
			{
				if (Request.QueryString["returnurl"] != null)
				{
					//return to the url passed to register
					redirectUrl = HttpUtility.UrlDecode(Request.QueryString["returnurl"]);

                    //clean the return url to avoid possible XSS attack.
                    redirectUrl = UrlUtils.ValidReturnUrl(redirectUrl);

                    if (redirectUrl.Contains("?returnurl"))
					{
						string baseURL = redirectUrl.Substring(0,
							redirectUrl.IndexOf("?returnurl", StringComparison.Ordinal));
						string returnURL =
							redirectUrl.Substring(redirectUrl.IndexOf("?returnurl", StringComparison.Ordinal) + 11);

						redirectUrl = string.Concat(baseURL, "?returnurl", HttpUtility.UrlEncode(returnURL));
					}
				}
				if (String.IsNullOrEmpty(redirectUrl))
				{
					//redirect to current page 
					redirectUrl = Globals.NavigateURL();
				}
			}

			return redirectUrl;
		}

        #endregion






        public String ShowUserProfile(UserInfo uInfo, String name)
        {
            String u = String.Empty;
            if (uInfo != null && uInfo.UserID > 0)
            {
                if (uInfo.Profile != null)
                {

                    u = uInfo.Profile.GetPropertyValue(name);
                    if (!String.IsNullOrEmpty(u) && uInfo.Profile.ProfileProperties[name] != null)
                    {
                        u = uInfo.Profile.ProfileProperties[name].PropertyValue;
                    }
                }
            }
            return !String.IsNullOrEmpty(u) ? HttpUtility.HtmlDecode(u) : "";
        }


        /// <summary>
        /// Gets the login URL.
        /// </summary>
        /// <param name="returnURL">The URL to redirect to after logging in.</param>
        /// <param name="override">if set to <c>true</c>, show the login control on the current page, even if there is a login page defined for the site.</param>
        /// <returns>Formatted URL.</returns>
        public string LoginURL(Int32 StrUserId, DateTime LastModifiedOnDate, string returnURL, bool @override)
        {
            string strURL = "";
            var portalSettings = PortalController.Instance.GetCurrentPortalSettings();
            if (!string.IsNullOrEmpty(returnURL))
            {
                returnURL = String.Format("returnurl={0}", returnURL);
            }
            //var popUpParameter = "";
            //if (HttpUtility.UrlDecode(returnURL).Contains("popUp=true"))
            //{
            //    popUpParameter = "popUp=true";
            //}


            String TokenString = String.Format("token={0}", HttpUtility.UrlEncode(CryptionHelper.EncryptString(StrUserId.ToString(), "123456789")));
            String DateString = String.Format("d={0}", HttpUtility.UrlEncode(CryptionHelper.EncryptString(LastModifiedOnDate.Ticks.ToString(), "x1x2x3x4x5")));
            if (portalSettings.LoginTabId != -1 && !@override)
            {
                if (Globals.ValidateLoginTabID(portalSettings.LoginTabId))
                {
                    strURL = string.IsNullOrEmpty(returnURL)
                                        ? Globals. NavigateURL(portalSettings.LoginTabId, "",  TokenString, DateString)
                                        : Globals.NavigateURL(portalSettings.LoginTabId, "", returnURL,  TokenString, DateString);
                }
                else
                {
                    string strMessage = String.Format("error={0}", Localization.GetString("NoLoginControl", Localization.GlobalResourceFile));
                    //No account module so use portal tab
                    strURL = string.IsNullOrEmpty(returnURL)
                                 ? Globals.NavigateURL(portalSettings.ActiveTab.TabID, "Login", strMessage,  TokenString, DateString)
                                 : Globals.NavigateURL(portalSettings.ActiveTab.TabID, "Login", returnURL, strMessage,  TokenString, DateString);
                }
            }
            else
            {
                //portal tab
                strURL = string.IsNullOrEmpty(returnURL)
                                ? Globals.NavigateURL(portalSettings.ActiveTab.TabID, "Login",  TokenString, DateString)
                                : Globals.NavigateURL(portalSettings.ActiveTab.TabID, "Login", returnURL,  TokenString, DateString);
            }
            return strURL;
        }


        /// <summary>
        /// Login Url
        /// </summary>
        /// <param name="StrUserId">User Id</param>
        /// <param name="LastModifiedOnDate">User last login date</param>
        /// <returns></returns>
        public string LoginURL(Int32 StrUserId,DateTime LastModifiedOnDate)
        {
            return LoginURL(StrUserId, LastModifiedOnDate,UrlReturn, false);
        }


        /// <summary>
        /// Get Return Url
        /// </summary>
        public String UrlReturn = WebHelper.GetStringParam(HttpContext.Current.Request, "returnurl", "");

    }
}