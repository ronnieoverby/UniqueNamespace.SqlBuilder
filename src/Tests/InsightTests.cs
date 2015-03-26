using System;
using System.Data.SqlClient;
using System.Linq;
using FluentAssertions;
using Insight.Database;
using NUnit.Framework;
using UniqueNamespace;
using UniqueNamespace.Insight;

namespace Tests
{
    public class InsightTests
    {
        [Test]
        public void Test1()
        {
            var b = new SqlBuilder();
            var p = new { a = 123 };
            var t = b.AddTemplate("select @a {{WHERE}}", p);

            b.Where("@b = @c", new { b = "abc", c = "abc" });


            int res;
            using (var db = new SqlConnection(ConnectionStrings.Default.ConnectionString))
            {
                res = db.SingleSql<int>(t.RawSql, t.Parameters);
            }
            Assert.AreEqual(p.a, res);
        }


        [Test]
        public void SqlPaging()
        {
            var b = new SqlBuilder()
                .Select("*")
                .From("Orders o")
                .Where("o.ShipCountry = @country ", new {country = "USA"})
                .OrderBy("CustomerID DESC");

            var paging = new
            {
                Page = 1,
                PageSize = 25
            };

            var t = b.AddTemplate(
                Templates.Combine(Templates.SqlServer.PagedSelection, Templates.Count), 
                paging);

            using (var db = new SqlConnection(ConnectionStrings.Northwind))
            {
                Console.WriteLine(t.RawSql);
                var results = db.QueryResultsSql<dynamic, int>(t.RawSql, t.Parameters);
                results.Set1.Should().HaveCount(paging.PageSize, "because thats how many are in the page");
                results.Set2.Single().Should().Be(122, "because thats how many there are");

            }
        }
    }
}