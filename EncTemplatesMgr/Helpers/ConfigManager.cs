using EllieMae.EMLite.ClientServer;
using EllieMae.Encompass.Automation;
using System.Reflection;

namespace EncTemplatesMgr.Helpers
{
    internal class ConfigManager
    {
        private static IConfigurationManager _configurationManager;

        /// <summary>
        /// EllieMae.EMLite.ClientServer.IConfigurationManager.
        /// </summary>
        public static IConfigurationManager ConfigurationManager
        {
            get
            {
                if (_configurationManager == null)
                {
                    var sessionInfo = EncompassApplication.Session.GetType().GetField("sessionObjects", BindingFlags.Instance | BindingFlags.NonPublic);
                    var sessionObj = (SessionObjects)sessionInfo.GetValue(EncompassApplication.Session);
                    _configurationManager = sessionObj.ConfigurationManager;
                }

                return _configurationManager;
            }
        }
    }
}
