using System;
using System.Text;

namespace FileValidator.Rules
{
    /// <summary>
    /// Custom exception for ValidatorRules to throw
    /// </summary>
    public class RuleException : Exception
    {
        public RuleException(string message) : base(message) { }

        public RuleException(StringBuilder messageStringBuilder) : base(messageStringBuilder.ToString()) { }
    }
}
