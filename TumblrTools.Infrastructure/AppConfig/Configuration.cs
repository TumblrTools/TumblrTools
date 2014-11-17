namespace TumblrTools.Infrastructure.AppConfig
{
    using System.Configuration;
    using System.Dynamic;
    using TumblrTools.Generic;

    public class AppSettingsConfiguration : DynamicObject, IConfiguration
    {
        public string ConnectionString
        {
            get { return ConfigurationManager.AppSettings["ConnectionString"]; }
        }

        public string TableName
        {
            get { return ConfigurationManager.AppSettings["TableName"]; }
        }

        public string ConsumerKey
        {
            get { return ConfigurationManager.AppSettings["ConsumerKey"]; }
        }

        public string ConsumerSecret
        {
            get { return ConfigurationManager.AppSettings["ConsumerKey"]; }
        }

        public string PhotosDirectory
        {
            get { return ConfigurationManager.AppSettings["PhotosDirectory"]; }
        }
    }
}