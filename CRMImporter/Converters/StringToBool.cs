using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace CRMImporter.Converters
{
    public class StringToBool : IConverter
    {
        public StringToBool(string trueValue, string falseValue = null)
        {
            this.TrueValue = trueValue.ToLower();
            this.FalseValue = falseValue == null ? null : falseValue.ToLower();
        }

        private readonly string TrueValue;
        private readonly string FalseValue;

        public object Convert(object input, IOrganizationService service)
        {
            if (input == null)
            {
                return null;
            }
            string value = ((string)input).ToLower();
            if (this.FalseValue != null && !(value == TrueValue || value == FalseValue))
            {
                throw new Exception($"Strict convert. {value} is not a valid boolean convert value. Possible values {this.TrueValue}, {this.FalseValue}");
            }
            return value == this.TrueValue;

        }
    }
}
