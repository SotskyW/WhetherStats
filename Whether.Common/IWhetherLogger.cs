using System;
using System.Collections.Generic;
using System.Text;

namespace Whether.Common
{
    public interface IWhetherLogger
    {
        void Info(string message, params object[] args);

        void Info<T>(string message, params object[] args);

        void Debug(string message, params object[] args);

        void Debug<T>(string message, params object[] args);

        void Warn(string message, params object[] args);

        void Warn<T>(string message, params object[] args);

        void Error(string message, params object[] args);

        void Error<T>(string message, params object[] args);

        void Error(Exception exception, string message = null, params object[] args);

        void Error<T>(Exception exception, string message = null, params object[] args);

        void Fatal(string message, params object[] args);

        void Fatal<T>(string message, params object[] args);

        void Fatal(Exception exception, string message = null, params object[] args);

        void Fatal<T>(Exception exception, string message = null, params object[] args);
    }
}
