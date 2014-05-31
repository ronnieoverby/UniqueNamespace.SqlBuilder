using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace UniqueNamespace
{
    public class SortExpression : IEquatable<SortExpression>
    {
        private string _name;

        public bool Descending { get; set; }
        public string Name
        {
            get { return _name; }
            set
            {
                Validate(value);
                _name = value;
            }
        }

        internal string UnescapedName { get { return Name.UnescapeColumnName(); } }


        private static void Validate(string s)
        {
            // acceptable forms:
            // SomeName (alphanumeric)
            // [Some Name] (alphanumeric/whitespace betweens enclosing characters

            s = s.Trim();

            if (s.All(char.IsLetterOrDigit))
                return;

            var encs = new[]
            {
                new[] {"[", "]"}, // sql server, access, sybase
                new[] {"`"}, // mysql
                new[] {"\""} // oracle, postgres, sqlite, db2
            };

            if (encs.Any(
                    e => s.StartsWith(e[0])
                        && s.EndsWith(e.ElementAtOrDefault(1) ?? e[0])
                        && s.Skip(1).Take(s.Length - 2).All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))))
                return;

            throw new InvalidSortExpressionNameException(s);
        }

        public SortExpression()
        {
            // for model binding
        }

        public SortExpression(string name, bool descending = false)
        {
            if (name == null) throw new ArgumentNullException("name");
            Name = name;
            Descending = @descending;
        }

        public static SortExpression[] Parse(string s)
        {
            if (s == null) throw new ArgumentNullException("s");

            var sortExpressions = from line in s.ReadLines()
                                  from part in line.Split(",;".ToCharArray())
                                  where !part.IsNullOrWhiteSpace()
                                  select (SortExpression)part;

            return sortExpressions.ToArray();
        }

        public override string ToString()
        {
            return Name + (Descending ? " DESC" : " ASC");
        }

        public static implicit operator string(SortExpression sort)
        {
            return sort == null ? null : sort.ToString();
        }

        public static implicit operator SortExpression(string s)
        {
            if (s == null)
                return null;

            s = s.Trim();
            var ak = new[] { " asc", " ascending" };
            var dk = new[] { " desc", " descending" };
            var k = ak.Concat(dk).FirstOrDefault(x => s.EndsWith(x, StringComparison.OrdinalIgnoreCase));

            if (k == null)
                return new SortExpression(s.Trim());

            var desc = dk.Contains(k);
            var name = s.Substring(0, s.Length - k.Length).Trim();
            return new SortExpression(name, desc);
        }

        public bool Equals(SortExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(UnescapedName, other.UnescapedName) && Descending.Equals(other.Descending);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SortExpression) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (UnescapedName.GetHashCode() * 397) ^ Descending.GetHashCode();
            }
        }

        public static bool operator ==(SortExpression left, SortExpression right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SortExpression left, SortExpression right)
        {
            return !Equals(left, right);
        }
    }

    [Serializable]
    public class InvalidSortExpressionNameException : Exception
    {
        public InvalidSortExpressionNameException(string name)
            : base("Invalid sort expression name: " + name)
        {
        }
    }

    public static class SortExpressionExtensions
    {
        internal static string UnescapeColumnName(this string s)
        {
            return string.Concat(
                s.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                    .Select(x => x.ToString(CultureInfo.InvariantCulture))
                    .ToArray()
                ).Trim();
        }

        public static IEnumerable<SortExpression> Whitelist(this IEnumerable<SortExpression> expressions, params string[] names)
        {
            return
                expressions.Where(
                    exp =>
                        names.Select(x => x.UnescapeColumnName())
                            .Contains(exp.UnescapedName, StringComparer.OrdinalIgnoreCase));
        }

        public static string Join(this IEnumerable<SortExpression> expressions)
        {
            if (expressions == null) throw new ArgumentNullException("expressions");
            return string.Join(", ", expressions.Where(x => x != null).Select(x => x.ToString()).ToArray());
        }
    }
}
