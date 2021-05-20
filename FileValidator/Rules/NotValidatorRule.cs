using FileValidator.FileProperties;
using Newtonsoft.Json;

namespace FileValidator.Rules
{
    /// <summary>
    /// Logic "NOT" rule to return the opposite "Resolve" of the sub-rule
    /// </summary>
    public class NotValidatorRule: BaseValidatorRule
    {
        [JsonProperty("rule", Required = Required.Always)]
        private BaseValidatorRule _rule = null;
        public override bool Resolve(IFileProperties fileProperties)
        {
            if (_rule == null)
            {
                throw new RuleException($"Resolve called when {nameof(_rule)} is null");
            }

            return !_rule.Resolve(fileProperties);
        }
    }
}
