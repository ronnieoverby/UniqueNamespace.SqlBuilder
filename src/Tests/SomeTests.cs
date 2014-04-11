using System;
using System.Data.SqlClient;
using Insight.Database;
using NUnit.Framework;
using UniqueNamespace;
using UniqueNamespace.Insight;

namespace Tests
{
    public class SomeTests
    {
        [Test]
        public void InsightNewStuff2()
        {
            var conn = new SqlConnection("server=.;database=__NAME__; integrated security = true");
            var repo = conn.As<SubRepoBase>();

            var r1 = repo.CountThings("ho ho");
            var r2 = repo.AnyThatAre(Guid.NewGuid());
        }

        [Test]
        public void InsightNewStuff()
        {
            var conn = new SqlConnection("server=.;database=__NAME__; integrated security = true");

            var b = new SqlBuilder()
                .InnerJoin("Categories c on c.Id = t.CategoryId")
                .InnerJoin("Accounts a on a.Id = t.AccountId")
                .Where("a.Household = @hh", new { hh = Guid.Empty })
                //.Where("c.Name = @category", new {category = "Gas"})
                .OrderBy("t.Date");

            var selector = b.AddTemplate("SELECT t.* FROM Transactions t {{INNERJOIN}} {{WHERE}} " +
                                         "{{ORDERBY}} OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY",
                new { skip = 2, take = 3 });

            var count = b.AddTemplate("select count(*) from Transactions t {{INNERJOIN}} {{WHERE}}");

            var results = conn.QueryResultsSql<dynamic, int>(selector.RawSql + ";" + count.RawSql,
                selector.Parameters.Expand(count.Parameters));
        }

        [Test]
        public void BasicContrivedTest()
        {
            var b = new SqlBuilder<SqlParameter>();
            var t = b.AddTemplate("{{SELECT}} FROM MyTable {{WHERE}}");
            var aParam = new SqlParameter("@a", 123);
            var t2 = b.AddTemplate("SELECT Count(*) FROM MyTable {{WHERE}}", aParam);

            var cParam = new SqlParameter("@c", "Kramer");
            var dParam = new SqlParameter("@d", "Seinfeld");

            b.Select("This, That")
                .Select("TheOther")
                .Where("Character = @c", cParam)
                .Where("Director = @d", dParam);

            Assert.AreEqual("SELECT This, That , TheOther\n FROM MyTable " +
                            "WHERE Character = @c AND Director = @d\n", t.RawSql);

            Assert.AreEqual("SELECT Count(*) FROM MyTable " +
                            "WHERE Character = @c AND Director = @d\n", t2.RawSql);

            CollectionAssert.AreEqual(new[] { cParam, dParam }, t.Parameters);
            CollectionAssert.AreEqual(new[] { aParam, cParam, dParam }, t2.Parameters);
        }
    }

    public interface IOne
    {
        int CountThings(string something);
    }

    public interface ITwo : IOne
    {
        bool AnyThatAre(Guid condition);
    }

    public abstract class RepoBase : ITwo
    {
        public abstract int CountThings(string something);
        public abstract bool AnyThatAre(Guid condition);

        public virtual int GetAge()
        {
            return 29;
        }
    }

    public abstract class SubRepoBase : RepoBase
    {
        public abstract string GetName(string thisAnd, string that);
    }
}
