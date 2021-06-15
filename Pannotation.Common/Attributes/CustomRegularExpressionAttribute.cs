using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pannotation.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = true, Inherited = true)]
    public class CustomRegularExpressionAttribute: RegularExpressionAttribute
    {
        private object _typeId = new object();
        public override object TypeId
        {
            get { return _typeId; }
        }

        public CustomRegularExpressionAttribute(string pattern):base(pattern)
        {
        }
    }
}
