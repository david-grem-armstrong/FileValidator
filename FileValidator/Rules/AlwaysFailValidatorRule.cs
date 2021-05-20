using FileValidator.FileProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileValidator.Rules
{
    /// <summary>
    /// A rule that always returns "false" (Can be used to always fail "validation_rule" for certain files)
    /// </summary>
    class AlwaysFailValidatorRule : BaseValidatorRule
    {
        public override bool Resolve(IFileProperties fileProperties)
        {
            return false;
        }
    }
}
