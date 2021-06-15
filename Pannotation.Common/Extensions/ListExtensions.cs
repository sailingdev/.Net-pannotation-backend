using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pannotation.Common.Extensions
{
    public static class ListExtensions
    {
        public static List<TResult> Empty<TResult>(this List<TResult> list)
        {
            return new List<TResult>();
        }
    }
}
