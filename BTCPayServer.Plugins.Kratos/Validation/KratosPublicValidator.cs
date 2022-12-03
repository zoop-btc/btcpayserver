#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace BTCPayServer.Plugins.Kratos
{
    /// <summary>
    /// Check connectivity to Kratos & email trait in identity schema
    /// </summary>
    public class KratosPublicValidator
    {
        static KratosPublicValidator() { }
        public static string IsKratosEndpoint(string? str)
        {
            List<string> validschemas = new List<string>();

            var _client = new HttpClient();
            try
            {
                string responseBody = _client.GetStringAsync(str + "/schemas").Result;

                var schemesarray = JArray.Parse(responseBody);

                foreach (JObject entry in schemesarray)
                {
                    var schemaid = (string?)entry["id"];

                    string schemajson = _client.GetStringAsync(str + "/schemas/" + schemaid).Result;

                    var schema = JObject.Parse(schemajson);

                    var email = schema?["properties"]?["traits"]?["properties"]?["email"];

                    if (email != null && schemaid != null)
                    {
                        validschemas.Add(schemaid);
                    }
                }
                if (!validschemas.Any()){
                    return "No Schemas with Email Trait found for " + str;
                }

            }
            catch (Exception e)
            {
                return e.Message;
            }

            Console.WriteLine("Following schemes have an email trait: " + String.Join(" ", validschemas.ToArray()));
            return "ok";
        }

    }
}
