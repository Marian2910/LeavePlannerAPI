using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Exceptions
{
    public class LessThanZeroNumbers : Exception
    {
        public LessThanZeroNumbers() { }
        public LessThanZeroNumbers(string errorMessage) : base(errorMessage) { }
        public LessThanZeroNumbers(string errorMessage, Exception innerException) : base(errorMessage, innerException) { }
        public LessThanZeroNumbers(int number): base(FormattableString.Invariant($"The {number} should be greater than 0")) { }

    }
}
