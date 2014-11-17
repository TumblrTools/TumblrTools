namespace TumblrTools.Viewer
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using TumblrTools.Generic;
    using TumblrTools.Infrastructure.AppConfig;
    using TumblrTools.Infrastructure.Log4Net;
    using TumblrTools.Viewer.Controllers;

    public class CustomDependencyResolver : IDependencyResolver
    {
        private readonly IDependencyResolver defaultResolver;
        private readonly ILogger logger;
        private readonly IConfiguration configuration;

        public CustomDependencyResolver(IDependencyResolver defaultResolver)
        {
            if (defaultResolver == null)
            {
                throw new ArgumentNullException("defaultResolver");
            }

            this.defaultResolver = defaultResolver;
            this.configuration = new AppSettingsConfiguration();
            this.logger = this.BuildLog();
        }

        private ILogger BuildLog()
        {
            ILogger builtLogger = new Log4NetLogger("TumblrViewer");
            log4net.Config.XmlConfigurator.Configure();

            builtLogger.Info("Log set");
            return builtLogger;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(GalleryController))
            {
                return new GalleryController(this.configuration, this.logger);
            }

            if (serviceType == typeof(DownloadController))
            {
                return new DownloadController(this.configuration, this.logger);
            }

            return this.defaultResolver.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.defaultResolver.GetServices(serviceType);
        }
    }
}