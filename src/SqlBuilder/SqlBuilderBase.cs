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

        public void AddClause(string name, string sql, TParamsIn parameters, string joiner, string prefix = "", string postfix = "")
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



        public SqlBuilderBase<TParamsIn,TParamsOut> From(string sql, TParamsIn parameters = null)
        {
            AddClause("from", sql, parameters, joiner: " , ", prefix: "\nFROM ", postfix: "\n");
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> InnerJoin(string sql, TParamsIn parameters = null)
        {
            AddClause("innerjoin", sql, parameters, joiner: nl + "INNER JOIN ", prefix: nl + "INNER JOIN ", postfix: nl);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> LeftJoin(string sql, TParamsIn parameters = null)
        {
            AddClause("leftjoin", sql, parameters, joiner: nl + "LEFT JOIN ", prefix: nl + "LEFT JOIN ", postfix: nl);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> RightJoin(string sql, TParamsIn parameters = null)
        {
            AddClause("rightjoin", sql, parameters, joiner: nl + "RIGHT JOIN ", prefix: nl + "RIGHT JOIN ", postfix: nl);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> Where(string sql, TParamsIn parameters = null)
        {
            AddClause("where", sql, parameters, " AND ", prefix: "WHERE ", postfix: nl);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> OrderBy(string sql, TParamsIn parameters = null)
        {
            AddClause("orderby", sql, parameters, " , ", prefix: "ORDER BY ", postfix: nl);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> Select(string sql, TParamsIn parameters = null)
        {
            AddClause("select", sql, parameters, " , ", prefix: "SELECT ", postfix: nl);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> AddParameters(TParamsIn parameters)
        {
            AddClause("--parameters", sql: "", parameters: parameters, joiner: "");
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> Join(string sql, TParamsIn parameters = null)
        {
            AddClause("join", sql, parameters, joiner: nl + "JOIN ", prefix: nl + "JOIN ", postfix: nl);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> GroupBy(string sql, TParamsIn parameters = null)
        {
            AddClause("groupby", sql, parameters, joiner: " , ", prefix: nl + "GROUP BY ", postfix: nl);
            return this;
        }

        public SqlBuilderBase<TParamsIn, TParamsOut> Having(string sql, TParamsIn parameters = null)
        {
            AddClause("having", sql, parameters, joiner: nl + "AND ", prefix: "HAVING ", postfix: nl);
            return this;
        }
    }
}

