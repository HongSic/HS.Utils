using System;
using System.Collections.Generic;

namespace HS.Utils
{
    public class SystemUtils
    {
        public static List<T> GetFlagsList<T>(T enumFlags) where T : Enum
        {
            List<T> result = new List<T>();

            foreach (T value in Enum.GetValues(typeof(T)))
            {
                if (enumFlags.HasFlag(value)) result.Add(value);
            }
            return result;
        }
    }
}
