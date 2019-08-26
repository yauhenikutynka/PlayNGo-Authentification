using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Web.Client.ClientResourceManagement;
using Playngo.Modules.Authentication.TwoFactor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Playngo.Modules.Authentication
{
    /// <summary>
    /// Base Module Control
    /// </summary>
    public class BaseModule: UserModuleBase
    {
        /// <summary>
        /// Two Factor Interface
        /// </summary>
        public iTwoFactor TwoFactor = (iTwoFactor)Activator.CreateInstance("Playngo.Modules.Authentication", "Playngo.Modules.Authentication.TwoFactor.GoogleTwoFactor").Unwrap();


        /// <summary>
        /// URL Token
        /// </summary>
        public String UrlToken = WebHelper.GetStringParam(HttpContext.Current.Request, "token", "");

        /// <summary>
        /// URL by return page
        /// </summary>
        public String UrlReturn = WebHelper.GetStringParam(HttpContext.Current.Request, "returnurl", "");


        /// <summary>
        /// URL by date
        /// </summary>
        public String UrlDate = WebHelper.GetStringParam(HttpContext.Current.Request, "d", "");



        public Int32 _TwoFactorUserId = -1;
        /// <summary>
        /// Two Factor User Id
        /// </summary>
        public Int32 TwoFactorUserId
        {
            get
            {
                if (!(_TwoFactorUserId > 0))
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(UrlToken))
                        {
                            //Decrypt UserId
                            var UserIdString = CryptionHelper.DecryptString(UrlToken, "123456789");
                            Int32.TryParse(UserIdString, out _TwoFactorUserId);
                        }
                    }
                    catch (Exception exc)
                    {

                    }
                }
                return _TwoFactorUserId;
            }
        }

        public long _TwoFactorUserTicks = 0;
        /// <summary>
        /// Two Factor User Ticks
        /// </summary>
        public long TwoFactorUserTicks
        {
            get
            {
                if (!(_TwoFactorUserTicks > 0))
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(UrlDate))
                        {
                            //Decrypt User LastModifiedOnDate
                            var UserTicksString = CryptionHelper.DecryptString(UrlDate, "x1x2x3x4x5");
                            long.TryParse(UserTicksString, out _TwoFactorUserTicks);
                        }
                    }
                    catch (Exception exc)
                    {

                    }
                }
                return _TwoFactorUserTicks;
            }
        }


        /// <summary>
        /// Two Factor User
        /// </summary>
        public UserInfo TwoFactorUserItem
        {
            get
            {
                if (TwoFactorUserId > 0 && !(_TwoFactorUserItem != null && _TwoFactorUserItem.UserID > 0))
                {
                    _TwoFactorUserItem = UserController.Instance.GetUser(PortalId, TwoFactorUserId);
                }
                return _TwoFactorUserItem;
            }
        }

        private UserInfo _TwoFactorUserItem = new UserInfo();



        /// <summary>
        /// Validate of association status
        /// </summary>
        /// <param name="UserId">User Id</param>
        /// <returns>-1:None; 0:Pin Code; 1:QR Scan; </returns>
        public Int32 ValidateUserAction(Int32 UserId)
        {
            Int32 Action = -2;


            var UserItem = UserController.Instance.GetUser(PortalId, UserId);
            if (UserItem != null && UserItem.UserID > 0)
            {
                //Get Two Factor Code
                var TwoFactorCode = GetUserProfile(UserItem, "TwoFactorCode");

                //Two Factor Type   Google、Microsoft、Authy、LastPass
                //var TwoFactorType = ShowUserProfile(UserItem, "TwoFactorType");

                /* Now only Google Two Factor */
                //if (TwoFactorType == "1")
                //{
                if (String.IsNullOrEmpty(TwoFactorCode))
                    {
                        Action = 1;
                    }
                    else
                    {
                        Action = 2;
                    }
                //}
            }

            return Action;
        }

        /// <summary>
        /// Get User Profile Setting
        /// </summary>
        /// <param name="uInfo">User Info</param>
        /// <param name="name">Property Name</param>
        /// <returns></returns>
        public String GetUserProfile(UserInfo uInfo, String name)
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
        /// redirect to url after login
        /// </summary>
        /// <param name="checkSettings"></param>
        /// <returns></returns>
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
                    redirectUrl = Globals.NavigateURL(PortalSettings.HomeTabId);
                }
            }

            return redirectUrl;
        }



        #region "load js & css to page"

        /// <summary>
        /// load style file to page
        /// </summary>
        /// <param name="Name">style name</param>
        /// <param name="FileName">style path</param>
        public void BindStyleFile(String Name, String FileName)
        {
            BindStyleFile(Name, FileName, 50);
        }

        /// <summary>
        /// load style file to page
        /// </summary>
        /// <param name="Name">style name</param>
        /// <param name="FileName">style path</param>
        /// <param name="priority">load priority</param>
        public void BindStyleFile(String Name, String FileName, int priority)
        {
            if (HttpContext.Current.Items[Name] == null)
            {
                HttpContext.Current.Items.Add(Name, "true");
                ClientResourceManager.RegisterStyleSheet(Page, FileName);
            }
        }

        /// <summary>
        /// load js file to page
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="FileName"></param>
        public void BindJavaScriptFile(String Name, String FileName)
        {
            BindJavaScriptFile(Name, FileName, 50);
        }

        /// <summary>
        /// load js file to page
        /// </summary>
        /// <param name="Name">js name</param>
        /// <param name="FileName">js file path</param>
        /// <param name="priority">load priority</param>
        public void BindJavaScriptFile(String Name, String FileName, int priority)
        {
            if (HttpContext.Current.Items[Name] == null)
            {
                HttpContext.Current.Items.Add(Name, "true");
                ClientResourceManager.RegisterScript(Page, FileName, priority);
            }
        }


        #endregion


        #region "DNN 920 的支持"

        #region "获取模块信息属性DNN920"

        /// <summary>
        /// 获取模块信息属性DNN920
        /// </summary>
        /// <param name="m">模块信息</param>
        /// <param name="Name">属性名</param>
        /// <returns></returns>
        public String ModuleProperty(ModuleInfo m, String Name)
        {
            bool propertyNotFound = false;
            return m.GetProperty(Name, "", System.Globalization.CultureInfo.CurrentCulture, UserInfo, DotNetNuke.Services.Tokens.Scope.DefaultSettings, ref propertyNotFound);
        }

        /// <summary>
        /// 获取模块信息属性DNN920
        /// </summary>
        /// <param name="Name">属性名</param>
        /// <returns></returns>
        public String ModuleProperty(String Name)
        {
            return ModuleProperty(ModuleConfiguration, Name);
        }

        #endregion

        #region "模块路径"
        /// <summary>
        /// 模块路径
        /// </summary>
        public String ModulePath
        {
            get { return ControlPath; }
        }

        #endregion

        #endregion
    }
}