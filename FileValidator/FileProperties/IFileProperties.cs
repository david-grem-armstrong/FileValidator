using System.IO;
using System.Threading.Tasks;

namespace FileValidator.FileProperties
{
    /// <summary>
    /// Common interface between all file-properties objects
    /// Used to pass to all classes that extend <see cref="BaseValidatorRule"/> 
    /// </summary>
    public interface IFileProperties
    {
        Task PopuplateFromFileInfo(FileInfo fileInfo);

        string FileName { get; }
        string FileNameWithoutExtension { get; }
    }
}
