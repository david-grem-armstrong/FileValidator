using FileValidator.Config;
using FileValidator.FileProperties;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FileValidator.Rules
{
    /// <summary>
    /// Logic "OR" rule, that checks a list of sub-rules and returns "true" at the first passed "Resolve"
    /// </summary>
    public class OrValidatorRule : BaseValidatorRule
    {
        [JsonProperty("rules", Required = Required.Always)]
        [JsonConverter(typeof(ValidatorRuleListConverter))]
        private List<BaseValidatorRule> _rules = null;
        public override bool Resolve(IFileProperties fileProperties)
        {
            if (_rules == null)
            {
                throw new RuleException($"Resolve called when {nameof(_rules)} is null");
            }

            int count = _rules.Count;
            if (count == 0)
            {
                throw new RuleException($"Resolve called on empty {nameof(OrValidatorRule)}: {Name}");
            }

            for (int i = 0; i < count; ++i)
            {
                if (_rules[i].Resolve(fileProperties))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
