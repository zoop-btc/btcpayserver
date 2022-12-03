using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BTCPayServer.Plugins.Kratos.Services;
using Microsoft.AspNetCore.Identity;
using BTCPayServer.Data;
using BTCPayServer.Plugins.Kratos.Models;
using BTCPayServer.Client;
using System.Linq;
using PasswordGenerator;

namespace BTCPayServer.Plugins.Kratos.Auth
{

    public class KratosAPIAuthenticationHandler : AuthenticationHandler<KratosAuthenticationOptions>
    {
        private readonly KratosService _kratosService;
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<IdentityOptions> _identityOptions;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        readonly string _sessionCookieName = "ory_kratos_session";

        public KratosAPIAuthenticationHandler(
            IOptionsMonitor<IdentityOptions> identityOptions,
            IOptionsMonitor<KratosAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            KratosService kratosService
        )
            : base(options, logger, encoder, clock)
        {
            _kratosService = kratosService;
            _identityOptions = identityOptions;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger.CreateLogger<KratosAPIAuthenticationHandler>();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                // Check if Cookie is set
                if (Request.Cookies.ContainsKey(_sessionCookieName))
                {
                    var cookie = Request.Cookies[_sessionCookieName];
                    var id = await _kratosService.GetUserIdByCookie(_sessionCookieName, cookie);
                    return await AuthenticateKratosUser(id);
                }

                // Check if Header is set
                if (Request.Headers.ContainsKey("Authorization"))
                {
                    var token = Request.Headers["Authorization"];
                    var id = await _kratosService.GetUserIdByToken(token);
                    return await AuthenticateKratosUser(id);
                }

                // If neither Cookie nor Authorization header was set, the request can't be authenticated.
                return AuthenticateResult.NoResult();
            }
            catch (Exception ex)
            {
                // If an error occurs while trying to validate the token, the Authentication request fails.
                return AuthenticateResult.Fail(ex.Message);
            }
        }

        private async Task<AuthenticateResult> AuthenticateKratosUser(KratosIdentity kratosUser)
        {
            //We get the user if he already exists
            var user = (await _userManager.FindByIdAsync(kratosUser.Id)) ?? await _userManager.FindByEmailAsync(kratosUser.Traits.Email);

            //Should user not exist we create a new one
            if (user is null)
            {
                var newuser = new ApplicationUser
                {
                    UserName = kratosUser.Traits.Email,
                    Email = kratosUser.Traits.Email,
                    RequiresEmailConfirmation = false,
                    Created = DateTimeOffset.UtcNow
                };

                //We use the GUID from Kratos here. That way we can correlate the users even if they change the email address.
                newuser.Id = kratosUser.Id;
                
                //Generate a random password
                var password = new Password(includeLowercase: true, includeUppercase: true, includeNumeric: true, includeSpecial: false, passwordLength: 32).Next();

                var result = await _userManager.CreateAsync(newuser, password);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Registered new user: {kratosUser.Traits.Email} with ID {kratosUser.Id}");
                    user = await _userManager.FindByIdAsync(kratosUser.Id);

                }
                else
                {
                    _logger.LogWarning($"Could not create user {kratosUser} because of an error.");
                    return AuthenticateResult.NoResult();
                }
            }

            //We create Claims same as basic auth (unrestricted) so it gets processed correctly by the Greenfield Authorizationhandler
            var claims = new List<Claim>()
            {
                new Claim(_identityOptions.CurrentValue.ClaimsIdentity.UserIdClaimType, user.Id),
                new Claim("APIKey.Permission", Permission.Create(Policies.Unrestricted).ToString())
            };
            claims.AddRange((await _userManager.GetRolesAsync(user)).Select(s => new Claim(_identityOptions.CurrentValue.ClaimsIdentity.RoleClaimType, s)));

            return AuthenticateResult.Success(new AuthenticationTicket(
                    new ClaimsPrincipal(new ClaimsIdentity(claims, "Greenfield")), "Greenfield"));
        }
    }
}