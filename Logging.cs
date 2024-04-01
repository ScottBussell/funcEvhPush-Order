using System;
using System.Net;
using Serilog;
using Serilog.Events;

namespace funcEvhPush_Order
{
    internal static class LoggingRepository
    {
        #region Private Settings

        static readonly string _Host = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LogHost"))
            ? "func-evhPushOrder" : Environment.GetEnvironmentVariable("LogHost");
        static readonly string _Service = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LogService"))
            ? "test" : Environment.GetEnvironmentVariable("LogService");
        static readonly string _APIKey = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DD_API_KEY"))
            ? string.Empty : Environment.GetEnvironmentVariable("DD_API_KEY");
        static readonly string _Minimum = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LogMinLevel"))
            ? "verbose" : Environment.GetEnvironmentVariable("LogMinLevel");
        static readonly bool _LoggingEnabled = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("EnableLogging"))
            ? true : Convert.ToBoolean(Environment.GetEnvironmentVariable("EnableLogging"));

        #endregion

        public static void SLog(string sMessage, LogEventLevel logLevel, string[] sTagArray = null)
        {
            if (_LoggingEnabled)
            {
                // pass empty tag array if NULL passed
                if (sTagArray == null) { sTagArray = new string[] { }; }

                ServicePointManager.SecurityProtocol = (SecurityProtocolType)0xc00; // force TLS 1.2
                var log = new LoggerConfiguration();
                log.WriteTo.DatadogLogs(_APIKey, source: "csharp", host: _Host, service: _Service, tags: sTagArray);

                if (_Minimum.Equals("verbose")) log.MinimumLevel.Verbose();
                else if (_Minimum.Equals("debug")) log.MinimumLevel.Debug();
                else if (_Minimum.Equals("error")) log.MinimumLevel.Error();
                else if (_Minimum.Equals("fatal")) log.MinimumLevel.Fatal();
                else if (_Minimum.Equals("info")) log.MinimumLevel.Information();
                else if (_Minimum.Equals("warning")) log.MinimumLevel.Warning();
                else log.MinimumLevel.Verbose();

                using (var logger = log.CreateLogger())
                {
                    logger.Write(logLevel, DateTime.Now.ToString() + " " + logLevel.ToString() + ": " + sMessage);
                }
            }
        }

        public static void SLogError(string sFunctionName, string sMessage, string[] sTagArray = null)
        {
            if (_LoggingEnabled)
            {
                // pass empty tag array if NULL passed
                if (sTagArray == null) { sTagArray = new string[] { }; }

                ServicePointManager.SecurityProtocol = (SecurityProtocolType)0xc00; // force TLS 1.2
                var log = new LoggerConfiguration();
                log.WriteTo.DatadogLogs(_APIKey, source: "csharp", host: _Host, service: _Service, tags: sTagArray);

                if (_Minimum.Equals("verbose")) log.MinimumLevel.Verbose();
                else if (_Minimum.Equals("debug")) log.MinimumLevel.Debug();
                else if (_Minimum.Equals("error")) log.MinimumLevel.Error();
                else if (_Minimum.Equals("fatal")) log.MinimumLevel.Fatal();
                else if (_Minimum.Equals("info")) log.MinimumLevel.Information();
                else if (_Minimum.Equals("warning")) log.MinimumLevel.Warning();
                else log.MinimumLevel.Verbose();

                using (var logger = log.CreateLogger())
                {
                    logger.Write(LogEventLevel.Error, string.Format("{0} Error in {1}: {2}", DateTime.Now.ToString(), sFunctionName, sMessage));
                }
            }
        }

        public static void SLogWarning(string sFunctionName, string sMessage, string[] sTagArray = null)
        {
            if (_LoggingEnabled)
            {
                // pass empty tag array if NULL passed
                if (sTagArray == null) { sTagArray = new string[] { }; }

                ServicePointManager.SecurityProtocol = (SecurityProtocolType)0xc00; // force TLS 1.2
                var log = new LoggerConfiguration();
                log.WriteTo.DatadogLogs(_APIKey, source: "csharp", host: _Host, service: _Service, tags: sTagArray);

                if (_Minimum.Equals("verbose")) log.MinimumLevel.Verbose();
                else if (_Minimum.Equals("debug")) log.MinimumLevel.Debug();
                else if (_Minimum.Equals("error")) log.MinimumLevel.Error();
                else if (_Minimum.Equals("fatal")) log.MinimumLevel.Fatal();
                else if (_Minimum.Equals("info")) log.MinimumLevel.Information();
                else if (_Minimum.Equals("warning")) log.MinimumLevel.Warning();
                else log.MinimumLevel.Verbose();

                using (var logger = log.CreateLogger())
                {
                    logger.Write(LogEventLevel.Warning, string.Format("{0} Warning in {1}: {2}", DateTime.Now.ToString(), sFunctionName, sMessage));
                }
            }
        }

        public static void SLogInfo(string sFunctionName, string sMessage, string[] sTagArray = null)
        {
            if (_LoggingEnabled)
            {
                // pass empty tag array if NULL passed
                if (sTagArray == null) { sTagArray = new string[] { }; }

                ServicePointManager.SecurityProtocol = (SecurityProtocolType)0xc00; // force TLS 1.2
                var log = new LoggerConfiguration();
                log.WriteTo.DatadogLogs(_APIKey, source: "csharp", host: _Host, service: _Service, tags: sTagArray);

                if (_Minimum.Equals("verbose")) log.MinimumLevel.Verbose();
                else if (_Minimum.Equals("debug")) log.MinimumLevel.Debug();
                else if (_Minimum.Equals("error")) log.MinimumLevel.Error();
                else if (_Minimum.Equals("fatal")) log.MinimumLevel.Fatal();
                else if (_Minimum.Equals("info")) log.MinimumLevel.Information();
                else if (_Minimum.Equals("warning")) log.MinimumLevel.Warning();
                else log.MinimumLevel.Verbose();

                using (var logger = log.CreateLogger())
                {
                    logger.Write(LogEventLevel.Information, string.Format("{0} Info from {1}: {2}", DateTime.Now.ToString(), sFunctionName, sMessage));
                }
            }
        }
    }
}
