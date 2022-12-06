using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BTCPayServer.Plugins.Kratos.Services;

namespace BTCPayServer.Plugins.Kratos.Validation
{
    /// <summary>
    /// Validate the Kratos endpoint. Checks for Schemas with Email Trait.
    /// </summary>
    public class KratosPublicAttribute : ValidationAttribute
    {
        public KratosPublicAttribute()
        {
            ErrorMessage = ErrorMessageConst;
        }
        public const string ErrorMessageConst = "Could not verify Kratos Public Endpoint.";
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var str = value as string;
            try
            {
                var kratosservice = (KratosService)validationContext.GetService(typeof(KratosService));
                var schemas = kratosservice.GetValidSchemes(str).Result;

                if (schemas.Any())
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("No Schemas with Email Trait found for " + str);
                }
            }
            catch (AggregateException e){
                return new ValidationResult(string.Join(" ", e.InnerExceptions.Select(ex => ex.Message)));
            }
            catch (Exception e)
            {
                return new ValidationResult(e.Message);
            }
        }
    }
}
