using System.ComponentModel.DataAnnotations;

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
            
            var result = KratosPublicValidator.IsKratosEndpoint(str);

            if (result == "ok")
                return ValidationResult.Success;

            return new ValidationResult(result);
        }
    }
}
