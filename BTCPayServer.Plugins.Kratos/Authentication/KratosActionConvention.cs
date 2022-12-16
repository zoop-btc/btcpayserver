using BTCPayServer.Abstractions.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace BTCPayServer.Plugins.Kratos.Auth;

public class KratosActionConvention : IActionModelConvention
{
    public void Apply(ActionModel action)
    {
        // Console.WriteLine($"Controller -> {action.Controller.ControllerName} Action -> {action.ActionName}:");
        foreach (object attribute in action.Attributes)
        {
            if (attribute is not null)
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
                        // Console.WriteLine($"Schemes: {authattribute.AuthenticationSchemes}");
                    }
                }
            }
        }
    }
}