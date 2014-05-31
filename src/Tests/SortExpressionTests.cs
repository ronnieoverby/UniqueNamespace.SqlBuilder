using System.Linq;
using NUnit.Framework;
using UniqueNamespace;
using UniqueNamespace.Insight;

namespace Tests
{
    public class SortExpressionTests
    {
        [Test]
        public void CanParseMultipleSorts()
        {
            var exps = SortExpression.Parse("Name, Age DeSc");
            Assert.AreEqual(2, exps.Length);

            Assert.AreEqual("Name", exps[0].Name);
            Assert.IsFalse(exps[0].Descending);

            Assert.AreEqual("Age", exps[1].Name);
            Assert.IsTrue(exps[1].Descending);
        }

        [Test]
        public void CanParseDirections()
        {
            SortExpression x = "A";
            Assert.IsFalse(x.Descending);

            x = "A ASC";
            Assert.IsFalse(x.Descending);

            x = "A asc";
            Assert.IsFalse(x.Descending);

            x = "A ASCENDING";
            Assert.IsFalse(x.Descending);

            x = "A ascending";
            Assert.IsFalse(x.Descending);

            x = "A DESC";
            Assert.IsTrue(x.Descending);

            x = "A desc";
            Assert.IsTrue(x.Descending);

            x = "A DESCENDING";
            Assert.IsTrue(x.Descending);

            x = "A descending";
            Assert.IsTrue(x.Descending);
        }

        [Test]
        public void ValidatesNames()
        {
            SortExpression x;
            Assert.Throws<InvalidSortExpressionNameException>(() => x = "My Column");
            Assert.Throws<InvalidSortExpressionNameException>(() => x = "[My Column");
            Assert.Throws<InvalidSortExpressionNameException>(() => x = "[My=Column]");
            Assert.Throws<InvalidSortExpressionNameException>(() => x = "[My Column`");
            Assert.Throws<InvalidSortExpressionNameException>(() => x = "[My Column]; or 1=1; delete database");

            x = " [My Column] ";
            Assert.AreEqual(x.Name, "[My Column]");
            x = " `My Column` ";
            Assert.AreEqual(x.Name, "`My Column`");
            x = " \"My Column\" ";
            Assert.AreEqual(x.Name, "\"My Column\"");
        }

        [Test]
        public void WhitelistNames()
        {
            var exps = SortExpression.Parse("This, That, [The Other]");
            Assert.AreEqual(3, exps.Length);

            var wl = exps.Whitelist("this");
            Assert.AreEqual(1, wl.Count());

            wl = exps.Whitelist("Tangy");
            Assert.AreEqual(0, wl.Count());

            wl = exps.Whitelist("The Other", "That"," [the other] ");
            Assert.AreEqual(2, wl.Count());
        }

        [Test]
        public void ValueEquality()
        {
            SortExpression a = "Name";
            SortExpression b = "[Name] ASC";
            SortExpression c = "Name DESC";

            Assert.AreEqual(a,b);
            Assert.AreNotEqual(a,c);
            Assert.AreNotSame(a,b);

            Assert.True(a.Equals(b));
            Assert.False(a.Equals(c));

            Assert.True(a == b);
            Assert.True(a != c);

            Assert.AreEqual(1, SortExpression.Parse("[Name], `Name`").Distinct().Count());

        }

        [Test]
        public void CanUseWithBuilder()
        {
            var sorts = SortExpression.Parse("This, That descending, [The Other]");
            var b = new SqlBuilder();
            b.OrderBy(sorts);
            var t = b.AddTemplate("{{ORDERBY}}");
            Assert.AreEqual("ORDER BY This ASC , That DESC , [The Other] ASC\r\n", t.RawSql);
        }
    }
}