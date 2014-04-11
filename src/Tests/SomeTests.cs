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
}
