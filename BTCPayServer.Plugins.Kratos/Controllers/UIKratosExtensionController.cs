using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BTCPayServer.Abstractions.Constants;
using BTCPayServer.Data;
using BTCPayServer.Plugins.Kratos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Plugins.Kratos.Data;
using System.ComponentModel.DataAnnotations;

namespace BTCPayServer.Plugins.Kratos
{
    [Route("plugins/Kratos")]
    [Authorize(Policy = BTCPayServer.Client.Policies.CanModifyServerSettings,
               AuthenticationSchemes = AuthenticationSchemes.Cookie)]
    public class UIKratosExtensionController : Controller
    {
        protected readonly UserManager<ApplicationUser> _userManager;
        // private readonly KratosPluginService _KratosPluginService;
        private readonly ISettingsRepository _settingsRepository;

        public UIKratosExtensionController(UserManager<ApplicationUser> userManager, ISettingsRepository settingsRepository)
        {
            _userManager = userManager;
            // _KratosPluginService = KratosPluginService;
            _settingsRepository = settingsRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(new KratosPluginPageViewModel()
            {
                // UserData = await _KratosPluginService.Get(),
                KratosConfig = await _settingsRepository.GetSettingAsync<KratosConf>()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Index(KratosPluginPageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var config = model.KratosConfig;
                System.Console.WriteLine($"Update URL to {config.KratosPublic}");
                await _settingsRepository.UpdateSetting(config);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(model);
            }
        }
    }
    public class KratosPluginPageViewModel
    {
        // [Display(Name = "Data of the Users")]
        // public List<ApplicationUser> UserData { get; set; }

        [Display(Name = "Kratos Configuration")]
        public KratosConf KratosConfig { get; set; }
    }
}
