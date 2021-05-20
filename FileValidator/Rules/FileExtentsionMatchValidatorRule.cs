using FileValidator.FileProperties;

namespace FileValidator.Rules
{
    /// <summary>
    /// Rule to check if the file's extension exist in "possible_matches"
    /// </summary>
    public class FileExtentsionMatchValidatorRule : StringOneOfValidatorRule
    {
        public override bool Resolve(IFileProperties fileProperties)
        {
            BaseFileProperties baseFileProperties = fileProperties as BaseFileProperties;
            if(baseFileProperties == null)
            {
                throw new RuleException($"Resolve unable to cast {nameof(fileProperties)} arg to {nameof(BaseFileProperties)}, {nameof(fileProperties)}.Type = {fileProperties.GetType()}");
            }

            return base.ResolveStringMatch(baseFileProperties.FileExtension);
        }
    }
}
