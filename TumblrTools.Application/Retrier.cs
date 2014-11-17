namespace TumblrTools.Application
{
    using System;
    using System.Collections.Generic;
    using TumblrTools.Generic;

    /// <summary>
    /// Utilities for retrying a block of code a number of times.
    /// </summary>
    public static class Retrier
    {
        public static void Retry(Action action, int numberOfTimes, ILogger logger)
        {
            Retry(
                action,
                numberOfTimes,
                null,
                (ex, retryNumber) =>  logger.Warn("{0}. Retrying...", ex.Message) );
        }

        /// <summary>
        /// Retries the specified action the specified number of times.
        /// The condition for retry is either throwing an exception in the action,
        /// or failing the result check (if provided).
        /// When retrying, it executes a handler (if provided).
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="resultChecker">The result checker.</param>
        /// <param name="numberOfTimes">The number of times.</param>
        /// <param name="retryExceptionHandler">The retry exception handler.</param>
        /// <returns>
        /// Result provided by the action.
        /// </returns>
        /// <exception cref="AggregateException">If the action to be invoked fails for more than <see cref="numberOfTimes" />.</exception>
        public static void Retry(Action action, int numberOfTimes, Func<bool> resultChecker = null,
            Action<Exception, int> retryExceptionHandler = null)
        {
            List<Exception> exceptions = new List<Exception>();
            for (int i = 0; i < numberOfTimes; i++)
            {
                try
                {
                    action();
                    if (resultChecker == null || resultChecker())
                    {
                        return;
                    }

                    throw new ResultCheckException("The action result check has failed");
                }
                catch (Exception ex)
                {
                    if (i < numberOfTimes - 1)
                    {
                        if (retryExceptionHandler != null)
                        {
                            try
                            {
                                retryExceptionHandler(ex, i + 1);
                            }
                            // ReSharper disable once EmptyGeneralCatchClause
                            catch
                            {
                            }
                        }
                    }

                    exceptions.Add(ex);
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}