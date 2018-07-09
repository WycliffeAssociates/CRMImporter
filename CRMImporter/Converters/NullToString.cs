using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace CRMImporter.Converters
{
    public class NullToString : IConverter
    {
        private readonly string SubstituteValue;
        public NullToString(string substituteValue = "")
        {
            SubstituteValue = substituteValue;
        }
        public object Convert(object input, IOrganizationService service)
        {
            return input == null ? SubstituteValue : input;
        }
    }
}
