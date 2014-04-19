using System.Collections.Generic;
using System.Data.Common;

namespace UniqueNamespace
{

    public class SqlBuilder<TDbParams> : SqlBuilderBase<IEnumerable<TDbParams>, TDbParams[]> where TDbParams : DbParameter
    {
        protected override ISqlBuilderParams<IEnumerable<TDbParams>, TDbParams[]> CreateParams()
        {
            return new SqlBuilderParams<TDbParams>();
        }

        public Template AddTemplate(string sql, params TDbParams[] parameters)
        {
            return base.AddTemplate(sql, parameters);
        }

        public SqlBuilder<TDbParams> From(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.From(sql, parameters);
        }

        public SqlBuilder<TDbParams> Select(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.Select(sql, parameters);
        }

        public SqlBuilder<TDbParams> Where(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.Where(sql, parameters);
        }

        public SqlBuilder<TDbParams> Join(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.Join(sql, parameters);
        }

        public SqlBuilder<TDbParams> InnerJoin(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.InnerJoin(sql, parameters);
        }

        public SqlBuilder<TDbParams> RightJoin(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.RightJoin(sql, parameters);
        }

        public SqlBuilder<TDbParams> LeftJoin(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.LeftJoin(sql, parameters);
        }

        public SqlBuilder<TDbParams> FullOuterJoin(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.FullOuterJoin(sql, parameters);
        }

        public SqlBuilder<TDbParams> OrderBy(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.OrderBy(sql, parameters);
        }

        public SqlBuilder<TDbParams> GroupBy(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.GroupBy(sql, parameters);
        }

        public SqlBuilder<TDbParams> Having(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.Having(sql, parameters);
        }

        public new SqlBuilder<TDbParams> OrderBy(IEnumerable<string> sqls)
        {
            return (SqlBuilder<TDbParams>) base.OrderBy(sqls);
        }
    }
}