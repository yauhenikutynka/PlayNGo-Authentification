using System;
using DotNetNuke.Services.Localization;



namespace Playngo.Modules.Authentication
{
    public partial class View_Index : BaseModule
    {
 
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
               
                if(UserId > 0)
                {
                    //login user redirect to return url or index url
                    Response.Redirect(GetRedirectUrl(false));
                }


                
                BaseModule ManageContent = new BaseModule();
                String ManageName = "View_Login";

                if (!String.IsNullOrEmpty(UrlToken))
                {
                    if (TwoFactorUserId > 0)
                    {
                        //get user enable two factor
                        var EnableTwoFactor = GetUserProfile(TwoFactorUserItem, "EnableTwoFactor");
                        if (String.IsNullOrEmpty(EnableTwoFactor) || EnableTwoFactor.ToLower() == "true")
                        {
                            //get user validate action
                            Int32 Action = ValidateUserAction(TwoFactorUserId);


                            if (Action == 1)
                            {

                                ManageName = "View_GoogleQR";
                            }
                            else if (Action == 2)
                            {

                                ManageName = "View_GoogleTwoFactor";
                            }
                        }
                    }

                }

                //Load Control
                string ContentSrc = ResolveClientUrl(string.Format("{0}/{1}.ascx", this.TemplateSourceDirectory, ManageName));
                if (System.IO.File.Exists(MapPath(ContentSrc)))
                {
                    ManageContent = (BaseModule)LoadControl(ContentSrc);
                    ManageContent.ModuleConfiguration = ModuleConfiguration;
                    ManageContent.ID = ManageName;
                    ManageContent.LocalResourceFile = Localization.GetResourceFile(this, string.Format("{0}.ascx.resx", ManageName));
                    phContainer.Controls.Add(ManageContent);
                }


                BindStyleFile("authentication-module-css", String.Format("{0}Resource/css/module.css", ModulePath));
            }
            catch (Exception exc) //Module failed to load
            {
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }


        }





    }
}