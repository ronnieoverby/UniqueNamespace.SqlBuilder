using System.Text.RegularExpressions;

namespace Tests
{
    static class Ext
    {
        public static string CleanupSql(this string sql)
        {
            return Regex.Replace(sql, @"\s+", " ").Trim();
        }
    }
}