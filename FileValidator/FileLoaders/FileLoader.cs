using FileValidator.FileProperties;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileValidator.FileLoaders
{
    public class FileLoader
    {
        /// <summary>
        /// Class that creates an appropriate <see cref="IFileProperties"/> object from a filePath
        /// based on the mapping <see cref="SupportedFileTypes"/>
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <returns></returns>
        public async Task<IFileProperties> FetchFileProperties(string fullFilePath)
        {
            FileInfo fileInfo = new FileInfo(fullFilePath);

            IFileProperties fileProperties = null;
            if (SupportedFileTypes.ExtensionToFilePropertiesFactory.ContainsKey(fileInfo.Extension))
            {
                Func<FileInfo, Task<IFileProperties>> filePropertiesCreateFunc = SupportedFileTypes.ExtensionToFilePropertiesFactory[fileInfo.Extension];
                fileProperties = await filePropertiesCreateFunc(fileInfo);
            }
            else
            {
                Console.WriteLine($"Warning: FileLoader attempt to load unknown file extension: {fileInfo.Extension}, defaulting to {nameof(BaseFileProperties)}");
                fileProperties = new BaseFileProperties();
                await fileProperties.PopuplateFromFileInfo(fileInfo);
            }

            return fileProperties;
        }
    }
}
