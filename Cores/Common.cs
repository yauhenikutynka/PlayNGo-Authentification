using System;
using System.Web;


 
using System.Text.RegularExpressions;


namespace Playngo.Modules.Authentication
{
    /// <summary>
    /// Common Helper
    /// </summary>
    public class Common
    {



        /// <summary>
        /// Lost XSS String
        /// </summary>
        /// <param name="Str"></param>
        /// <returns>safe string</returns>
        public static string LostXSS(string Str)
        {
            string Re_Str = Str;

            if (!String.IsNullOrEmpty(Str))
            {
                string Pattern = "<\\/*[^<>]*>";
                Re_Str = Regex.Replace(HttpUtility.HtmlDecode(Str), Pattern, "");
                Re_Str = (Re_Str.Replace("\r\n", "")).Replace("\r", "");
                Re_Str = Common.ReplaceNoCase(Re_Str, "<", "");
                Re_Str = Common.ReplaceNoCase(Re_Str, ">", "");
                Re_Str = Common.ReplaceNoCase(Re_Str, "javascript", "");
                Re_Str = Common.ReplaceNoCase(Re_Str, "script", "");
                Re_Str = Common.ReplaceNoCase(Re_Str, "cookie", "");
                Re_Str = Common.ReplaceNoCase(Re_Str, "iframe", "");
                Re_Str = Common.ReplaceNoCase(Re_Str, "expression", "");

         
                Re_Str = Common.ReplaceNoCase(Re_Str, "onabort", "");
                Re_Str = Common.ReplaceNoCase(Re_Str, "onblur", ""); 
                Re_Str = Common.ReplaceNoCase(Re_Str, "onchange", ""); 
                Re_Str = Common.ReplaceNoCase(Re_Str, "onclick", ""); 
                Re_Str = Common.ReplaceNoCase(Re_Str, "ondblclick", ""); 
                Re_Str = Common.ReplaceNoCase(Re_Str, "onerror", ""); 
                Re_Str = Common.ReplaceNoCase(Re_Str, "onfocus", ""); 
                Re_Str = Common.ReplaceNoCase(Re_Str, "onkeydown", ""); 
                Re_Str = Common.ReplaceNoCase(Re_Str, "onkeypress", "");
                Re_Str = Common.ReplaceNoCase(Re_Str, "onkeyup", ""); 
                Re_Str = Common.ReplaceNoCase(Re_Str, "onload", ""); 
                Re_Str = Common.ReplaceNoCase(Re_Str, "onmousedown", ""); 
                Re_Str = Common.ReplaceNoCase(Re_Str, "onmousemove", "");
                Re_Str = Common.ReplaceNoCase(Re_Str, "onmouseout", ""); 
                Re_Str = Common.ReplaceNoCase(Re_Str, "onmouseover", ""); 
                Re_Str = Common.ReplaceNoCase(Re_Str, "onmouseup", ""); 
                Re_Str = Common.ReplaceNoCase(Re_Str, "onreset", ""); 
                Re_Str = Common.ReplaceNoCase(Re_Str, "onresize", ""); 
                Re_Str = Common.ReplaceNoCase(Re_Str, "onselect", ""); 
                Re_Str = Common.ReplaceNoCase(Re_Str, "onsubmit", ""); 
                Re_Str = Common.ReplaceNoCase(Re_Str, "onunload", "");


            }
            return Re_Str;
        }

        /// <summary>
        /// Replace String
        /// </summary>
        /// <param name="realaceValue"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static string ReplaceNoCase(string realaceValue, string oldValue, string newValue)  
        {
            return realaceValue.Replace(oldValue, newValue);
        }
    }
}