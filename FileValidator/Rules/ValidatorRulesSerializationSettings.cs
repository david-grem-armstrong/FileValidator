using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FileValidator.Rules
{
    public static class ValidatorRulesSerializationSettings
    {
        /// <summary>
        /// A list of the name of the ValidatorRule and the type of that same ValidatorRule.
        /// (Used to send to the JsonSerializerSettings to property handle SubTypes)
        /// 
        /// NOTE: If you want to support more ValidatorRules, you just need to inherit from <see cref="BaseValidatorRule"/>
        /// and add an entry here for the Json to parse.
        /// </summary>
        private static IReadOnlyDictionary<string, Type> _ruleTypeToRuleClassTypeMapping = new Dictionary<string, Type>()
        {
            { nameof(OrValidatorRule), typeof(OrValidatorRule) },
            { nameof(AndValidatorRule), typeof(AndValidatorRule) },
            { nameof(NotValidatorRule), typeof(NotValidatorRule) },
            { nameof(AlwaysFailValidatorRule), typeof(AlwaysFailValidatorRule) },
            { nameof(FileExtentsionMatchValidatorRule), typeof(FileExtentsionMatchValidatorRule) },
            { nameof(FileNameMatchValidatorRule), typeof(FileNameMatchValidatorRule) },
            { nameof(TextureSizeValidatorRule), typeof(TextureSizeValidatorRule) }
        };

        private static JsonSerializerSettings _settings = null;

        /// <summary>
        /// Static singleton function to get the JsonSerializerSettings for parsing ValidatorRule JSON objects
        /// </summary>
        /// <returns></returns>
        public static JsonSerializerSettings GetSettings()
        {
            if (_settings == null)
            {
                _settings = new JsonSerializerSettings();

                JsonSubtypesConverterBuilder builder = JsonSubtypesConverterBuilder.Of(typeof(BaseValidatorRule), BaseValidatorRule.RULE_TYPE_KEY);
                foreach(var ruleTypeToRuleCalssTypePair in _ruleTypeToRuleClassTypeMapping)
                {
                    string ruleTypeString = ruleTypeToRuleCalssTypePair.Key;
                    Type ruleClassType = ruleTypeToRuleCalssTypePair.Value;
                    builder = builder.RegisterSubtype(ruleClassType, ruleTypeString);
                }
                
                _settings.Converters.Add(builder.SerializeDiscriminatorProperty().Build());
            }

            return _settings;
        }
    }
}
