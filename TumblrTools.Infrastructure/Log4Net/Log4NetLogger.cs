namespace TumblrTools.Infrastructure.Log4Net
{
    using System;
    using log4net;
    using TumblrTools.Generic;

    public class Log4NetLogger : ILogger
    {
        private readonly ILog logger;
        private readonly string name;

        public Log4NetLogger(string name)
        {
            this.name = name;
            this.logger = LogManager.GetLogger(name);
        }


        public string Name
        {
            get { return this.name; }
        }

        public void Trace(string message)
        {
            this.logger.Debug(message);
        }

        public void Trace(string message, params object[] args)
        {
            this.logger.DebugFormat(message, args);
        }

        public void Trace(string message, Exception exception)
        {
            this.logger.Debug(message, exception);
        }

        public void Debug(string message)
        {
            this.logger.Debug(message);
        }

        public void Debug(string message, params object[] args)
        {
            this.logger.DebugFormat(message, args);
        }

        public void Debug(string message, Exception exception)
        {
            this.logger.Debug(message, exception);
        }

        public void Info(string message)
        {
            this.logger.Info(message);
        }

        public void Info(string message, params object[] args)
        {
            this.logger.InfoFormat(message, args);
        }

        public void Info(string message, Exception exception)
        {
            this.logger.Info(message, exception);
        }

        public void Warn(string message)
        {
            this.logger.Warn(message);
        }

        public void Warn(string message, params object[] args)
        {
            this.logger.WarnFormat(message, args);
        }

        public void Warn(string message, Exception exception)
        {
            this.logger.Warn(message, exception);
        }

        public void Error(string message)
        {
            this.logger.Error(message);
        }

        public void Error(string message, params object[] args)
        {
            this.logger.ErrorFormat(message, args);
        }

        public void Error(string message, Exception exception)
        {
            this.logger.Error(message, exception);
        }

        public void Fatal(string message)
        {
            this.logger.Fatal(message);
        }

        public void Fatal(string message, params object[] args)
        {
            this.logger.FatalFormat(message, args);
        }

        public void Fatal(string message, Exception exception)
        {
            this.logger.Fatal(message, exception);
        }

        public void Dispose()
        {
            this.logger.Logger.Repository.Shutdown();
        }
    }
}
