using Newtonsoft.Json;
using System.Collections.Generic;

namespace FileValidator.Rules
{
    /// <summary>
    /// Partial bstract class that can resolve if a given string exists in the "possible_matches" list
    /// </summary>
    public abstract class StringOneOfValidatorRule : BaseValidatorRule
    {
        [JsonProperty("possible_matches", Required = Required.Always)]
        protected List<string> _possibleStringMatches = null;
        protected virtual bool ResolveStringMatch(string stringToCheck)
        {
            if (_possibleStringMatches == null)
            {
                throw new RuleException($"Resolve called when {nameof(_possibleStringMatches)} is null on rule: {Name}");
            }

            return _possibleStringMatches.Contains(stringToCheck);
        }
    }
}
