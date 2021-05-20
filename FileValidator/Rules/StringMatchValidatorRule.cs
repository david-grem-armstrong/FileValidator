using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FileValidator.Rules
{
    /// <summary>
    /// Partial abstract class to check if a string StartsWith, EndsWith, or Contains
    /// a given string
    /// </summary>
    public abstract class StringMatchValidatorRule : StringOneOfValidatorRule
    {
        protected enum StringOperator
        {
            UNKNOWN = 0,
            STARTS_WITH,
            ENDS_WITH,
            CONTAINS,
            EQUALS
        }

        [JsonProperty("string_operator", Required = Required.Always)]
        [JsonConverter(typeof(StringEnumConverter))]
        private StringOperator _stringOperator = StringOperator.UNKNOWN;

        protected override bool ResolveStringMatch(string stringToCheck)
        {
            if (_possibleStringMatches == null)
            {
                throw new RuleException($"Resolve called when {nameof(_possibleStringMatches)} is null on rule: {Name}");
            }

            int count = _possibleStringMatches.Count;
            switch (_stringOperator)
            {
                case StringOperator.STARTS_WITH:
                    {
                        for (int i = 0; i < count; ++i)
                        {
                            if(stringToCheck.StartsWith(_possibleStringMatches[i]))
                            {
                                return true;
                            }
                        }

                        return false;
                    }
                case StringOperator.ENDS_WITH:
                    {
                        for (int i = 0; i < count; ++i)
                        {
                            if (stringToCheck.EndsWith(_possibleStringMatches[i]))
                            {
                                return true;
                            }
                        }

                        return false;
                    }
                case StringOperator.CONTAINS:
                    {
                        for (int i = 0; i < count; ++i)
                        {
                            if (stringToCheck.Contains(_possibleStringMatches[i]))
                            {
                                return true;
                            }
                        }

                        return false;
                    }
                case StringOperator.EQUALS:
                    {
                        for (int i = 0; i < count; ++i)
                        {
                            if (stringToCheck.Equals(_possibleStringMatches[i]))
                            {
                                return true;
                            }
                        }

                        return false;
                    }
            }

            throw new RuleException($"Resolve called on unknown StringOperator in rule: {Name}");
        }
    }
}
