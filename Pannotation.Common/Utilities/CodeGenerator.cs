using System;

namespace Pannotation.Common.Utilities
{
    public static class CodeGenerator
    {
        public static string GenerateCode()
        {
            int min = 1000;
            int max = 9999;
            Random rdm = new Random();
            return rdm.Next(min, max).ToString();
        }
    }
}
