using System;
using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Abstractions.Models;
using BTCPayServer.Abstractions.Services;
using BTCPayServer.Plugins.Kratos.Auth;
using BTCPayServer.Plugins.Kratos.Data;
using BTCPayServer.Plugins.Kratos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace BTCPayServer.Plugins.Kratos
{
    public class KratosPlugin : BaseBTCPayServerPlugin
    {
        public override string Identifier { get; } = "BTCPayServer.Plugins.Kratos";
        public override string Name { get; } = "Kratos";
        public override string Description { get; } = "Add Kratos Authentication Middleware.";

        public override void Execute(IServiceCollection services)
        {

            services.AddHostedService<KratosInitConfig>();
            // services.AddSingleton(container =>
            // {
            //     var settingsrepo = container.GetRequiredService<ISettingsRepository>();
            //     var kratosconfig = settingsrepo.GetSettingAsync<KratosConf>().Result;
            //     if (kratosconfig == null)
            //     { throw new ArgumentException("Parameter cannot be null", nameof(kratosconfig)); }

            //     return new KratosService(kratosconfig.KratosPublic);
            // });
            // services.AddHostedService<KratosService>();
            services.AddSingleton<KratosService>();
            services.AddSingleton<IUIExtension>(new UIExtension("KratosExtensionNavExtension", "header-nav"));

            services.AddAuthentication()
            .AddScheme<KratosAuthenticationOptions, KratosUIAuthenticationHandler>("Kratos.UI", options => { })
            .AddScheme<KratosAuthenticationOptions, KratosAPIAuthenticationHandler>("Kratos.API", options => { });

            services.Configure<MvcOptions>(options =>
            {
                options.Conventions.Add(new KratosControllerConvention());
                options.Conventions.Add(new KratosActionConvention());
            });



        }
    }
}
