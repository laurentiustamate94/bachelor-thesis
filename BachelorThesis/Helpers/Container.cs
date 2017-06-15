using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;

namespace BachelorThesis.Helpers
{
    public class Container
    {
        private Container()
        {

        }

        public static ILifetimeScope GetContainer()
        {
            var config = GlobalConfiguration.Configuration;
            var resolver = (AutofacWebApiDependencyResolver)config.DependencyResolver;

            return resolver.Container;
        }

        public static T Resolve<T>()
        {
            return GetContainer().Resolve<T>();
        }
    }
}