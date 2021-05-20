using System;
using System.IO;
using System.Threading.Tasks;

namespace FileValidator.FileProperties
{
    /// <summary>
    /// Basic file properties for a file (file name, extension, directory, etc)
    /// </summary>
    public class BaseFileProperties : IFileProperties
    {
        private FileInfo _fileInfo;

        public virtual Task PopuplateFromFileInfo(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
            return Task.CompletedTask;
        }

        public string FileName => _fileInfo.Name;

        public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(_fileInfo.Name);

        public string DirectoryName => _fileInfo.DirectoryName;

        public string FileExtension => _fileInfo.Extension;

        public long FileSizeBytes => _fileInfo.Length;

        public static Func<FileInfo, Task<IFileProperties>> FactoryFunc = CreateBaseFileProperties;
        private static async Task<IFileProperties> CreateBaseFileProperties(FileInfo fileInfo)
        {
            BaseFileProperties newFileProperties = new BaseFileProperties();
            await newFileProperties.PopuplateFromFileInfo(fileInfo);

            return newFileProperties;
        }
    }
}
