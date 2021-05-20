using FileValidator.FileProperties;
using Newtonsoft.Json;

namespace FileValidator.Rules
{
    /// <summary>
    /// Abstract base class for all rules
    /// </summary>
    public abstract class BaseValidatorRule
    {
        /// <summary>
        /// Key used to determine which derived class will be used to parse the JSON data
        /// </summary>
        public const string RULE_TYPE_KEY = "rule_type";

        /// <summary>
        /// the name of this rule (used for console output only)
        /// </summary>
        [JsonProperty("name", Required = Required.Always)]
        private string _name = "";
        public abstract bool Resolve(IFileProperties fileProperties);

        public string Name => _name;
    }
}
