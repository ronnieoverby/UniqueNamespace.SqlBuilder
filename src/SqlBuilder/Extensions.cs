using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UniqueNamespace
{
    static class Extensions
    {
        public static string Replace(this string s, string oldValue, string newValue, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            var idx = 0;
            while (true)
            {
                idx = s.IndexOf(oldValue, idx, StringComparison.OrdinalIgnoreCase);
                if (idx == -1)
                    break;

                s = s.Remove(idx, oldValue.Length).Insert(idx, newValue);
                idx += newValue.Length;
            }

            return s;
        }

        public static IEnumerable<string> ReadLines(this string s)
        {
            using (var r = new StringReader(s))
                foreach (var line in r.ReadLines())
                    yield return line;
        }

        public static IEnumerable<string> ReadLines(this TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
                yield return line;
        }

        public static bool IsNullOrWhiteSpace(this string s)
        {
            return s == null || s.All(char.IsWhiteSpace);
        }
    }
}
