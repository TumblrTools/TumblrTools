namespace TumblrTools.Viewer.Controllers
{
    using System.Web.Mvc;
    using TumblrTools.Generic;
    using TumblrTools.Infrastructure.Mongo;

    public abstract class BaseController : Controller
    {
        public IConfiguration Configuration { get; set; }
        protected ILogger Logger { get; private set; }

        protected BaseController(IConfiguration configuration, ILogger logger)
        {
            this.Configuration = configuration;
            this.Logger = logger;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.Logger.Info("{0} {1}", this.Request.HttpMethod, filterContext.ActionDescriptor.ActionName);
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonCamelCasedResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        protected RepositoryBuilder GetRepositoryBuilder()
        {
            return new RepositoryBuilder(this.Configuration.ConnectionString, this.Configuration.TableName);
        }
    }
}