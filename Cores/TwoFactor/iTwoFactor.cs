using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Playngo.Modules.Authentication.TwoFactor
{
    /// <summary>
    /// Two Factor Interface
    /// </summary>
    public interface iTwoFactor
    {
        /// <summary>
        /// Validate PIN
        /// </summary>
        /// <param name="AccountSecretKey">Secret Key</param>
        /// <param name="PinCode">Pin Code</param>
        /// <returns></returns>
        bool ValidatePIN(string AccountSecretKey, string PinCode);


        /// <summary>
        /// Generate a setup code for a Google Authenticator user to scan.
        /// </summary>
        /// <param name="issuer">Issuer ID (the name of the system, i.e. 'MyApp')</param>
        /// <param name="accountTitleNoSpaces">Account Title (no spaces)</param>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <param name="qrCodeWidth">QR Code Width</param>
        /// <param name="qrCodeHeight">QR Code Height</param>
        /// <returns>SetupCode object</returns>
        SetupCode GenerateSetupCode(string issuer, string accountTitleNoSpaces, string accountSecretKey, int qrCodeWidth, int qrCodeHeight);
    }
}