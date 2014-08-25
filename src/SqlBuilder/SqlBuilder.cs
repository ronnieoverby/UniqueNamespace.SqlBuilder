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

        public void AddClause(string name, string sql, string joiner, string prefix = "", string postfix = "", params TDbParams[] parameters)
        {
            base.AddClause(name, sql, joiner, prefix, postfix, parameters);
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

        public SqlBuilder<TDbParams> CrossJoin(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.CrossJoin(sql, parameters);
        }

        public SqlBuilder<TDbParams> OrderBy(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.OrderBy(sql, parameters);
        }

        public new SqlBuilder<TDbParams> OrderBy(IEnumerable<string> sqls)
        {
            return (SqlBuilder<TDbParams>)base.OrderBy(sqls);
        }

        public new SqlBuilder<TDbParams> OrderBy(IEnumerable<SortExpression> sortExpressions)
        {
            return (SqlBuilder<TDbParams>)base.OrderBy(sortExpressions);
        }

        public SqlBuilder<TDbParams> GroupBy(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.GroupBy(sql, parameters);
        }

        public SqlBuilder<TDbParams> Having(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.Having(sql, parameters);
        }

        public SqlBuilder<TDbParams> Columns(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.Columns(sql, parameters);
        }

        public SqlBuilder<TDbParams> AddParameters(params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.AddParameters(parameters);
        }
    }
}