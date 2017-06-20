using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using BachelorThesis.Dialogs;
using BachelorThesis.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using QnAMakerDialog;

namespace BachelorThesis
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();
            var config = GlobalConfiguration.Configuration;

            RegisterDependencies(builder);

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly())
                .PropertiesAutowired();

            builder.RegisterWebApiFilterProvider(config);

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        private void RegisterDependencies(ContainerBuilder builder)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "bin\\";
            var types = Directory.EnumerateFiles(path, "*.dll", SearchOption.TopDirectoryOnly)
                    .Where(filePath => Path.GetFileName(filePath).StartsWith("BachelorThesis"))
                    .Select(Assembly.LoadFrom)
                    .ToArray();

            var translatorAuthentication = new AuthenticationService(ConfigurationManager.AppSettings["TranslatorTextApiKey"]);
            var translatorService = new TranslatorService(translatorAuthentication);
            builder.RegisterInstance(translatorService)
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();
            builder.RegisterInstance(new TextAnalyticsService())
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();
            builder.RegisterType<LoggingService>()
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();

            builder.RegisterInstance(new LuisService(new LuisModelAttribute(
                    ConfigurationManager.AppSettings["LuisModelId"],
                    ConfigurationManager.AppSettings["LuisSubscriptionKey"])))
                .As<ILuisService>();

            builder.RegisterType<FeedbackDialog>()
                .AsSelf()
                .PropertiesAutowired();
            builder.
                Register(c => new FormDialog<FeedbackDialog>(c.Resolve<FeedbackDialog>(), FeedbackDialog.BuildForm, FormOptions.PromptInStart))
                .AsSelf();

            builder.RegisterAssemblyTypes(types)
                .Where(t =>
                    t.IsAssignableTo<IDialog<object>>() ||
                    t.IsAssignableTo<IDialog<bool>>() ||
                    t.IsAssignableTo<LuisDialog<bool>>() ||
                    t.IsAssignableTo<QnAMakerDialog<object>>())
                .AsImplementedInterfaces()
                .AsSelf()
                .PropertiesAutowired();
        }
    }
}
