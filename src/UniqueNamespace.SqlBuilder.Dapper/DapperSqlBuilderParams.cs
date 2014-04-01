using Dapper;

namespace UniqueNamespace.Dapper
{
    public class DapperSqlBuilderParams : ISqlBuilderParams<object, DynamicParameters>
    {
        private readonly DynamicParameters _params = new DynamicParameters();

        public void Expand(dynamic parameters)
        {
            _params.AddDynamicParams(parameters);
        }

        public DynamicParameters Materialize()
        {
            return _params;
        }
    }
}