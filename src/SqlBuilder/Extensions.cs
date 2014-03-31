using System;

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
    }
}
