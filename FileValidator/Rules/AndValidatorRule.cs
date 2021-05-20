using FileValidator.Config;
using FileValidator.FileProperties;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FileValidator.Rules
{
    /// <summary>
    /// Logic "AND" rule, that checks a list of sub-rules and returns "false" at the first failed "Resolve"
    /// </summary>
    public class AndValidatorRule : BaseValidatorRule
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
                throw new RuleException($"Resolve called on empty {nameof(AndValidatorRule)}: {Name}");
            }

            for (int i = 0; i < count; ++i)
            {
                if(!_rules[i].Resolve(fileProperties))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
