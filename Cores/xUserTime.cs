using System;
using System.Collections.Generic;
using System.Web;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Common.Utilities;

namespace Playngo.Modules.Authentication
{
    /// <summary>
    /// extend user time
    /// </summary>
    public class xUserTime //: UserTime
    {

        public static DateTime LocalTime()
        {
            return LocalTime(UtcTime());
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Convert utc time in User's timezone
        /// </summary>
        /// <param name="utcTime">Utc time to convert</param>
        /// -----------------------------------------------------------------------------       
        public static DateTime LocalTime(DateTime utcTime)
        {
            var PortalSettings = PortalController.Instance.GetCurrentPortalSettings();
            if (PortalSettings != null)
            {
                if (PortalSettings.UserId > Null.NullInteger)
                {
                    return TimeZoneInfo.ConvertTime(utcTime, TimeZoneInfo.Utc, PortalSettings.UserInfo.Profile.PreferredTimeZone);
                }
                else
                {
                    return TimeZoneInfo.ConvertTime(utcTime, TimeZoneInfo.Utc, PortalController.Instance.GetCurrentPortalSettings().TimeZone);
                }
            }
            return TimeZoneInfo.ConvertTime(utcTime, TimeZoneInfo.Utc, TimeZoneInfo.Local);

        }

        /// <summary>
        /// utc time by database
        /// </summary>
        /// <returns></returns>
        public static DateTime UtcTime()
        {
            return DateUtils.GetDatabaseTime();
        }


        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Convert utc time in User's timezone
        /// </summary>
        /// <param name="utcTime">Utc time to convert</param>
        /// -----------------------------------------------------------------------------       
        public static DateTime ServerTime(DateTime localTime)
        {
            var PortalSettings = PortalController.Instance.GetCurrentPortalSettings();
            if (PortalSettings != null)
            {
                if (PortalSettings.UserId > Null.NullInteger)
                {
                    return TimeZoneInfo.ConvertTimeToUtc(localTime, PortalSettings.UserInfo.Profile.PreferredTimeZone);
                }
                else
                {
                    return TimeZoneInfo.ConvertTimeToUtc(localTime, PortalController.Instance.GetCurrentPortalSettings().TimeZone);
                }

            }
            return TimeZoneInfo.ConvertTimeToUtc(localTime, TimeZoneInfo.Local);

        }

        public static DateTime ServerTime()
        {
            return ServerTime(DateTime.Now);
        }


    }
}