using System;
using System.Text;

namespace FileValidator.Config
{
    /// <summary>
    /// Custom exception class used by the <see cref="ConfigLoader"/>
    /// </summary>
    class ConfigException : Exception
    {
        public ConfigException(string message) : base(message) { }

        public ConfigException(StringBuilder messageStringBuilder) : base(messageStringBuilder.ToString()) { }
    }
}
