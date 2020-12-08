using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Whether.Common;

namespace Whether.Classes
{
    public class SerilogLogger : IWhetherLogger
    {
        public void Info(string message, params object[] args)
        {
            Log.Information(message, args);
        }

        public void Info<T>(string message, params object[] args)
        {
            ForContext<T>().Information(message, args);
        }

        public void Debug(string message, params object[] args)
        {
            Log.Debug(message, args);
        }

        public void Debug<T>(string message, params object[] args)
        {
            ForContext<T>().Debug(message, args);
        }

        public void Warn(string message, params object[] args)
        {
            Log.Warning(message, args);
        }

        public void Warn<T>(string message, params object[] args)
        {
            ForContext<T>().Warning(message, args);
        }

        public void Error(string message, params object[] args)
        {
            Log.Error(message, args);
        }

        public void Error<T>(string message, params object[] args)
        {
            ForContext<T>().Error(message, args);
        }

        public void Error(Exception exception, string message = null, params object[] args)
        {
            Log.Error(exception, message ?? exception.Message, args);
        }

        public void Error<T>(Exception exception, string message = null, params object[] args)
        {
            ForContext<T>().Error(exception, message ?? exception.Message, args);
        }

        public void Fatal(string message, params object[] args)
        {
            Log.Fatal(message, args);
        }

        public void Fatal<T>(string message, params object[] args)
        {
            ForContext<T>().Fatal(message, args);
        }

        public void Fatal(Exception exception, string message = null, params object[] args)
        {
            Log.Fatal(exception, message ?? exception.Message, args);
        }

        public void Fatal<T>(Exception exception, string message = null, params object[] args)
        {
            ForContext<T>().Fatal(exception, message ?? exception.Message, args);
        }

        private ILogger ForContext<T>()
        {
            return Log.ForContext<T>();
        }
    }
}
