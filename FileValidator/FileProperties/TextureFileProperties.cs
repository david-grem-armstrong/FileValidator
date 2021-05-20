using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace FileValidator.FileProperties
{
    /// <summary>
    /// File properties of a texture file loaded via System.Drawing.Image.FromFile
    /// </summary>
    public class TextureFileProperties : BaseFileProperties
    {
        private uint _width = 0;
        private uint _height = 0;

        public override async Task PopuplateFromFileInfo(FileInfo fileInfo)
        {
            await base.PopuplateFromFileInfo(fileInfo);
            
            //TODO can this be async?
            Image loadedImage = Image.FromFile(fileInfo.FullName);

            if(loadedImage == null)
            {
                throw new Exception($"TextureFileProperties:PopuplateFromFileInfo unable to load image file: {fileInfo.FullName}");
            }

            _width = (uint)loadedImage.Width;
            _height = (uint)loadedImage.Height;

            //NOTE: because Image.FromFile locks the file until the image is release,
            //we SPECIFICALLY dispose and release the reference as quickly as possible
            loadedImage.Dispose();
            loadedImage = null;
        }

        public uint Width => _width;

        public uint Height => _height;

        public new static Func<FileInfo, Task<IFileProperties>> FactoryFunc = CreateTextureFileProperties;
        private static async Task<IFileProperties> CreateTextureFileProperties(FileInfo fileInfo)
        {
            TextureFileProperties newFileProperties = new TextureFileProperties();
            await newFileProperties.PopuplateFromFileInfo(fileInfo);

            return newFileProperties;
        }
    }
}
