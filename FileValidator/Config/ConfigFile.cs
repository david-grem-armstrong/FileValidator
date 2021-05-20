using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FileValidator.Config
{
    /// <summary>
    /// Hard-type class that represents the entire config file when loaded into memory
    /// </summary>
    [JsonObject]
    class ConfigFile
    {
        [JsonProperty("search_all_sub_folders", Required = Required.Always)]
        private bool _searchAllSubFolders = false;

        [JsonProperty("validation_configs", Required = Required.Always)]
        private List<ValidationConfig> _validationConfigs = new List<ValidationConfig>();

        public bool SearchAllSubFolders => _searchAllSubFolders;

        public IReadOnlyList<ValidationConfig> ValidationConfigs => _validationConfigs;

        [OnError]
        internal void OnSerializationError(StreamingContext streamingContext, ErrorContext errorContext)
        {
            Console.WriteLine($"ConfigFile Json error: {errorContext.Error.Message}");
        }
    }
}
