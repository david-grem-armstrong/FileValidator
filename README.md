# FileValidator<br/>
A simple Windows .Net 5 Console Application to sort/validate files in a folder structure based on a JSON config file<br/>

# Dependencies<br/>
  -.NET Core 5.0<br/>
NuGet Packages:<br/>
  -Newtonsoft.Json (13.0.1)<br/>
  -JsonSubTypes (1.8.0)<br/>
  -System.Drawing.Common (5.0.2)<br/>
 
# How it Works
The purpose of the FileValidator is to set up a FileValidatorConfig.json file to filter, sort, and validate files in a specific directory (and possibly sub-directories).
When ran, the FileValidator expects 2 command-line arguments: The path to the .json config file you want to use, and the path the the specific directory you want to run it against.<br/>

When ran, each file that exists in the search directories will be loaded and key information will be drawn out about the file into a FileProperties class that implements the IFileProperties interface based on the extension of that file.<br/>

If a file is found that does not exist in the SupportedFileTypes.ExtensionToFilePropertiesFactory static dictionary, BaseFileProperties will be used to load basic information about the file only (file name, directory name, file size in bytes, and file extension)<br/>

Supported special file-types:<br/>
".txt" - TextFileProperties<br/>
".json" - TextFileProperties<br/>
".png" - TextureFileProperties<br/>
".jpeg" - TextureFileProperties<br/>
".jpg" - TextureFileProperties<br/>

Additional FileProperties types can be added by deriving from BaseFileProperties and updating the SupportedFileTypes.ExtensionToFilePropertiesFactory static dictionary<br/>

a compatible config .json file has the following structure:<br/>

"search_all_sub_folders" (boolean) //A flag for if the all sub-directories should be search OR just the top-level directory<br/>
"validation_configs" (Array of ValidationConfig objects) //A list of different configurations that *could* match any file in the search directories<br/>

A ValidationConfig contains:<br/>
"name" (string) //A human-readable name used for console output to show which ValidationConfig is chosen to "match" any specific file<br/>
"filter_rules" (Array of ValidationRule objects) //A list of different rules that resolve to either true or false depending on different qualities of the current file being compared against this ValidationConfig. NOTE: the first declared ValidationConfig with the most matching "filter_rules" for a file is the one selected to "run" on that file.<br/>
"validation_rule" (ValidationRule object, optional) //A single rule used to determine if the file is/isn't "valid". If not included, all files that match this config are valid.<br/>
"output_path" (string, default = *SearchDirectoryRoot*) //The expected path that the file *should* belong in if it matches this ValidationConfig (Can be tokenized)<br/>
"output_filename" (string) //The expected file_name that the file *should* belong in if it matches this ValidationConfig (Can be tokenized)<br/>
"name_seperator" (string) //A string used to separate the original file name into parts for tokenization in "output_path" or "output_filename"<br/>
"auto_convert" (boolean, optional) //If set to true, if a file matches this config and does NOT match "output_path" and "output_filename", it will automatically be moved/renamed. Otherwise, the console will output the expected resolved "output_path" combined with "output_filename"<br/>

A ValidationRule contains:<br/>
"name" (string) //A human-readable name used for console output to show if the ValidationRule throws an error or fails to "validate"<br/>
"rule_type" (string) //The class name of the C# class that ValidationRule json will be parsed into at runtime (meaning that every supported ValidationRule can expect different fields beyond these two)<br/>

Supported ValidationRules:<br/>
"TextureSizeValidatorRule" (can only be ran on TextureFileProperties files), a rule that checks if both the width and height of a texture are Greater Than, Less Than, or Equal To an expected value<br/>
"FileExtentsionMatchValidatorRule", a rule that checks if a file has an extension that matches one of a set of possibilities<br/>
"FileNameMatchValidatorRule", a rule that checks if a file name (without extension) either Starts With, Ends With, Contains, or Equals one of a set of possibilities<br/>
"AndValidatorRule", a logical "And" operation on a list of ValidatorRules, resolving to false on the first sub-rule to resolve false<br/>
"OrValidatorRule", a logical "Or" operation on a list of ValidatorRules, resolving to true on the first sub-rule to resolve true<br/>
"NotValidatorRule", a logical "Not" operation on a single ValidatorRule to invert the sub-rule's resolution<br/>
"AlwaysFailValidator", a rule that will alway return false when resolved (this can be used to fail "validation_rule" for a specific set of filtered files)<br/>

It is possible to easily add additional ValidatorRules by deriving from the partially abstract BaseValidatorRule and updating the static dictionary inside of ValidatorRulesSerializationSettings<br/>

Output Tokenization:<br/>
"output_path" and "output_filename" can be written with a "{}" format to support filling in parts of the path or name with qualities from the file itself.<br/>
Supported tokens:<br/>
"{current_name}": the current name of the file (without extension)<br/>
"{current_extension}": the current extension of the file<br/>
"{width}": the current width of texture image (throws an exception if the IFileProperties is not a TextureFileProperties)<br/>
"{height}": the current height of texture image (throws an exception if the IFileProperties is not a TextureFileProperties)<br/>
"{file_count}": a 3 digit counting number of all files that match this ValidationConfig so far (used to avoid name collisions)<br/>
"{part[NUMBER]}": a special token where [NUMBER] is an integer index into the original file name split by "name_seperator" (throws exception if out of range of split array), this can be used to preserve portions of the original file name as desired.<br/>

NOTE: there is nothing protecting the FileValidator.exe OR the loaded config .json file from being filter INSIDE of the search directory tree, so use with caution.<br/>

Example config .json file and search folder shown in /Sample folder<br/>
