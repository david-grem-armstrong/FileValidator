using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileValidator.FileProperties
{
    public static class SupportedFileTypes
    {
        /// <summary>
        /// A static mapping of all file type extensions that can be supported by a custom <see cref="IFileProperties"/> type
        /// If NO match is found, <see cref="BaseFileProperties"/> will be used for basic file properties.
        /// 
        /// NOTE: If you want to add more supported file-types, just add the extension and an async-factory function below
        /// </summary>
        public static IReadOnlyDictionary<string, Func<FileInfo, Task<IFileProperties>>> ExtensionToFilePropertiesFactory 
            = new Dictionary<string, Func<FileInfo, Task<IFileProperties>>>()
        {
            { ".txt", TextFileProperties.FactoryFunc },
            { ".json", TextFileProperties.FactoryFunc },
            { ".png",  TextureFileProperties.FactoryFunc },
            { ".jpg", TextureFileProperties.FactoryFunc },
            { ".jpeg", TextureFileProperties.FactoryFunc },
        };
    }
}
