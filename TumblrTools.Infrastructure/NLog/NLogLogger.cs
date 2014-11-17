namespace TumblrTools.Infrastructure.NLog
{
    using System;
    using global::NLog;
    using TumblrTools.Generic;

    public class NLogLogger : ILogger
    {
        private readonly Logger logger;

        public NLogLogger(string name)
            : this(LogManager.GetLogger(name))
        {
        }

        public NLogLogger(Logger logger)
        {
            this.logger = logger;
        }

        public string Name
        {
            get { return this.logger.Name; }
        }

        public void Trace(string message)
        {
            this.logger.Trace(message);
        }

        public void Trace(string message, params object[] args)
        {
            this.logger.Trace(message, args);
        }

        public void Trace(string message, Exception exception)
        {
            this.logger.Trace(message, exception);
        }

        public void Debug(string message)
        {
            this.logger.Debug(message);
        }

        public void Debug(string message, params object[] args)
        {
            this.logger.Debug(message, args);
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
            this.logger.Info(message, args);
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
            this.logger.Warn(message, args);
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
            this.logger.Error(message, args);
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
            this.logger.Fatal(message, args);
        }

        public void Fatal(string message, Exception exception)
        {
            this.logger.Fatal(message, exception);
        }

        public void Dispose()
        {
            
        }
    }
}
