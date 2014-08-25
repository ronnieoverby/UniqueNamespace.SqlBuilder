using FluentAssertions;
using NUnit.Framework;
using UniqueNamespace;

namespace Tests
{
    public class PredicateTests
    {
        [Test]
        public void CanAndSomeStuff()
        {
            SqlPredicate p1 = "1 = 1";
            SqlPredicate p2 = "2 = 2";

            var final = p1.And(p2);

            final.Sql.Should().Be("( 1 = 1 ) AND ( 2 = 2 )");
        }
        
        [Test]
        public void CanOrSomeStuff()
        {
            SqlPredicate p1 = "1 = 1";
            SqlPredicate p2 = "2 = 2";

            var final = p1.Or(p2);

            final.Sql.Should().Be("( 1 = 1 ) OR ( 2 = 2 )");
        }
        
        [Test]
        public void CanComposeSomeStuff()
        {
            SqlPredicate p1 = "1 = 1";
            SqlPredicate p2 = "2 = 2";
            SqlPredicate p3 = "10 = 20";
            SqlPredicate p4 = "something IS NULL";

            var p5 = p1.And(p2);
            var p6 = p3.And(p4);
            var final = p5.Or(p6);

            final.Sql.Should().Be("( ( 1 = 1 ) AND ( 2 = 2 ) ) OR ( ( 10 = 20 ) AND ( something IS NULL ) )");
        }
    }
}
