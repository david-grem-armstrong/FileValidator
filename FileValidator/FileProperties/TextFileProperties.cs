using System;
using System.IO;
using System.Threading.Tasks;

namespace FileValidator.FileProperties
{
    /// <summary>
    /// File properties of a text file
    /// </summary>
    public class TextFileProperties : BaseFileProperties
    {
        private string _fileContents;

        public override async Task PopuplateFromFileInfo(FileInfo fileInfo)
        {
            _fileContents = await File.ReadAllTextAsync(fileInfo.FullName);

            await base.PopuplateFromFileInfo(fileInfo);
        }

        public string FileContents => _fileContents;

        public new static Func<FileInfo, Task<IFileProperties>> FactoryFunc = CreateTextFileProperties;
        private static async Task<IFileProperties> CreateTextFileProperties(FileInfo fileInfo)
        {
            TextFileProperties newFileProperties = new TextFileProperties();
            await newFileProperties.PopuplateFromFileInfo(fileInfo);

            return newFileProperties;
        }
    }
}
