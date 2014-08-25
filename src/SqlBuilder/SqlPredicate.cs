using System;
using System.Collections.Generic;
using System.Linq;

namespace UniqueNamespace
{
    static public class SqlPredicateExtensions
    {
        public static SqlPredicate OrEach(this IEnumerable<SqlPredicate> predicates)
        {
            if (predicates == null) throw new ArgumentNullException("predicates");

            var list = predicates.ToList();

            if (!list.Any())
                list.Add(SqlPredicate.True);

            return list.Aggregate((x, y) => x.Or(y));
        }

        public static SqlPredicate AndEach(this IEnumerable<SqlPredicate> predicates)
        {
            if (predicates == null) throw new ArgumentNullException("predicates");

            var list = predicates.ToList();

            if (!list.Any())
                list.Add(SqlPredicate.False);

            return list.Aggregate((x, y) => x.And(y));
        }
    }

    public class SqlPredicate
    {
        public static readonly SqlPredicate False = "0 = 1";
        public static readonly SqlPredicate True = "1 = 1";
        private readonly string _sql;

        public string Sql
        {
            get { return _sql; }
        }

        public SqlPredicate(string sql )
        {
            if (sql.IsNullOrWhiteSpace()) 
                throw new ArgumentNullException("sql", "sql must not be empty");

            _sql = sql;
        }

        public SqlPredicate And(SqlPredicate p)
        {

            return string.Format("( {0} ) AND ( {1} )", Sql, p.Sql);
        }

        public SqlPredicate Or(SqlPredicate p)
        {
            return string.Format("( {0} ) OR ( {1} )", Sql, p.Sql);
        }

        public static implicit operator SqlPredicate(string sql)
        {
            return new SqlPredicate(sql);
        }

        public override string ToString()
        {
            return Sql;
        }

    }
}
