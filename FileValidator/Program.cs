using FileValidator.Config;
using FileValidator.FileLoaders;
using FileValidator.FileProperties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileValidator
{
    class FileValidator
    {
        /// <summary>
        /// Program "main"
        /// </summary>
        /// <param name="args">list of command line args. Should contain 
        ///                    [0]: the full path to the config file to run
        ///                    [1]: the path the the directory to search for files to run thru the validator</param>
        static void Main(string[] args)
        {
            bool missingArgs = false;
            if(args.Length < 1)
            {
                Console.WriteLine("Error: Missing arg for configFile path as 1st arg");
                missingArgs = true;
            }
            if (args.Length < 2)
            {
                Console.WriteLine("Error: Missing arg for search directory path as 2nd arg");
                missingArgs = true;
            }
            
            if (!missingArgs)
            {
                string configFilePath = args[0];
                string searchDirectoryPath = args[1];
                FileValidator fileValidator = new FileValidator();
                fileValidator.Run(configFilePath, searchDirectoryPath).GetAwaiter().GetResult();
            }

            Console.WriteLine("Press Enter to exit...");
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            while (keyInfo.Key != ConsoleKey.Enter)
            {
                keyInfo = Console.ReadKey(true);
            }
        }

        /// <summary>
        /// Async function to "Run" the <see cref="FileValidator"/> on a specific directory with a specific
        /// <see cref="ConfigFile"/>
        /// </summary>
        /// <param name="configFilePath">the path to the <see cref="ConfigFile"/></param>
        /// <param name="searchDirectoryPath">the path to the directory to search for files</param>
        /// <returns>awaitable <see cref="Task"/></returns>
        private async Task Run(string configFilePath, string searchDirectoryPath)
        {
            try
            {
                if (!Directory.Exists(searchDirectoryPath))
                {
                    throw new ArgumentException($"Search Directory '{searchDirectoryPath}' does not exist");
                }

                ConfigLoader configLoader = new ConfigLoader();
                FileLoader fileLoader = new FileLoader();

                //Load the config file
                ConfigFile configFile = await configLoader.AsyncLoadConfigFile(configFilePath);                

                //Get all file names in the search directory (and possibly sub folders)
                SearchOption searchOption = configFile.SearchAllSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                string[] allFiles = Directory.GetFiles(searchDirectoryPath, "*.*", searchOption);

                //For each file, create an appropriate IFileProperties object
                int fileCount = allFiles.Length;
                int validationConfigFileCount = configFile.ValidationConfigs.Count;
                Dictionary<string, IFileProperties> loadedFileProperties = new Dictionary<string, IFileProperties>();
                for (int fileIndex = 0; fileIndex < fileCount; ++fileIndex)
                {
                    string filePath = Path.GetFullPath(allFiles[fileIndex]);
                    //TODO rather than calling "await" in the loop, we should get the Tasks and do Task.WhenAll()
                    IFileProperties fileProperties = await fileLoader.FetchFileProperties(filePath);
                    loadedFileProperties.Add(filePath, fileProperties);
                }

                //for each file, find the best ValidationConfig (the first with the highest match)
                Dictionary<string, ValidationConfig> bestMatchingConfigs = new Dictionary<string, ValidationConfig>();
                foreach (var entry in loadedFileProperties)
                {
                    string filePath = entry.Key;
                    IFileProperties fileProperties = entry.Value;
                    uint higestMatchCount = 0;
                    ValidationConfig bestMatchConfig = null;
                    for (int configIndex = 0; configIndex < validationConfigFileCount; ++configIndex)
                    {
                        ValidationConfig validationConfig = configFile.ValidationConfigs[configIndex];
                        uint filterRulesMatchCount = validationConfig.NumberOfMatchingFilterRules(fileProperties);
                        //NOTE: if there are multiple validationConfigs that match the same number of filter_rules, the first will always win
                        if (higestMatchCount < filterRulesMatchCount)
                        {
                            higestMatchCount = filterRulesMatchCount;
                            bestMatchConfig = validationConfig;
                        }
                    }
                    bestMatchingConfigs.Add(filePath, bestMatchConfig);
                }

                //for each file resolve the ValidationConfig
                Dictionary<string, uint> validationConfigCount = new Dictionary<string, uint>();
                foreach (var entry in bestMatchingConfigs)
                {
                    string filePath = entry.Key;
                    ValidationConfig bestMatchConfig = entry.Value;
                    IFileProperties fileProperties = loadedFileProperties[filePath];

                    if(bestMatchConfig == null)
                    {
                        Console.WriteLine($"No match could be found for file: {fileProperties.FileName}");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine($"file: {fileProperties.FileName} matched validation config: {bestMatchConfig.Name}");
                    }

                    string configName = bestMatchConfig.Name;
                    if(!validationConfigCount.ContainsKey(configName))
                    {
                        validationConfigCount.Add(configName, 0);
                    }
                    validationConfigCount[configName] += 1;

                    //TODO possibly don't pass the count if it's 1?
                    string resolvedCombinedPath = bestMatchConfig.GetResolvedCombinedPath(searchDirectoryPath, fileProperties, validationConfigCount[configName]);
                    string fullResolvedPath = Path.GetFullPath(resolvedCombinedPath);
                    
                    //If the file isn't already named correctly and in the expected path
                    if (!string.Equals(fullResolvedPath, filePath, StringComparison.OrdinalIgnoreCase))
                    {
                        //If "auto_convert" is set, move/rename the file
                        if(bestMatchConfig.AutoConvert)
                        {
                            try
                            {
                                FileInfo newFileInfo = new FileInfo(fullResolvedPath);
                                //If the expected folder doesn't exist, create it
                                if (!newFileInfo.Directory.Exists)
                                {
                                    newFileInfo.Directory.Create();
                                }
                                //Move the file
                                File.Move(filePath, fullResolvedPath);
                                loadedFileProperties.Remove(filePath);
                                //Reload the IFileProperties in case the change will now change the result of ValidationConfig.Validate()
                                //TODO can we do this async call NOT in the loop?
                                fileProperties = await fileLoader.FetchFileProperties(fullResolvedPath);
                                filePath = fullResolvedPath;
                                loadedFileProperties[fullResolvedPath] = fileProperties;
                            }
                            catch(IOException e)
                            {
                                Console.WriteLine($"Unable to move/rename: {filePath} to: {fullResolvedPath}, exception: {e}");
                            }
                        }
                        else
                        {
                            //If "auto_convert" is not set, just output the recommended output
                            Console.WriteLine($"file: {fileProperties.FileName} is not set to 'auto_convert' and should moved/renamed to: {fullResolvedPath}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"file: {fileProperties.FileName} matches expected output");
                    }

                    if(bestMatchConfig.Validate(fileProperties))
                    {
                        Console.WriteLine($"File: {fileProperties.FileName} is valid");
                    }
                    else
                    {
                        Console.WriteLine($"File: {fileProperties.FileName} failed to validate {bestMatchConfig.ValidationRuleName}");
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine($"\n Unhandled exception: {e}");
            }
        }
    }
}
