using System;
using System.Collections.Generic;
using System.Linq;

namespace SSOService.Models.Enums.Dictionary
{
    public static class EnumLoopingUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
