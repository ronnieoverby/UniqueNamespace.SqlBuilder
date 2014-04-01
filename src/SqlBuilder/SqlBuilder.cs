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
    }
}