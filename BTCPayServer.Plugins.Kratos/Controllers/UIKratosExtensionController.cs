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
        private readonly ISettingsRepository _settingsRepository;

        private readonly KratosService _kratosService;

        public UIKratosExtensionController(UserManager<ApplicationUser> userManager, ISettingsRepository settingsRepository, KratosService kratosService)
        {
            _userManager = userManager;
            _settingsRepository = settingsRepository;
            _kratosService = kratosService;
        }

        public async Task<IActionResult> Index()
        {
            return View(new KratosPluginPageViewModel()
            {
                KratosConfig = await _settingsRepository.GetSettingAsync<KratosConf>()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Index(KratosPluginPageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var config = model.KratosConfig;
                await _settingsRepository.UpdateSetting(config);
                _kratosService.RefreshKratosSettings();
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
        [Display(Name = "Kratos Configuration")]
        public KratosConf KratosConfig { get; set; }
    }
}
