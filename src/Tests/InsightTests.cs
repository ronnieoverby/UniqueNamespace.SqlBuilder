using System.Data.SqlClient;
using Insight.Database;
using NUnit.Framework;
using UniqueNamespace.Insight;

namespace Tests
{
    public class InsightTests
    {
        [Test]
        public void Tes1()
        {
            var b = new SqlBuilder();
            var p = new {a = 123};
            var t = b.AddTemplate("select @a {{WHERE}}", p);
            
            b.Where("@b = @c", new {b = "abc", c = "abc"});


            var db = new SqlConnection("server=.;integrated security=true");
            var res = db.SingleSql<int>(t.RawSql, t.Parameters);
            Assert.AreEqual(p.a, res);
        }
    }
}