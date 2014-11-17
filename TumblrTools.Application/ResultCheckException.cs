namespace TumblrTools.Application
{
    using System;

    public class ResultCheckException : Exception
    {
        public ResultCheckException(string message)
            : base(message)
        {
        }
    }
}