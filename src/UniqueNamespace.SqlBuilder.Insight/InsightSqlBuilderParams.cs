using Insight.Database;

namespace UniqueNamespace.Insight
{
    public class InsightSqlBuilderParams : ISqlBuilderParams<object, FastExpando>
    {
        private readonly FastExpando _data = new FastExpando();

        public void Expand(object parameters)
        {
            _data.Expand(parameters);
        }

        public FastExpando Materialize()
        {
            return _data;
        }
    }
}