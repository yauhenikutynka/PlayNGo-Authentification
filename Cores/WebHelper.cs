using System;
using System.Web;
using System.Collections.Generic;


namespace Playngo.Modules.Authentication
{
    /// <summary>
    /// Web Helper Class
    /// </summary>
    public static class WebHelper
    {
  
        /// <summary>
        /// Gets the string param.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="errorReturn">The error return.</param>
        /// <returns>The param value.</returns>
        public static string GetStringParam(System.Web.HttpRequest request, string paramName, string errorReturn, Boolean IsLostHTML = true)
        {
            string retStr = errorReturn;
            if (request.QueryString[paramName] != null)
            {
                // Html, js in filter parameters ||  filter XSS
                retStr = IsLostHTML ? Common.LostXSS(request.QueryString[paramName]) : request.QueryString[paramName];
            }

            //string retStr = request.QueryString[paramName];
            if (request.Form[paramName] != null)
            {
                retStr = request.Form[paramName];
            }

            return retStr;
        }

        /// <summary>
        /// Gets the int param.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="errorReturn">The error return.</param>
        /// <returns>The param value.</returns>
        public static int GetIntParam(System.Web.HttpRequest request, string paramName, int errorReturn)
        {
            string retStr = request.Form[paramName];
            if (retStr == null)
            {
                retStr = request.QueryString[paramName];
            }
            if (retStr == null || retStr.Trim() == string.Empty)
            {
                return errorReturn;
            }
            try
            {
                return Convert.ToInt32(retStr);
            }
            catch
            {
                return errorReturn;
            }
        }

        /// <summary>
        /// Gets the date time param.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="errorReturn">The error return.</param>
        /// <returns>The param value.</returns>
        public static DateTime GetDateTimeParam(System.Web.HttpRequest request, string paramName, DateTime errorReturn)
        {
            string retStr = request.Form[paramName];
            if (retStr == null)
            {
                retStr = request.QueryString[paramName];
            }
            if (retStr == null || retStr.Trim() == string.Empty)
            {
                return errorReturn;
            }
            try
            {
                return Convert.ToDateTime(HttpUtility.UrlDecode(retStr));
            }
            catch
            {
                return errorReturn;
            }
        }

        /// <summary>
        /// Gets the Boolean param.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="errorReturn">The error return.</param>
        /// <returns>The param value.</returns>
        public static Boolean GetBooleanParam(System.Web.HttpRequest request, string paramName, Boolean errorReturn)
        {
            string retStr = request.Form[paramName];
            if (retStr == null)
            {
                retStr = request.QueryString[paramName];
            }
            if (retStr == null || retStr.Trim() == string.Empty)
            {
                return errorReturn;
            }
            try
            {
                return Convert.ToBoolean(retStr);
            }
            catch
            {
                return errorReturn;
            }
        }


        /// <summary>
        /// Strongs the typed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The strong typed instance.</returns>
        public static ObjectType StrongTyped<ObjectType>(object obj)
        {
            return (ObjectType)obj;
        }

        /// <summary>
        /// Toes the js single quote safe string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The formated str.</returns>
        public static string ToJsSingleQuoteSafeString(string str)
        {
            return str.Replace("'", "\\'");
        }

        /// <summary>
        /// Toes the js double quote safe string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The formated str.</returns>
        public static string ToJsDoubleQuoteSafeString(string str)
        {
            return str.Replace("\"", "\\\"");
        }

        /// <summary>
        /// Toes the VBS quote safe string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The formated str.</returns>
        public static string ToVbsQuoteSafeString(string str)
        {
            return str.Replace("\"", "\"\"");
        }

        /// <summary>
        /// Toes the SQL quote safe string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The formated str.</returns>
        public static string ToSqlQuoteSafeString(string str)
        {
            return str.Replace("'", "''");
        }

        /// <summary>
        /// Texts to HTML.
        /// </summary>
        /// <param name="txtStr">The TXT STR.</param>
        /// <returns>The formated str.</returns>
        public static string TextToHtml(string txtStr)
        {
            return txtStr.Replace(" ", "&nbsp;").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;").
                Replace("<", "&lt;").Replace(">", "&gt;").Replace("\r", "").Replace("\n", "<br />");
        }

        /// <summary>
        /// Split String "," To List
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static List<string> GetList(string strs)
        {
            List<string> list = new List<string>();
            if (!string.IsNullOrEmpty(strs))
            {
                strs = strs.Replace("，", ",");
                strs = strs.TrimStart(',').TrimEnd(',');
                string[] strArray = strs.Split(',');
                list.AddRange(strArray);
            }
            return list;
        }


        /// <summary>
        /// Split String "separator" To List
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static List<string> GetList(string strs, string separator)
        {
            List<string> list = new List<string>();
            if (!string.IsNullOrEmpty(strs))
            {
                string[] strArray = strs.Split(new string[]{ separator},  StringSplitOptions.None);
                list.AddRange(strArray);
            }
            return list;
        }




        /// <summary>
        /// Get Client IP 
        /// </summary>
        public static String UserHost
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    String str = (String)HttpContext.Current.Items["UserHostAddress"];
                    if (!String.IsNullOrEmpty(str)) return str;

                    if (HttpContext.Current.Request != null)
                    {
                        str = HttpContext.Current.Request.UserHostName;
                        if (String.IsNullOrEmpty(str)) str = HttpContext.Current.Request.UserHostAddress;
                        HttpContext.Current.Items["UserHostAddress"] = str;
                        return str;
                    }
                }
                return null;
            }
        }



    }
}