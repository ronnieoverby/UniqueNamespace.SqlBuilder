using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Parameters = System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>>;

namespace UniqueNamespace
{
    public class SqlBuilder
    {
        readonly Dictionary<string, Clauses> _data = new Dictionary<string, Clauses>();
        int _seq;

        class Clause
        {
            public string Sql { get; set; }
            public Parameters Parameters { get; set; }
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

            public string ResolveClauses(IDictionary<string, object> p)
            {
                var sql = string.Join(_joiner, this.Select(clause =>
                {
                    if (clause.Parameters != null)
                        foreach (var cp in clause.Parameters)
                            p[cp.Key] = cp.Value;

                    return clause.Sql;
                }).ToArray());

                return _prefix + sql + _postfix;
            }
        }

        public class Template
        {
            readonly string _sql;
            readonly SqlBuilder _builder;
            readonly Parameters _initParams;
            int _dataSeq = -1; // Unresolved

            public Template(SqlBuilder builder, string sql, Parameters parameters)
            {
                _initParams = parameters ?? new Dictionary<string, object>();
                _sql = sql;
                _builder = builder;
            }

            private static readonly Regex Regex = new Regex(@"{{2}.+?}{2}", RegexOptions.Compiled | RegexOptions.Multiline);

            void ResolveSql()
            {
                if (_dataSeq == _builder._seq) return;

                var p = _initParams.ToDictionary(x => x.Key, x => x.Value);
                _rawSql = _sql;

                foreach (var pair in _builder._data)
                {
                    var target = "{{" + pair.Key + "}}";
                    _rawSql = _rawSql.Replace(target, pair.Value.ResolveClauses(p), StringComparison.OrdinalIgnoreCase);
                }

                _parameters = p;

                // replace all that is left with empty
                _rawSql = Regex.Replace(_rawSql, "");

                _dataSeq = _builder._seq;
            }

            string _rawSql;
            private Dictionary<string, object> _parameters;

            public string RawSql { get { ResolveSql(); return _rawSql; } }
            public Dictionary<string, object> Parameters { get { ResolveSql(); return _parameters; } }
        }

        public Template AddTemplate(string sql, Parameters parameters)
        {
            return new Template(this,sql, parameters);
        }

        void AddClause(string name, string sql, Parameters parameters, string joiner, string prefix = "", string postfix = "")
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

        public SqlBuilder InnerJoin(string sql, Parameters parameters = null)
        {
            AddClause("innerjoin", sql, parameters, joiner: "\nINNER JOIN ", prefix: "\nINNER JOIN ", postfix: "\n");
            return this;
        }

        public SqlBuilder LeftJoin(string sql, Parameters parameters = null)
        {
            AddClause("leftjoin", sql, parameters, joiner: "\nLEFT JOIN ", prefix: "\nLEFT JOIN ", postfix: "\n");
            return this;
        }

        public SqlBuilder RightJoin(string sql, Parameters parameters = null)
        {
            AddClause("rightjoin", sql, parameters, joiner: "\nRIGHT JOIN ", prefix: "\nRIGHT JOIN ", postfix: "\n");
            return this;
        }

        public SqlBuilder Where(string sql, Parameters parameters = null)
        {
            AddClause("where", sql, parameters, " AND ", prefix: "WHERE ", postfix: "\n");
            return this;
        }

        public SqlBuilder OrderBy(string sql, Parameters parameters = null)
        {
            AddClause("orderby", sql, parameters, " , ", prefix: "ORDER BY ", postfix: "\n");
            return this;
        }

        public SqlBuilder Select(string sql, Parameters parameters = null)
        {
            AddClause("select", sql, parameters, " , ", prefix: "SELECT ", postfix: "\n");
            return this;
        }

        public SqlBuilder AddParameters(Parameters parameters)
        {
            AddClause("--parameters", sql: "", parameters: parameters, joiner: "");
            return this;
        }

        public SqlBuilder Join(string sql, Parameters parameters = null)
        {
            AddClause("join", sql, parameters, joiner: "\nJOIN ", prefix: "\nJOIN ", postfix: "\n");
            return this;
        }

        public SqlBuilder GroupBy(string sql, Parameters parameters = null)
        {
            AddClause("groupby", sql, parameters, joiner: " , ", prefix: "\nGROUP BY ", postfix: "\n");
            return this;
        }

        public SqlBuilder Having(string sql, Parameters parameters = null)
        {
            AddClause("having", sql, parameters, joiner: "\nAND ", prefix: "HAVING ", postfix: "\n");
            return this;
        }
    }
}

