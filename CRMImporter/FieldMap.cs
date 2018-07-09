using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMImporter
{
    public class FieldMap
    {
        public string SourceField;
        public string TargetField;
        public IConverter[] Convert;

        /// <summary>
        /// FieldMap constructor
        /// </summary>
        /// <param name="source">The field in your source data to match against</param>
        /// <param name="target">The field in CRM to match to</param>
        /// <param name="converter">A list of optional conversion functions</param>
        public FieldMap(string source, string target, params IConverter[] converter)
        {
            this.SourceField = source;
            this.TargetField = target;
            this.Convert = converter != null ? converter : new IConverter[0];
        }
    }
}
