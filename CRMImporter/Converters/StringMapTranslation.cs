using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace CRMImporter.Converters
{
    public class StringMapTranslation : IConverter
    {
        private readonly Dictionary<string, string> mapping;
        public StringMapTranslation(Dictionary<string, string> mapping)
        {
            this.mapping = mapping;
        }
        public object Convert(object input, IOrganizationService service)
        {
            string value = (string)input;
            if (!this.mapping.ContainsKey(value))
            {
                return null;
            }
            return this.mapping[value];
        }
    }
}
