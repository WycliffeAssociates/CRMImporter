using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace CRMImporter
{
    public class ImportMap
    {
        public string EntityName;
        public FieldMap Key;
        public List<FieldMap> Mapping;

        public ImportMap(string entity, FieldMap key)
        {
            this.EntityName = entity;
            this.Key = key;
        }

        public void Import(IOrganizationService service, List<Dictionary<string, object>> data, Action<int,int> callback = null)
        {
            EntityMetadata metadata = GetMetaData(service);
            int count = 0;
            int total = data.Count;
            foreach (var row in data)
            {
                count++;
                QueryExpression query = new QueryExpression(this.EntityName);
                query.Criteria.AddCondition(Key.TargetField,
                    ConditionOperator.Equal,
                    this.ConvertValue(row[Key.SourceField], Key, metadata.Attributes.First(m => m.LogicalName == Key.TargetField), service));
                query.ColumnSet = new ColumnSet(Mapping.Select(i => i.TargetField).ToArray());
                var result = service.RetrieveMultiple(query);
                if (result.Entities.Count == 0)
                {
                    // Create new
                    this.CreateEntity(row, metadata, service);
                }
                else
                {
                    this.UpdateEntity(result.Entities[0], row, metadata, service);
                }
                // Call progress callback if it exists
                callback?.Invoke(count, total);
            }
        }

        private void UpdateEntity(Entity current, Dictionary<string, object> data, EntityMetadata meta, IOrganizationService service)
        {
            Entity target = new Entity(this.EntityName, current.Id);
            foreach (FieldMap field in this.Mapping)
            {
                if (!data.ContainsKey(field.SourceField))
                {
                    throw new KeyNotFoundException($"Key {field.SourceField} doesn't exist in the source data");
                }
                object tmp = this.ConvertValue(data[field.SourceField], field, meta.Attributes.First(f => f.LogicalName == field.TargetField), service);
                if (!current.Contains(field.TargetField) || current[field.TargetField] != tmp)
                {
                    target[field.TargetField] = tmp;
                }
            }
            if (target.Attributes.Count > 0)
            {
                service.Update(target);
            }
        }
        private void CreateEntity(Dictionary<string, object> data, EntityMetadata meta, IOrganizationService service)
        {
            Entity target = new Entity(this.EntityName);
            // Set primary field
            foreach (var item in this.Mapping)
            {
                if (!data.ContainsKey(item.SourceField))
                {
                    throw new KeyNotFoundException($"Key {item.SourceField} doesn't exist in the source data");
                }
                AttributeMetadata field = meta.Attributes.First(f => f.LogicalName == item.TargetField);
                target[item.TargetField] = this.ConvertValue(data[item.SourceField], item, field, service);
            }
            service.Create(target);
        }

        private object ConvertValue(object input, FieldMap map, AttributeMetadata field, IOrganizationService service)
        {
            if (map.Convert != null)
            {
                input = map.Convert.Convert(input, service);
            }

            if (input == null)
            {
                return null;
            }

            if (input is int && field.AttributeType == AttributeTypeCode.Integer)
            {
                return input;
            }

            if (input is double && field.AttributeType == AttributeTypeCode.Integer)
            {
                return (int)((double)input);
            }

            if (input is string && field.AttributeType == AttributeTypeCode.String)
            {
                return input;
            }

            if (input is EntityReference && field.AttributeType == AttributeTypeCode.Lookup)
            {
                return input;
            }

            if (input is OptionSetValue && field.AttributeType == AttributeTypeCode.Picklist)
            {
                return input;
            }

            if (input is decimal && field.AttributeType == AttributeTypeCode.Decimal)
            {
                return input;
            }

            if (input is double && field.AttributeType == AttributeTypeCode.Decimal)
            {
                return Convert.ToDecimal(input);
            }

            if (input is int && field.AttributeType == AttributeTypeCode.Decimal)
            {
                return (decimal)((int)input);
            }

            if (input is string && field.AttributeType == AttributeTypeCode.Picklist)
            {
                PicklistAttributeMetadata picklist = (PicklistAttributeMetadata)field;
                var tmp = picklist.OptionSet.Options.First(o => o.Label.UserLocalizedLabel.Label == (string)input);
                if (tmp != null)
                {
                    return new OptionSetValue(tmp.Value.Value);
                }
                return null;
            }

            if (field.AttributeType == AttributeTypeCode.String)
            {
                return input.ToString();
            }

            if (input is string && field.AttributeType == AttributeTypeCode.Boolean)
            {
                BooleanAttributeMetadata boolMetadata = (BooleanAttributeMetadata)field;
                return boolMetadata.OptionSet.TrueOption.Label.UserLocalizedLabel.Label == (string)input;
            }

            if (input is bool && field.AttributeType == AttributeTypeCode.Boolean)
            {
                return input;
            }


            throw new Exception($"No conversion for field {field.LogicalName} input: {input.GetType().Name} output: {field.AttributeTypeName.Value}");
        }

        public EntityMetadata GetMetaData(IOrganizationService service)
        {
            RetrieveEntityRequest request = new RetrieveEntityRequest()
            {
                LogicalName = this.EntityName,
                EntityFilters = EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships
            };
            RetrieveEntityResponse response = (RetrieveEntityResponse)service.Execute(request);
            return response.EntityMetadata;
        }
    }
}
