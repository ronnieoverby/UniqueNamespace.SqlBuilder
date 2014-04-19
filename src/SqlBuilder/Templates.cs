using System;

namespace UniqueNamespace
{
    public static class Templates
    {
        private static readonly string NL = Environment.NewLine;

        private const string FromToHaving =
            " {{FROM}} {{JOIN}} {{INNERJOIN}} {{LEFTJOIN}} {{RIGHTJOIN}} {{CROSSJOIN}} {{WHERE}} {{GROUPBY}} {{HAVING}} ";

        public const string Selection = "{{SELECT}} " + FromToHaving + " {{ORDERBY}} ";

        public const string Count = "SELECT Count(*) " + FromToHaving;

        public static class SqlServer
        {
            internal const string ParamPrefix = "@";

            public static readonly string PagedSelection = "SELECT * " +
                                                           "FROM (" +
                                                           "{{SELECT}}, ROW_NUMBER() OVER( {{ORDERBY}} ) [RowNumber] " +
                                                           FromToHaving +
                                                           ") AS Data " +
                                                           "WHERE [RowNumber] BETWEEN ( (~Page - 1) * ~PageSize ) + 1 AND ~PageSize * ~Page"
                                                               .ReplaceParamPrefix(ParamPrefix);

            public static class V2012
            {
                public static readonly string PagedSelection = Selection +
                                                               "OFFSET ( (~Page - 1) * ~PageSize ) ROWS FETCH NEXT ~PageSize ROWS ONLY"
                                                                   .ReplaceParamPrefix(ParamPrefix);
            }

            public static class CE
            {
                public static readonly string PagedSelection = V2012.PagedSelection;
            }
        }

        public static class Oracle
        {
            internal const string ParamPrefix = ":";

            public static readonly string PagedSelection = SqlServer.PagedSelection.ReplaceParamPrefix(ParamPrefix);
        }

        public static class MySql
        {
            internal const string ParamPrefix = "?";

            public static readonly string PagedSelection = Selection +
                                                           " LIMIT (~Page - 1) * ~PageSize, ~PageSize"
                                                               .ReplaceParamPrefix(ParamPrefix);
        }

        public static class SQLite
        {
            internal const string ParamPrefix = "@";

            public static readonly string PagedSelection = MySql.PagedSelection.ReplaceParamPrefix(ParamPrefix);
        }

        public static class PostgreSQL
        {
            internal const string ParamPrefix = "@";

            public static readonly string PagedSelection = Selection +
                                                           " OFFSET (~Page - 1) * ~PageSize LIMIT ~PageSize"
                                                               .ReplaceParamPrefix(ParamPrefix);
        }

        public static string Combine(params string[] sqlStatements)
        {
            return string.Join(";" + NL + NL, sqlStatements);
        }

        private static string ReplaceParamPrefix(this string sql, string prefix)
        {
            return sql.Replace("~", prefix);
        }
    }
}