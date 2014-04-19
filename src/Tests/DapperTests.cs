using System.Data.SqlClient;
using System.Linq;
using Dapper;
using NUnit.Framework;
using UniqueNamespace.Dapper;

namespace Tests
{
    public class DapperTests
    {
        [Test]
        public void Test1()
        {
            var b = new SqlBuilder();
            var p = new {a = 123};
            var t = b.AddTemplate("select @a {{WHERE}}", p);

            b.Where("@b = @c", new {b = "abc", c = "abc"});

            int res;
            using (var db = new SqlConnection(ConnectionStrings.Default))
                res = db.Query<int>(t.RawSql, t.Parameters).Single();
            Assert.AreEqual(p.a, res);
        }
    }
}