using System.Text.RegularExpressions;

namespace Tests
{
    static class Extensions
    {
        public static string CleanupSql(this string sql)
        {
            return Regex.Replace(sql, @"\s+", " ").Trim();
        }
    }
}