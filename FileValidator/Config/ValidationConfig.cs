using FileValidator.FileProperties;
using FileValidator.Rules;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FileValidator.Config
{
    /// <summary>
    /// A single entry in the <see cref="ConfigFile"/> that represents how a file should be filtered, 
    /// how it should be renamed/moved and what would be required for it to be valid
    /// </summary>
    public class ValidationConfig
    {
        /// <summary>
        /// The name of this configuration (used only for console output)
        /// </summary>
        [JsonProperty("name", Required = Required.Always)]
        private string _name = "";

        /// <summary>
        /// A list of rules that the file is tested against to see if this is the best-match
        /// <see cref="ValidationConfig"/> to run on that file.
        /// NOTE: the first <see cref="ValidationConfig"/> with the highest number of matches is chosen
        /// </summary>
        [JsonProperty("filter_rules", Required = Required.Always)]
        [JsonConverter(typeof(ValidatorRuleListConverter))]
        private List<BaseValidatorRule> _filterRules = null;

        [JsonProperty("name_seperator", Required = Required.Always)]
        private string _nameSeperator = "";

        /// <summary>
        /// The tokenized desired output path (folder) that this file *should* go to
        /// default to the root of the search directory
        /// </summary>
        [JsonProperty("output_path")]
        private string _outputPath = "./";

        /// <summary>
        /// The tokenized desired output filename that this file *should* have
        /// </summary>
        [JsonProperty("output_filename", Required = Required.Always)]
        private string _outputFileName = "";

        /// <summary>
        /// Flag for whether or not this file should automatically be moved to
        /// "output_path" and renamed to "output_file"
        /// </summary>
        [JsonProperty("auto_convert")]
        private bool _autoConvert = false;

        /// <summary>
        /// The rule that is required for the file to be "valid" according to this config.
        /// (if this field is missing, "valid" is assumed)
        /// NOTE: because <see cref="AndValidatorRule"/>, and <see cref="OrValidatorRule"/>
        /// exist, complex rules can be used here.
        /// </summary>
        [JsonProperty("validation_rule")]
        private BaseValidatorRule _validationRule = null;

        ///Regex pattern to search for tokens in "output_path" or "output_file"
        private Regex tokenRegexPattern = new Regex(@"\{([^}]+)\}");

        public string Name => _name;

        public uint NumberOfMatchingFilterRules(IFileProperties fileProperties)
        {
            uint matchingCount = 0;
            int filterRulesCount = (_filterRules != null) ? _filterRules.Count : 0;
            for(int i = 0; i < filterRulesCount; ++i)
            {
                if(_filterRules[i].Resolve(fileProperties))
                {
                    matchingCount++;
                }
            }

            return matchingCount;
        }

        public bool Validate(IFileProperties fileProperties)
        {
            return (_validationRule != null) ? _validationRule.Resolve(fileProperties) : true;
        }

        public string ValidationRuleName => (_validationRule != null) ? _validationRule.Name : "auto-pass";

        public string OutputPath => _outputPath;

        public string OutputFileName => _outputFileName;

        public bool AutoConvert => _autoConvert;

        /// <summary>
        /// Resolve a tokenized string based on a file's properties
        /// </summary>
        /// <param name="tokenString">the value of either "output_path" or "output_file"</param>
        /// <param name="fileProperties">the file property object of the file being checked</param>
        /// <param name="matchingFileCount">the current number of files that match this <see cref="ValidationConfig"/>, including the current fileProperties</param>
        /// <param name="jsonPropertyName">literal either "output_path" or "output_file" (used for error output)</param>
        /// <returns>the resolved string with the tokens substituted</returns>
        private string ResolveTokenString(string tokenString, IFileProperties fileProperties, uint matchingFileCount, string jsonPropertyName)
        {
            string originalFileNameWithoutExtension = fileProperties.FileNameWithoutExtension;
            string[] fileNameParts = originalFileNameWithoutExtension.Split(_nameSeperator);
            string resolvedTokenString = tokenString;
            foreach (Match match in tokenRegexPattern.Matches(tokenString))
            {
                string token = match.Value;

                string partPrefix = "{part";
                string partSuffix = "}";
                if (token.StartsWith(partPrefix) && token.EndsWith(partSuffix))
                {
                    string partIndexStr = token.Substring(partPrefix.Length, token.Length - (partPrefix.Length + partSuffix.Length));
                    if(Int32.TryParse(partIndexStr, out int partIndex))
                    {
                        if(partIndex < 0 || partIndex >= fileNameParts.Length)
                        {
                            throw new IndexOutOfRangeException($"'part' index: {partIndex} out of range: {fileNameParts.Length} in validation config: {Name} for file: {originalFileNameWithoutExtension}");
                        }
                        resolvedTokenString = resolvedTokenString.Replace(token, fileNameParts[partIndex]);
                    }
                    else
                    {
                        throw new Exception($"Failed to parse 'part' token for file: {originalFileNameWithoutExtension} in validation cofig: {Name}");
                    }
                    continue;
                }

                switch (token)
                {
                    case "{current_name}":
                        {
                            resolvedTokenString = resolvedTokenString.Replace(token, fileProperties.FileNameWithoutExtension);
                            break;
                        }
                    case "{current_extension}":
                        {
                            BaseFileProperties baseFileProperties = fileProperties as BaseFileProperties;
                            string extensionWithoutDot = baseFileProperties.FileExtension.Substring(1);
                            resolvedTokenString = resolvedTokenString.Replace(token, extensionWithoutDot);
                            break;
                        }
                    case "{width}":
                        {
                            TextureFileProperties textureFileProperties = fileProperties as TextureFileProperties;
                            if (textureFileProperties == null)
                            {
                                throw new Exception($"file: {fileProperties.FileName} is not a texture file and thus doesn't have a 'width' property");
                            }
                            resolvedTokenString = resolvedTokenString.Replace(token, textureFileProperties.Width.ToString());
                            break;
                        }
                    case "{height}":
                        {
                            TextureFileProperties textureFileProperties = fileProperties as TextureFileProperties;
                            if (textureFileProperties == null)
                            {
                                throw new Exception($"file: {fileProperties.FileName} is not a texture file and thus doesn't have a 'height' property");
                            }
                            resolvedTokenString = resolvedTokenString.Replace(token, textureFileProperties.Height.ToString());
                            break;
                        }
                    case "{file_count}":
                        {
                            
                            resolvedTokenString = resolvedTokenString.Replace(token, matchingFileCount.ToString("D3"));
                            break;
                        }
                    default:
                        {
                            throw new Exception($"Unknown token: {token} found in '{jsonPropertyName}' of validation_config: {Name}");
                        }
                }
            }
            return resolvedTokenString;
        }

        /// <summary>
        /// Get the complete, fully resolved, path+filename of the fileProperties after tokens have been substituted
        /// </summary>
        /// <param name="searchDirectoryPath">The root dir that the search is running in</param>
        /// <param name="fileProperties">the file properties object</param>
        /// <param name="matchingFileCount">the number of files that have matched this config, including fileProperties</param>
        /// <returns></returns>
        public string GetResolvedCombinedPath(string searchDirectoryPath, IFileProperties fileProperties, uint matchingFileCount)
        {
            string resolvedOutputFileName = ResolveTokenString(_outputFileName, fileProperties, matchingFileCount,  "output_filename");
            string resolvedOutputPath = ResolveTokenString(_outputPath, fileProperties, matchingFileCount, "output_path");
            //If we have an "output_path" that is relative, combine it with the searchDirectoryPath
            if(resolvedOutputPath.StartsWith('.'))
            {
                resolvedOutputPath = Path.Combine(searchDirectoryPath, resolvedOutputPath);
            }

            return Path.Combine(resolvedOutputPath, resolvedOutputFileName);
        }
    }
}
