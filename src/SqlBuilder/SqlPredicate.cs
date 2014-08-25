using System;

namespace UniqueNamespace
{
    public class SqlPredicate
    {
        public string Sql { get; private set; }

        public SqlPredicate(string sql)
        {
            if (sql == null) throw new ArgumentNullException("sql");

            if (sql.IsNullOrWhiteSpace())
                throw new ArgumentOutOfRangeException("sql", "sql was empty");

            Sql = sql;
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
