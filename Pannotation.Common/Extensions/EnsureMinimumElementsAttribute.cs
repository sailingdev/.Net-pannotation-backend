using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Common.Extensions
{
    public class EnsureMinimumElementsAttribute : ValidationAttribute
    {
        private readonly int _minElements;
        public EnsureMinimumElementsAttribute(int minElements)
        {
            _minElements = minElements;
        }

        public override bool IsValid(object value)
        {
            var list = value as ICollection;
            if (list != null)
            {
                return list.Count >= _minElements;
            }
            return false;
        }
    }
}
