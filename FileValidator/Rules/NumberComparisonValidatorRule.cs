using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FileValidator.Rules
{
    /// <summary>
    /// Partial abstract class for checking if a number value is GreaterThan, LessThan, or Equal to
    /// "expected_value"
    /// </summary>
    public abstract class NumberComparisonValidatorRule : BaseValidatorRule
    {
        protected enum ComparisonOperator
        {
            UNKNOWN = 0,
            GREATER_THAN,
            LESS_THAN,
            EQUAL,
        }

        [JsonProperty("comparison", Required = Required.Always)]
        [JsonConverter(typeof(StringEnumConverter))]
        private ComparisonOperator _comparisonOperator = ComparisonOperator.UNKNOWN;

        [JsonProperty("expected_value", Required = Required.Always)]
        private uint _expectedValue = 0;

        protected bool ResolveNumberValue(uint numberValue)
        {
            if(_comparisonOperator == ComparisonOperator.EQUAL)
            {
                return numberValue == _expectedValue;
            }
            else if(_comparisonOperator == ComparisonOperator.GREATER_THAN)
            {
                return numberValue > _expectedValue;
            }
            else if(_comparisonOperator == ComparisonOperator.LESS_THAN)
            {
                return numberValue < _expectedValue;
            }
            //TODO support greater_than_or_equal & less_than_or_equal?

            throw new RuleException($"Resolve called on unknown ComparisonOperator in rule: {Name}");
        }
    }
}
