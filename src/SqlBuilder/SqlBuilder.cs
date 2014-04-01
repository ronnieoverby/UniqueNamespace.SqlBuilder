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

        public SqlBuilder<TDbParams> Select(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.Select(sql, parameters);
        }

        public SqlBuilder<TDbParams> Where(string sql, params TDbParams[] parameters)
        {
            return (SqlBuilder<TDbParams>)base.Where(sql, parameters);
        }
    }
}