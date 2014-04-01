using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace UniqueNamespace
{
    public class SqlBuilderParams<TDbParams> : ISqlBuilderParams<IEnumerable<TDbParams>, TDbParams[]> where TDbParams : DbParameter
    {
        private readonly Dictionary<string, TDbParams> _params =
            new Dictionary<string, TDbParams>(StringComparer.OrdinalIgnoreCase);

        public void Expand(IEnumerable<TDbParams> parameters)
        {
            foreach (var p in parameters)
                _params[p.ParameterName] = p;
        }

        public TDbParams[] Materialize()
        {
            return _params.Values.ToArray();
        }
    }
}