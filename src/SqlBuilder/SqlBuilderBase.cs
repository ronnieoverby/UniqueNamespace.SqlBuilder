using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace UniqueNamespace
{

    public abstract class SqlBuilderBase<TParamsIn, TParamsOut>
        where TParamsIn : class
        where TParamsOut : class
    {
        private static readonly string nl = Environment.NewLine;

        readonly Dictionary<string, Clauses> _data = new Dictionary<string, Clauses>();
        int _seq;

        protected abstract ISqlBuilderParams<TParamsIn, TParamsOut> CreateParams();

        class Clause
        {
            public string Sql { get; set; }
            public TParamsIn Parameters { get; set; }
        }

        class Clauses : List<Clause>
        {
            readonly string _joiner;
            readonly string _prefix;
            readonly string _postfix;

            public Clauses(string joiner, string prefix = "", string postfix = "")
            {
                _joiner = joiner;
                _prefix = prefix;
                _postfix = postfix;
            }

            public string ResolveClauses(ISqlBuilderParams<TParamsIn, TParamsOut> p)
            {
                var sql = string.Join(_joiner, this.Select(clause =>
                {
                    if (clause.Parameters != null)
                        p.Expand(clause.Parameters);

                    return clause.Sql;
                }).ToArray());

                return _prefix + sql + _postfix;
            }
        }

        public class Template
        {
            readonly string _sql;
            readonly SqlBuilderBase<TParamsIn, TParamsOut> _builder;
            readonly ISqlBuilderParams<TParamsIn, TParamsOut> _initParams;
            int _dataSeq = -1; // Unresolved

            public Template(SqlBuilderBase<TParamsIn, TParamsOut> builder, string sql, TParamsIn parameters)
            {
                _sql = sql;
                _builder = builder;

                _initParams = builder.CreateParams();

                if (parameters != null)
                    _initParams.Expand(parameters);
            }

            private static readonly Regex Regex = new Regex(@"{{2}.+?}{2}", RegexOptions.Compiled | RegexOptions.Multiline);

            void ResolveSql()
            {
                if (_dataSeq == _builder._seq) return;

                var p = _initParams;
                _rawSql = _sql;

                foreach (var pair in _builder._data)
                {
                    var target = "{{" + pair.Key + "}}";
                    _rawSql = _rawSql.Replace(target, pair.Value.ResolveClauses(p), StringComparison.OrdinalIgnoreCase);
                }

                _parameters = p.Materialize();

                // replace all that is left with empty
                _rawSql = Regex.Replace(_rawSql, "");

                _dataSeq = _builder._seq;
            }

            string _rawSql;
            private TParamsOut _parameters;

            public string RawSql { get { ResolveSql(); return _rawSql; } }
            public TParamsOut Parameters
            {
                get
                {
                    ResolveSql();
                    return _parameters;
                }
            }
        }

        public Template AddTemplate(string sql, TParamsIn parameters = null)
        {
            return new Template(this, sql, parameters);
        }

        public void AddClause(string name, string sql, string joiner, string prefix = "", string postfix = "", TParamsIn parameters=null)
        {
            Clauses clauses;
            if (!_data.TryGetValue(name, out clauses))
            {
                clauses = new Clauses(joiner, prefix, postfix);
                _data[name] = clauses;
            }
            clauses.Add(new Clause { Sql = sql, Parameters = parameters });
            _seq++;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> From(string sql, TParamsIn parameters = null)
        {
            AddClause("From", sql, " , ", "FROM ", nl, parameters);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> Join(string sql, TParamsIn parameters = null)
        {
            AddClause("Join", sql, nl + "JOIN ", nl + "JOIN ", nl, parameters);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> InnerJoin(string sql, TParamsIn parameters = null)
        {
            AddClause("InnerJoin", sql, nl + "INNER JOIN ", nl + "INNER JOIN ", nl,parameters);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> LeftJoin(string sql, TParamsIn parameters = null)
        {
            AddClause("LeftJoin", sql, nl + "LEFT JOIN ", nl + "LEFT JOIN ", nl,parameters);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> RightJoin(string sql, TParamsIn parameters = null)
        {
            AddClause("RightJoin", sql, nl + "RIGHT JOIN ", nl + "RIGHT JOIN ", nl,parameters);
            return this;
        }

        protected SqlBuilderBase<TParamsIn, TParamsOut> FullOuterJoin(string sql, TParamsIn parameters= null)
        {
            AddClause("FullOuterJoin", sql, nl + "FULL OUTER JOIN ", nl + "FULL OUTER JOIN ", nl, parameters);
            return this;
        }

        protected SqlBuilderBase<TParamsIn, TParamsOut> CrossJoin(string sql, TParamsIn parameters= null)
        {
            AddClause("CrossJoin", sql, nl + "CROSS JOIN ", nl + "CROSS JOIN ", nl,parameters);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> Where(string sql, TParamsIn parameters = null)
        {
            AddClause("Where", sql,  " AND ", "WHERE ", nl,parameters);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> OrderBy(string sql, TParamsIn parameters = null)
        {
            AddClause("OrderBy", sql, " , ", "ORDER BY ", nl, parameters);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> OrderBy(IEnumerable<string> sqls)
        {
            if (sqls == null) throw new ArgumentNullException("sqls");

            foreach (var sql in sqls)
                OrderBy(sql);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> Select(string sql, TParamsIn parameters = null)
        {
            AddClause("Select", sql,  " , ", "SELECT ", nl, parameters);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> Columns(string sql, TParamsIn parameters = null)
        {
            AddClause("Columns", sql,  " , ", "", nl, parameters);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> GroupBy(string sql, TParamsIn parameters = null)
        {
            AddClause("GroupBy", sql, " , ", nl + "GROUP BY ", nl, parameters);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> Having(string sql, TParamsIn parameters = null)
        {
            AddClause("Having", sql, nl + "AND ", "HAVING ", nl, parameters);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> AddParameters(TParamsIn parameters)
        {
            AddClause("--parameters", "", "", parameters: parameters);
            return this;
        }
    }
}

