using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CRMImporter.Converters
{
    public class LookupValue : IConverter
    {
        private string EntityName;
        private string LookupField;

        public LookupValue(string entity, string field)
        {
            this.EntityName = entity;
            this.LookupField = field;
        }
        public object Convert(object input, IOrganizationService service)
        {
            if (input == null)
            {
                return null;
            }
            QueryExpression query = new QueryExpression(this.EntityName);
            query.Criteria.AddCondition(this.LookupField, ConditionOperator.Equal, input);
            var result = service.RetrieveMultiple(query);
            if (result.Entities.Count == 0)
            {
                return null;
            }
            return result.Entities[0].ToEntityReference();
        }
    }
}
