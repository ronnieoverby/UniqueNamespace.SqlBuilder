using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using FluentAssertions;
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

            var t = b.AddTemplate(Templates.Selection);
            var t2 = b.AddTemplate(Templates.Count, aParam);

            var expected =
                "SELECT This, That , TheOther " +
                "FROM MyTable " +
                "WHERE Character = @c AND Director = @d " +
                "ORDER BY This , That , TheOther";

            Assert.AreEqual(expected.CleanupSql(), t.RawSql.CleanupSql(), "1");


            expected = "SELECT Count(*) " +
                       "FROM MyTable " +
                       "WHERE Character = @c AND Director = @d";

            Assert.AreEqual(expected.CleanupSql(), t2.RawSql.CleanupSql(), "2");

            CollectionAssert.AreEqual(new[] { cParam, dParam }, t.Parameters);
            CollectionAssert.AreEqual(new[] { aParam, cParam, dParam }, t2.Parameters);
        }


        [Test]
        public void BasicContrivedTestWithOrWhere()
        {
            var aParam = new SqlParameter("@a", 123);
            var cParam = new SqlParameter("@c", "Kramer");
            var dParam = new SqlParameter("@d", "Seinfeld");

            var b = new SqlBuilder<SqlParameter>()
                .Select("This, That")
                .Select("TheOther")
                .From("MyTable")
                .OrWhere("Character = @c", cParam)
                .OrWhere("Director = @d", dParam)
                .OrderBy(new[] { "This", "That", "TheOther" });

            var t = b.AddTemplate(Templates.Selection);
            var t2 = b.AddTemplate(Templates.Count, aParam);

            var expected =
                "SELECT This, That , TheOther " +
                "FROM MyTable " +
                "WHERE Character = @c OR Director = @d " +
                "ORDER BY This , That , TheOther";

            Assert.AreEqual(expected.CleanupSql(), t.RawSql.CleanupSql(), "1");


            expected = "SELECT Count(*) " +
                       "FROM MyTable " +
                       "WHERE Character = @c OR Director = @d";

            Assert.AreEqual(expected.CleanupSql(), t2.RawSql.CleanupSql(), "2");

            CollectionAssert.AreEqual(new[] { cParam, dParam }, t.Parameters);
            CollectionAssert.AreEqual(new[] { aParam, cParam, dParam }, t2.Parameters);
        }


        [Test]
        public void SqlBuilderOfDbParamsNotMissingMethods()
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            var baseMethods = typeof(SqlBuilderBase<,>).GetMethods(bindingFlags).Select(x => x.Name).OrderBy(x => x).ToArray();
            var methods = typeof(SqlBuilder<>).GetMethods(bindingFlags).Select(x => x.Name).OrderBy(x => x).ToArray();

            for (var i = 0; i < Math.Max(baseMethods.Length, methods.Length); i++)
            {
                var bm = baseMethods.ElementAtOrDefault(i);
                var m = methods.ElementAtOrDefault(i);

                bm.Should().NotBeNull();
                m.Should().NotBeNull();

                bm.Should().Be(m);
            }

            methods.ShouldAllBeEquivalentTo(baseMethods);
        }
    }
}
