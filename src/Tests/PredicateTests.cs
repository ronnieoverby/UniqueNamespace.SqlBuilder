using FluentAssertions;
using NUnit.Framework;
using UniqueNamespace;

namespace Tests
{
    public class PredicateTests
    {
        [Test]
        public void FalseIsFalse()
        {
            SqlPredicate.False.Sql.Should().Be("0 = 1");
        }

        [Test]
        public void TrueIsTrue()
        {
            SqlPredicate.True.Sql.Should().Be("1 = 1");
        }

        [Test]
        public void EmptyOrEachIsTrue()
        {
            new SqlPredicate[0].OrEach().Sql.ShouldBeEquivalentTo(SqlPredicate.True.Sql);
        }

        [Test]
        public void EmptyAndEachIsFalse()
        {
            new SqlPredicate[0].AndEach().Sql.ShouldBeEquivalentTo(SqlPredicate.False.Sql);
        }

        [Test]
        public void OrEachWorks()
        {
            new SqlPredicate[]{"a = b","b = c","c = d"}.OrEach().Sql
                .ShouldBeEquivalentTo("( ( a = b ) OR ( b = c ) ) OR ( c = d )");
        }


        [Test]
        public void AndEachWorks()
        {

            new SqlPredicate[]{"a = b","b = c","c = d","d = e","e = f"}.AndEach().Sql
                .ShouldBeEquivalentTo("( ( ( ( a = b ) AND ( b = c ) ) AND ( c = d ) ) AND ( d = e ) ) AND ( e = f )");
        }



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
