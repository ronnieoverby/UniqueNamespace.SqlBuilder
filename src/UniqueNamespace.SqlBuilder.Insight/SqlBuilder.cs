using Insight.Database;

namespace UniqueNamespace.Insight
{
    public class SqlBuilder : SqlBuilderBase<object, FastExpando>
    {
        protected override ISqlBuilderParams<object, FastExpando> CreateParams()
        {
            return new InsightSqlBuilderParams();
        }
    }
}