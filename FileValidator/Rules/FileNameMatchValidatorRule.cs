using FileValidator.FileProperties;

namespace FileValidator.Rules
{
    /// <summary>
    /// Rule to check if a file's name either StartsWith, EndsWith, or Contains one of "possible_matches"
    /// </summary>
    public class FileNameMatchValidatorRule : StringMatchValidatorRule
    {
        public override bool Resolve(IFileProperties fileProperties)
        {
            BaseFileProperties baseFileProperties = fileProperties as BaseFileProperties;
            if (baseFileProperties == null)
            {
                throw new RuleException($"Resolve unable to cast {nameof(fileProperties)} arg to {nameof(BaseFileProperties)}, {nameof(fileProperties)}.Type = {fileProperties.GetType()}");
            }

            return ResolveStringMatch(fileProperties.FileNameWithoutExtension);
        }
    }
}
