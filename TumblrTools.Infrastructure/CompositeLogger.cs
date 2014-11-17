namespace TumblrTools.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TumblrTools.Generic;

    public class CompositeLogger : ILogger
    {
        private readonly IEnumerable<ILogger> loggers;

        public CompositeLogger(params ILogger[] loggers)
        {
            this.loggers = loggers;
        }

        public CompositeLogger(IEnumerable<ILogger> loggers)
        {
            this.loggers = loggers;
        }

        public void Dispose()
        {
            foreach (var logger in this.loggers)
            {
                logger.Dispose();
            }
        }

        public string Name
        {
            get { return string.Join("|", this.loggers.Select(logger => logger.Name)); }
        }

        public void Trace(string message)
        {
            foreach (var logger in this.loggers)
            {
                logger.Trace(message);
            }
        }

        public void Trace(string message, params object[] args)
        {
            foreach (var logger in this.loggers)
            {
                logger.Trace(message, args);
            }
        }

        public void Trace(string message, Exception exception)
        {
            foreach (var logger in this.loggers)
            {
                logger.Trace(message, exception);
            }
        }

        public void Debug(string message)
        {
            foreach (var logger in this.loggers)
            {
                logger.Debug(message);
            }
        }

        public void Debug(string message, params object[] args)
        {
            foreach (var logger in this.loggers)
            {
                logger.Debug(message, args);
            }
        }

        public void Debug(string message, Exception exception)
        {
            foreach (var logger in this.loggers)
            {
                logger.Debug(message, exception);
            }
        }

        public void Info(string message)
        {
            foreach (var logger in this.loggers)
            {
                logger.Info(message);
            }
        }

        public void Info(string message, params object[] args)
        {
            foreach (var logger in this.loggers)
            {
                logger.Info(message, args);
            }
        }

        public void Info(string message, Exception exception)
        {
            foreach (var logger in this.loggers)
            {
                logger.Info(message, exception);
            }
        }

        public void Warn(string message)
        {
            foreach (var logger in this.loggers)
            {
                logger.Warn(message);
            }
        }

        public void Warn(string message, params object[] args)
        {
            foreach (var logger in this.loggers)
            {
                logger.Warn(message, args);
            }
        }

        public void Warn(string message, Exception exception)
        {
            foreach (var logger in this.loggers)
            {
                logger.Warn(message, exception);
            }
        }

        public void Error(string message)
        {
            foreach (var logger in this.loggers)
            {
                logger.Error(message);
            }
        }

        public void Error(string message, params object[] args)
        {
            foreach (var logger in this.loggers)
            {
                logger.Error(message, args);
            }
        }

        public void Error(string message, Exception exception)
        {
            foreach (var logger in this.loggers)
            {
                logger.Error(message, exception);
            }
        }

        public void Fatal(string message)
        {
            foreach (var logger in this.loggers)
            {
                logger.Fatal(message);
            }
        }

        public void Fatal(string message, params object[] args)
        {
            foreach (var logger in this.loggers)
            {
                logger.Fatal(message, args);
            }
        }

        public void Fatal(string message, Exception exception)
        {
            foreach (var logger in this.loggers)
            {
                logger.Fatal(message, exception);
            }
        }
    }
}
