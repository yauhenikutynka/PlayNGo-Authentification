﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Playngo.Modules.Authentication.TwoFactor
{
    /// <summary>
    /// Two Factor Setup Code
    /// </summary>
    public class SetupCode
    {
        public string Account { get; internal set; }
        public string AccountSecretKey { get; internal set; }
        public string ManualEntryKey { get; internal set; }
        public string QrCodeSetupImageUrl { get; internal set; }
    }
}
