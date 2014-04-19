using System;

namespace UniqueNamespace
{
    /// <summary>
    /// Prebuilt templates.
    /// </summary>
    public static class Templates
    {
        private static readonly string NL = Environment.NewLine;

        private const string FromToHaving =
            " {{FROM}} {{JOIN}} {{INNERJOIN}} {{LEFTJOIN}} {{RIGHTJOIN}} {{CROSSJOIN}} {{WHERE}} {{GROUPBY}} {{HAVING}} ";

        public const string Selection = "{{SELECT}} " + FromToHaving + " {{ORDERBY}} ";

        public const string Count = "SELECT Count(*) " + FromToHaving;

        public static class SqlServer
        {

            public static readonly string PagedSelection = "SELECT * " +
                                                           "FROM (" +
                                                           "{{SELECT}}, ROW_NUMBER() OVER( {{ORDERBY}} ) [RowNumber] " +
                                                           FromToHaving +
                                                           ") AS Data " +
                                                           "WHERE [RowNumber] BETWEEN ( (@Page - 1) * @PageSize ) + 1 AND @PageSize * @Page";

            public static class V2012
            {
                public static readonly string PagedSelection = Selection +
                                                               "OFFSET ( (@Page - 1) * @PageSize ) ROWS FETCH NEXT @PageSize ROWS ONLY";
            }

            public static class CE
            {
                public static readonly string PagedSelection = V2012.PagedSelection;
            }
        }

        public static class Oracle
        {
            public static readonly string PagedSelection = "SELECT * " +
                                                            "FROM (" +
                                                            "{{SELECT}}, ROW_NUMBER() OVER( {{ORDERBY}} ) [RowNumber] " +
                                                            FromToHaving +
                                                            ") AS Data " +
                                                            "WHERE [RowNumber] BETWEEN ( (:Page - 1) * :PageSize ) + 1 AND :PageSize * :Page";
        }

        public static class MySql
        {

            public static readonly string PagedSelection = Selection + " LIMIT (?Page - 1) * ?PageSize, ?PageSize";
        }

        public static class SQLite
        {

            public static readonly string PagedSelection = Selection + " LIMIT (@Page - 1) * @PageSize, @PageSize";
        }

        public static class PostgreSQL
        {

            public static readonly string PagedSelection = Selection + " OFFSET (@Page - 1) * @PageSize LIMIT @PageSize";
        }

        public static string Combine(params string[] sqlStatements)
        {
            return string.Join(";" + NL + NL, sqlStatements);
        }

      
    }
}