//using Serilog.Core;

namespace EncTemplatesMgr.Helpers
{
    public class Log
    {
        //public static Logger Logger
        //{
        //    get
        //    { 
        //        if (Logger == null)
        //        {
        //            Logger = new LoggerConfiguration().WriteTo.File("logs/EncTemplatesMgr.log", rollingInterval: RollingInterval.Day).CreateLogger();
        //        }

        //        return Logger;
        //    }
        //    private set { Logger = value; }
        //}

        // ToDo: Reimpliment Serilog, this is a 'temporary' workaround because Encompass doesn't have serilog reference.
        private static Logger _logger;

        public static Logger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new Logger();
                }

                return _logger;
            }
            set { Logger = value; }
        }
    }

    public class Logger
    {
        public void Error(string message, System.Exception exception = null)
        {
            System.Windows.MessageBox.Show(string.Concat(message, ", Exception: ", exception.ToString() ));
        }

        public void Verbose(string message)
        {
            return;
        }
    }
}
