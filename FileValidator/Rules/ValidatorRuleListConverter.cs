using FileValidator.Rules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace FileValidator.Config
{
    /// <summary>
    /// JsonConverter for List<BaseValidatorRule> (which handles derived types too)
    /// </summary>
    class ValidatorRuleListConverter : JsonConverter<List<BaseValidatorRule>>
    {
        public override List<BaseValidatorRule> ReadJson(JsonReader reader, Type objectType, List<BaseValidatorRule> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JsonSerializerSettings settings = ValidatorRulesSerializationSettings.GetSettings();
            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray arrayEntry = JArray.Load(reader);

                int count = arrayEntry.Count;
                List<BaseValidatorRule> ruleList = new List<BaseValidatorRule>(count);
                foreach (JObject item in arrayEntry.Children())
                {
                    string jsonString = item.ToString();
                    BaseValidatorRule rule = JsonConvert.DeserializeObject<BaseValidatorRule>(jsonString, settings);
                    ruleList.Add(rule);
                }

                return ruleList;
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, List<BaseValidatorRule> value, JsonSerializer serializer)
        {
            throw new NotImplementedException("ValidatorRuleListConverter:WriteJson not supported");
        }
    }
}
