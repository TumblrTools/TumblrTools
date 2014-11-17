namespace TumblrSharp2.Clients
{
    using System;

    public class CallingApiMethodEventArgs : EventArgs
    {
        public CallingApiMethodEventArgs(string apiUrl)
        {
            this.ApiUrl = apiUrl;
        }
        public string ApiUrl { get; private set; }
    }
}