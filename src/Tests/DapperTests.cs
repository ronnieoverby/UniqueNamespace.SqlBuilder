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
            using (var db = new SqlConnection(ConnectionStrings.Default.ConnectionString))
                res = db.Query<int>(t.RawSql, t.Parameters).Single();
            Assert.AreEqual(p.a, res);
        }

        [Test]
        public void OrWhereSimpleTest()
        {
            var builder = new SqlBuilder();
            var query = builder.AddTemplate("SELECT * FROM Customers {{WHERE}}");

            builder.OrWhere("LastSeen IS NULL");
            builder.OrWhere("Gender = 'M'");

            var expected = "SELECT * FROM Customers WHERE ( LastSeen IS NULL OR Gender = 'M' )";

            Assert.AreEqual(expected.CleanupSql(), query.RawSql.CleanupSql());
        }

        [Test]
        public void OrWhereMixedTest()
        {
            var builder = new SqlBuilder();
            var query = builder.AddTemplate("SELECT * FROM Customers {{WHERE}}");

            builder.Where("Name = 'Dapper'");
            builder.OrWhere("LastSeen IS NULL");
            builder.Where("Age >= 21");
            builder.OrWhere("Gender = 'M'");

            var expected = "SELECT * FROM Customers WHERE Name = 'Dapper' AND Age >= 21 AND  ( LastSeen IS NULL OR Gender = 'M' )";

            Assert.AreEqual(expected.CleanupSql(), query.RawSql.CleanupSql());
        }

        [Test]
        public void UnionTest()
        {
            var builder = new SqlBuilder();
            var query = builder.AddTemplate("SELECT CustomerID FROM Customers WHERE CustomerID = 1 {{UNION}}");

            builder.Union("SELECT CustomerID FROM Customers WHERE CustomerID = 2");

            var expected = "SELECT CustomerID FROM Customers WHERE CustomerID = 1 UNION SELECT CustomerID FROM Customers WHERE CustomerID = 2";

            Assert.AreEqual(expected.CleanupSql(), query.RawSql.CleanupSql());
        }

        [Test]
        public void IntersectTest()
        {
            var builder = new SqlBuilder();
            var query = builder.AddTemplate("SELECT CustomerID FROM Customers WHERE CustomerID = 1 {{INTERSECT}}");

            builder.Intersect("SELECT CustomerID FROM Customers WHERE CustomerID = 2");

            var expected = "SELECT CustomerID FROM Customers WHERE CustomerID = 1 INTERSECT SELECT CustomerID FROM Customers WHERE CustomerID = 2";

            Assert.AreEqual(expected.CleanupSql(), query.RawSql.CleanupSql());
        }
    }
}