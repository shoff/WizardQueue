using System;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;

namespace Wizard.Shared
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseNLog(this IHostBuilder builder)
        {
            return builder.UseNLog(null);
        }
        public static IHostBuilder UseNLog(this IHostBuilder builder, NLogAspNetCoreOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            options = (options ?? NLogAspNetCoreOptions.Default);
            builder.ConfigureServices(delegate (IServiceCollection services)
            {
                LogManager.AddHiddenAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
                services.AddSingleton((Func<IServiceProvider, ILoggerProvider>)delegate
                {
                    return new NLogLoggerProvider(options);
                });
                if (options.RegisterHttpContextAccessor)
                {
                    services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                }
            });
            return builder;
        }
    }
}