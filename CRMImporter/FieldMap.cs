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
        public IConverter Convert;

        public FieldMap(string source, string target, IConverter converter = null)
        {
            this.SourceField = source;
            this.TargetField = target;
            this.Convert = converter;
        }
    }
}
