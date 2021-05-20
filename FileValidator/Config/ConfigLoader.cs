using FileValidator.Rules;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileValidator.Config
{
    /// <summary>
    /// Class responsible for loading the config file
    /// </summary>
    class ConfigLoader
    {
        /// <summary>
        /// Async function to load and serialize a <see cref="ConfigFile"/> from disk
        /// </summary>
        /// <param name="configFilePath">the full file path to the <see cref="ConfigFile"/></param>
        /// <returns>A serialized <see cref="ConfigFile"/></returns>
        public async Task<ConfigFile> AsyncLoadConfigFile(string configFilePath)
        {
            if(string.IsNullOrWhiteSpace(configFilePath))
            {
                throw new ArgumentException("ConfigLoader.AsyncLoadConfigFile called with empty/null configFilePath");
            }

            if(!File.Exists(configFilePath))
            {
                throw new ConfigException($"ConfigLoader.AsyncLoadConfigFile cannot find configFile: '{configFilePath}'");
            }

            string configFileRaw = await File.ReadAllTextAsync(configFilePath);

            ConfigFile configFile = null;
            try
            {
                JsonSerializerSettings settings = ValidatorRulesSerializationSettings.GetSettings();
                configFile = JsonConvert.DeserializeObject<ConfigFile>(configFileRaw, settings);
            }
            catch
            {
                throw new ConfigException($"ConfigLoader.AsyncLoadConfigFile cannot deserialize '{configFilePath}'");
            }

            return configFile;
        }
    }
}
