using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Whether
{
    public static class Helper
    {
        public static string ToTempStat(int max, int min)
        {
            return $"{(max > 0 ? "+" + max.ToString() : max.ToString())} {(min > 0 ? "+" + min.ToString() : min.ToString())}";
        }
        public static void AddOrUpdate(this Dictionary<string, Tuple<DateTime, int>> dic, string key, DateTime lastEvent)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            if (dic.ContainsKey(key))
            {
                dic[key] = new Tuple<DateTime, int>(lastEvent, dic[key].Item2 + 1);
            }
            else
            {
                dic.Add(key, new Tuple<DateTime, int>(lastEvent, 1));
            }
        }
    }
}
