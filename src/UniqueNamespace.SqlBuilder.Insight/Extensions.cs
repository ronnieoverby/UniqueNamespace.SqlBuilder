using System.Collections.Generic;
using Insight.Database;

namespace UniqueNamespace.Insight
{
    public static class Extensions
    {
        public static SqlBuilder.Template AddTemplate(this SqlBuilder builder, string sql, object parameters = null)
        {
            return builder.AddTemplate(sql, GetParameters(parameters));
        }

        private static IEnumerable<KeyValuePair<string, object>> GetParameters(object parameters)
        {
            if (parameters == null)
                yield break;

            foreach (var item in FastExpando.FromObject(parameters))
                yield return new KeyValuePair<string, object>(item.Key, item.Value);
        }

        static public SqlBuilder InnerJoin(this SqlBuilder builder, string sql, object parameters = null)
        {
            builder.InnerJoin(sql, GetParameters(parameters));
            return builder;
        }

        static public SqlBuilder LeftJoin(this SqlBuilder builder, string sql, object parameters = null)
        {
            builder.LeftJoin(sql, GetParameters(parameters));
            return builder;
        }

        public static SqlBuilder RightJoin(this SqlBuilder builder, string sql, object parameters = null)
        {
            builder.RightJoin(sql, GetParameters(parameters));
            return builder;
        }

        static public SqlBuilder Where(this SqlBuilder builder, string sql, object parameters = null)
        {
            builder.Where(sql, GetParameters(parameters));
            return builder;
        }

        static public SqlBuilder OrderBy(this SqlBuilder builder, string sql, object parameters = null)
        {
            builder.OrderBy(sql, GetParameters(parameters));
            return builder;
        }

        static public SqlBuilder Select(this SqlBuilder builder, string sql, object parameters = null)
        {
            builder.Select(sql, GetParameters(parameters));
            return builder;
        }

        static public SqlBuilder AddParameters(this SqlBuilder builder, object parameters)
        {
            builder.AddParameters(GetParameters(parameters));
            return builder;
        }

        static public SqlBuilder Join(this SqlBuilder builder, string sql, object parameters = null)
        {
            builder.Join(sql, GetParameters(parameters));
            return builder;
        }

        static public SqlBuilder GroupBy(this SqlBuilder builder, string sql, object parameters = null)
        {
            builder.GroupBy(sql, GetParameters(parameters));
            return builder;
        }

        static public SqlBuilder Having(this SqlBuilder builder, string sql, object parameters = null)
        {
            builder.Having(sql, GetParameters(parameters));
            return builder;
        }
    }


}
