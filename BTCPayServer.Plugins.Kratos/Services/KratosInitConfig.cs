
using System.Threading;
using System.Threading.Tasks;
using BTCPayServer.Abstractions.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using BTCPayServer.Plugins.Kratos.Data;
using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace BTCPayServer.Plugins.Kratos.Services;
public class KratosInitConfig : IHostedService
{
    public static KratosConf DefaultKratosConf()
    {
        return new KratosConf
        {
            KratosEnabled = true,
            KratosPublic = "http://localhost:4433"
        };
    }

    private readonly ISettingsRepository _settingsRepository;
    private readonly IActionDescriptorCollectionProvider _descriptorCollectionProvider;

    public KratosInitConfig(ISettingsRepository settingsRepository, IActionDescriptorCollectionProvider descriptorCollectionProvider)
    {
        _settingsRepository = settingsRepository;
        _descriptorCollectionProvider = descriptorCollectionProvider;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var kratosConfig = await _settingsRepository.GetSettingAsync<KratosConf>();

        if (kratosConfig == null)
        {
            await _settingsRepository.UpdateSetting<KratosConf>(DefaultKratosConf());
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}