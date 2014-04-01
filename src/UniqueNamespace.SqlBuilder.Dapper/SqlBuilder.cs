using Dapper;

namespace UniqueNamespace.Dapper
{
    public class SqlBuilder : SqlBuilderBase<object, DynamicParameters>
    {
        protected override ISqlBuilderParams<object, DynamicParameters> CreateParams()
        {
            return new DapperSqlBuilderParams();
        }
    }
}