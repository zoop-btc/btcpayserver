using System.Threading;
using System.Threading.Tasks;
using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Plugins.Kratos.Data;

namespace BTCPayServer.Plugins.Kratos.Services;
public class KratosInitConfig : IStartupTask
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
    public KratosInitConfig(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var kratosConfig = await _settingsRepository.GetSettingAsync<KratosConf>();

        if (kratosConfig == null)
        {
            await _settingsRepository.UpdateSetting<KratosConf>(DefaultKratosConf());
        }
    }

}