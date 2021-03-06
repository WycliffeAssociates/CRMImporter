﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using CRMImporter.ActionHandlers;

namespace CRMImporter
{
    public class ImportMap
    {
        public string EntityName;
        public FieldMap Key;
        public List<FieldMap> Mapping;
        public IActionHandler CreateHandler;
        public IActionHandler UpdateHandler;

        /// <summary>
        /// ImportMap Constructor
        /// </summary>
        /// <param name="entity">Logical name of the entity to use in CRM</param>
        /// <param name="key">Mapping for the attribute that is used to match records in CRM</param>
        public ImportMap(string entity, FieldMap key, IActionHandler create = null, IActionHandler update = null)
        {
            this.EntityName = entity;
            this.Key = key;
            
            this.CreateHandler = create == null ? new DefaultCreateHandler() : create;
            this.UpdateHandler = update == null ? new DefaultUpdateHandler() : update;
        }

        /// <summary>
        /// Convert an object to a dictionary
        /// </summary>
        /// <param name="input">The object to convert</param>
        /// <returns>A dictionary of string, object that expresses the data in the object</returns>
        public static Dictionary<string, object> ConvertToDictionary(object input)
        {
            Dictionary<string, object> output = new Dictionary<string, object>();
            foreach (var field in input.GetType().GetProperties())
            {
                output.Add(field.Name, field.GetValue(input));
            }
            return output;
        }

        /// <summary>
        /// Import data into CRM
        /// </summary>
        /// <param name="service">Connection to CRM (IOrganizationService)</param>
        /// <param name="data">Data to import</param>
        /// <param name="callback">Optional progress callback</param>
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
                    ConvertValue(row[Key.SourceField], Key, metadata.Attributes.First(m => m.LogicalName == Key.TargetField), service));
                query.ColumnSet = new ColumnSet(Mapping.Select(i => i.TargetField).ToArray());
                var result = service.RetrieveMultiple(query);
                if (result.Entities.Count == 0)
                {
                    // Create new
                    this.CreateHandler.Execute(this.CreateEntityToCreate(row, metadata, service), service);
                }
                else
                {
                    this.UpdateHandler.Execute(this.CreateEntityToUpdate(result.Entities[0], row, metadata, service), service);
                }
                // Call progress callback if it exists
                callback?.Invoke(count, total);
            }
        }
        /// <summary>
        /// Import a list of objects into CRM
        /// </summary>
        /// <param name="service">Connection to CRM (IOrganizationService)</param>
        /// <param name="data">Data to import</param>
        /// <param name="callback">Optional progress callback</param>
        public void Import<T>(IOrganizationService service, List<T> data, Action<int, int> callback = null)
        {
            List<Dictionary<string, object>> convertedData = new List<Dictionary<string, object>>();
            foreach (var i in data)
            {
                convertedData.Add(ConvertToDictionary(i));
            }
            Import(service, convertedData, callback);
        }

        /// <summary>
        /// Import a single object into CRM
        /// </summary>
        /// <param name="service">Connection to CRM (IOrganizationService)</param>
        /// <param name="data">Data to import</param>
        /// <param name="callback">Optional progress callback</param>
        public void Import(IOrganizationService service, object data, Action<int, int> callback = null)
        {
            Import(service, new List<Dictionary<string, object>>() { ConvertToDictionary(data) }, callback);
        }


        private  Entity CreateEntityToUpdate(Entity current, Dictionary<string, object> data, EntityMetadata meta, IOrganizationService service)
        {
            Entity target = new Entity(this.EntityName, current.Id);
            foreach (FieldMap field in this.Mapping)
            {
                if (!data.ContainsKey(field.SourceField))
                {
                    throw new KeyNotFoundException($"Key {field.SourceField} doesn't exist in the source data");
                }
                object tmp = ConvertValue(data[field.SourceField], field, meta.Attributes.First(f => f.LogicalName == field.TargetField), service);
                object source = current.Contains(field.TargetField) ? current[field.TargetField] : null;
                if (source != tmp)
                {
                    target[field.TargetField] = tmp;
                }
            }
            return target;
        }
        private Entity CreateEntityToCreate(Dictionary<string, object> data, EntityMetadata meta, IOrganizationService service)
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
                object tmp = ConvertValue(data[item.SourceField], item, field, service);
                if (tmp != null)
                {
                    target[item.TargetField] = tmp;
                }
            }
            return target;
        }

        public static object ConvertValue(object input, FieldMap map, AttributeMetadata field, IOrganizationService service)
        {
            if (map.Convert.Length != 0)
            {
                foreach (var converter in map.Convert)
                {
                    input = converter.Convert(input, service);
                }
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

            if (input is Single && field.AttributeType == AttributeTypeCode.Decimal)
            {
                return (decimal)((Single)input);
            }

            if (input is string && field.AttributeType == AttributeTypeCode.Picklist)
            {
                PicklistAttributeMetadata picklist = (PicklistAttributeMetadata)field;
                var tmp = picklist.OptionSet.Options.FirstOrDefault(o => o.Label.UserLocalizedLabel.Label == (string)input);
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

            if (input is float && field.AttributeType == AttributeTypeCode.Double)
            {
                return Convert.ToDouble((float)input);
            }

            if (input is double && field.AttributeType == AttributeTypeCode.Double)
            {
                return (double)input;
            }

            if (input is string && field.AttributeType == AttributeTypeCode.Status)
            {
                StatusAttributeMetadata picklist = (StatusAttributeMetadata)field;
                var tmp = picklist.OptionSet.Options.FirstOrDefault(o => o.Label.UserLocalizedLabel.Label == (string)input);
                if (tmp != null)
                {
                    return new OptionSetValue(tmp.Value.Value);
                }
                return null;
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
