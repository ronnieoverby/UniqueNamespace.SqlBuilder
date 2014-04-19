using System;
using System.Data.SqlClient;
using NUnit.Framework;
using UniqueNamespace;

namespace Tests
{
    // TODO: mo' better testing

    public class SomeTests
    {
        [Test]
        public void BasicContrivedTest()
        {
            var aParam = new SqlParameter("@a", 123);
            var cParam = new SqlParameter("@c", "Kramer");
            var dParam = new SqlParameter("@d", "Seinfeld");

            var b = new SqlBuilder<SqlParameter>()
                .Select("This, That")
                .Select("TheOther")
                .From("MyTable")
                .Where("Character = @c", cParam)
                .Where("Director = @d", dParam)
                .OrderBy(new[] { "This", "That", "TheOther" });

            var t = b.AddTemplate(Templates.SelectionTemplate);
            var t2 = b.AddTemplate(Templates.CountTemplate, aParam);

            var nl = Environment.NewLine;
            var expected =
                "SELECT This, That , TheOther " +
                "FROM MyTable " +
                "WHERE Character = @c AND Director = @d " +
                "ORDER BY This , That , TheOther";

            Assert.AreEqual(expected.CleanupSql(), t.RawSql.CleanupSql(), "1");


            expected = "SELECT Count(*) " +
                       "FROM MyTable " +
                       "WHERE Character = @c AND Director = @d " +
                       "ORDER BY This , That , TheOther";

            Assert.AreEqual(expected.CleanupSql(), t2.RawSql.CleanupSql(), "2");

            CollectionAssert.AreEqual(new[] { cParam, dParam }, t.Parameters);
            CollectionAssert.AreEqual(new[] { aParam, cParam, dParam }, t2.Parameters);
        }
    }
}
