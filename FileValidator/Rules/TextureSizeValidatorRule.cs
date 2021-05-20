using FileValidator.FileProperties;

namespace FileValidator.Rules
{
    /// <summary>
    /// Class for resolving if an image file matches the expected height & width
    /// </summary>
    class TextureSizeValidatorRule : NumberComparisonValidatorRule
    {
        public override bool Resolve(IFileProperties fileProperties)
        {
            TextureFileProperties textureFileProperties = fileProperties as TextureFileProperties;
            if (textureFileProperties == null)
            {
                throw new RuleException($"Resolve unable to cast {nameof(fileProperties)} arg to {nameof(TextureFileProperties)}, {nameof(fileProperties)}.Type = {fileProperties.GetType()}");
            }
            bool resolveWidth = ResolveNumberValue(textureFileProperties.Width);
            bool resolveHeight = ResolveNumberValue(textureFileProperties.Height);

            //TODO consider supporting different width and height
            return resolveWidth && resolveHeight;
        }
    }
}
