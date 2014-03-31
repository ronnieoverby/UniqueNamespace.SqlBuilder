using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Insight.Database;
using NUnit.Framework;
using UniqueNamespace;
using UniqueNamespace.Insight;

namespace Tests
{
    public class Class1
    {
        [Test]
        public void Test1()
        {
            var b = new SqlBuilder();

            var t = b.AddTemplate("SELECT * FROM Accounts {{WHERE}}");
            b.Where("Household = @hh", new {hh = Guid.NewGuid()});

            using (var conn = new SqlConnection("server=.;database=__NAME__;integrated security=true"))
            {
                var res = conn.QuerySql(t.RawSql, t.Parameters);
                var c = res.Count;
            }
        }
    }
}
