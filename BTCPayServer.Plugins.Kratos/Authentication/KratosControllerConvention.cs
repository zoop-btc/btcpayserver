using BTCPayServer.Abstractions.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace BTCPayServer.Plugins.Kratos.Auth;

public class KratosControllerConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        // Console.WriteLine($"\n--> Controller {controller.ControllerName} :");
        foreach (object attribute in controller.Attributes)
        {
            if (attribute is AuthorizeAttribute authattribute)
            {
                // Console.WriteLine($"Policies: {authattribute.Policy}");
                if (authattribute.AuthenticationSchemes is not null)
                {
                    if (authattribute.AuthenticationSchemes.Contains(AuthenticationSchemes.Cookie))
                    {
                        authattribute.AuthenticationSchemes = authattribute.AuthenticationSchemes + ",Kratos.UI";
                    }
                    if (authattribute.AuthenticationSchemes.Contains(AuthenticationSchemes.Greenfield) ||
                    authattribute.AuthenticationSchemes.Contains(AuthenticationSchemes.GreenfieldAPIKeys) ||
                    authattribute.AuthenticationSchemes.Contains(AuthenticationSchemes.GreenfieldBasic))
                    {
                        authattribute.AuthenticationSchemes = authattribute.AuthenticationSchemes + ",Kratos.API";
                    }
                }
                // Console.WriteLine($"Schemes: {authattribute.AuthenticationSchemes}");
            }
        }
    }
}