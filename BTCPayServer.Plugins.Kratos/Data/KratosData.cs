using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BTCPayServer.Plugins.Kratos.Validation;

namespace BTCPayServer.Plugins.Kratos.Data
{

    public class KratosConf
    {
        [Display(Name = "Enable Kratos Authentication")]
        public bool KratosEnabled { get; set; }

        [Display(Name = "Public Kratos Endpoint. Please use the URI without path like so 'https://your.kratos.domain' ")]
        [KratosPublicAttribute]
        public string KratosPublic { get; set; }

    }

}
