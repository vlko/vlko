using vlko.core.Setting;

namespace vlko.core.web
{
    public class WebSettings
    {

        /// <summary>
        /// Minimum length of password.
        /// </summary>
        public static readonly int MinRequiredPasswordLength = 8;

        /// <summary>
        /// Base url.
        /// </summary>
        public static readonly SettingValue<string> BaseUrl
            = new SettingValue<string>("BaseUrl", null, new ConfigSettingProvider());

        /// <summary>
        /// Static content version used to bypass static file caching.
        /// </summary>
        public static readonly SettingValue<string> StaticContentVersion
            = new SettingValue<string>("StaticContentVersion", "0", new ConfigSettingProvider());

        /// <summary>
        /// Google Analytics token.
        /// </summary>
        public static readonly SettingValue<string> GAToken
            = new SettingValue<string>("GAToken", null, new ConfigSettingProvider());

        /// <summary>
        /// Google Tag Manager token.
        /// </summary>
        public static readonly SettingValue<string> GTMToken
            = new SettingValue<string>("GTMToken", null, new ConfigSettingProvider());

        /// <summary>
        /// Additional bcc email for registration.
        /// </summary>
        public static readonly SettingValue<string> AdditionalBCCEmailForRegistration
            = new SettingValue<string>("AdditionalBCCEmailForRegistration", "bcc_email@bcc_email.bcc", new ConfigSettingProvider());
    }
}
