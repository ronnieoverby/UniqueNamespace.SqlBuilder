using System.Collections.Generic;
using System.Data.SqlClient;
using Insight.Database;
using NUnit.Framework;
using UniqueNamespace.Insight;

namespace Tests
{
    public class InsightTests
    {
        [Test]
        public void ExampleForReadme()
        {
            SqlConnection connection = new SqlConnection("server=.;integrated security=true");
            var userQuery =
                new {This = (int?) 2, That = 3, SortExpressions = new []{"This", "That DESC"} };

            var builder = new SqlBuilder();
            var query = builder.AddTemplate("SELECT Id, This, That, TheOther " +
                                            "FROM MyTable {{WHERE}} {{ORDERBY}}" +
                                            "OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY",
                new {skip = 10, take = 25});

            var count = builder.AddTemplate("SELECT Count(*) FROM MyTable {{WHERE}}");

            if (userQuery.This != null)
                builder.Where("This = @This", new { userQuery.This });

            if (userQuery.That != null)
                builder.Where("That = @That", new { userQuery.That });

            if (userQuery.SortExpressions != null)
                foreach (var sort in userQuery.SortExpressions)
                    builder.OrderBy(sort);

            // using Insight.Database (MY FAVORITE!)
            var results = connection.QuerySql<dynamic>(query.RawSql, query.Parameters);
            var totalRows = connection.SingleSql<int>(count.RawSql, count.Parameters);

            var xxx = new {results, totalRows};
        }

        [Test]
        public void Tes1()
        {
            var b = new SqlBuilder();
            var p = new { a = 123 };
            var t = b.AddTemplate("select @a {{WHERE}}", p);

            b.Where("@b = @c", new { b = "abc", c = "abc" });


            var db = new SqlConnection("server=.;integrated security=true");
            var res = db.SingleSql<int>(t.RawSql, t.Parameters);
            Assert.AreEqual(p.a, res);
        }
    }
}