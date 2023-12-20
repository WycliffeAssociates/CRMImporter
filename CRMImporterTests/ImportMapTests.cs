using CRMImporter;
using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Metadata;
using CRMImporter.Converters;
using CRMImporter.ActionHandlers;
using FakeXrmEasy;
using FakeXrmEasy.Abstractions;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace CRMImporterTests
{
    public class ImportMapTests
    {
        IXrmFakedContext context;
        IOrganizationService service;

        [SetUp]
        public void SetUp()
        {
            context = Utils.GetContext();
            service = context.GetOrganizationService();
        }
        [Test]
        public void TestConstructor()
        {
            string entityname = "entity";
            FieldMap fieldMapping = new FieldMap("source", "target");
            ImportMap map = new ImportMap(entityname, fieldMapping);
            ClassicAssert.AreEqual(entityname, map.EntityName);
            ClassicAssert.AreEqual(fieldMapping, map.Key);
        }

        [Test]
        public void TestConstructorWithHandlers()
        {
            string entityname = "entity";
            IActionHandler createHandler = new EmptyHandler();
            IActionHandler updateHandler = new EmptyHandler();
            FieldMap fieldMapping = new FieldMap("source", "target");
            ImportMap map = new ImportMap(entityname, fieldMapping, createHandler, updateHandler); 
            ClassicAssert.AreEqual(map.EntityName, entityname);
            ClassicAssert.AreEqual(map.Key, fieldMapping);
            ClassicAssert.AreEqual(createHandler, map.CreateHandler);
            ClassicAssert.AreEqual(updateHandler, map.UpdateHandler);
        }

        [Test]
        public void ConvertToDictionaryTests()
        {
            string fieldValue = "Value";
            var result = ImportMap.ConvertToDictionary(new DummyClass { DummyField = fieldValue });
            ClassicAssert.AreEqual(true, result.ContainsKey("DummyField"));
            ClassicAssert.AreEqual(result["DummyField"], fieldValue);
        }

        [Test]
        public void ConvertValueTests()
        {
            FieldMap map = new FieldMap("source", "dest");
            string optionSetLabel = "label";
            int optionSetInt = 1;
            string booleanTrueLabel = "Yes";
            string booleanFalseLabel = "No";
            EntityReference entityRef = new EntityReference("entity", Guid.NewGuid());

            // Set up metadata
            PicklistAttributeMetadata optionSetMetadata = new PicklistAttributeMetadata()
            {
                OptionSet = new OptionSetMetadata(
                    new OptionMetadataCollection(
                        new List<OptionMetadata>() {
                            new OptionMetadata {
                                Value = optionSetInt,
                                Label = GenerateLabel(optionSetLabel)
                            }
                        }
                    )
                )
            };
            StatusAttributeMetadata statusMetadata = new StatusAttributeMetadata()
            {
                OptionSet = new OptionSetMetadata(
                    new OptionMetadataCollection(
                        new List<OptionMetadata>() {
                            new OptionMetadata {
                                Value = optionSetInt,
                                Label = GenerateLabel(optionSetLabel)
                            }
                        }
                    )
                )
            };
            BooleanAttributeMetadata booleanMetadata = new BooleanAttributeMetadata(
                new BooleanOptionSetMetadata(
                    new OptionMetadata(GenerateLabel(booleanTrueLabel), 0),
                    new OptionMetadata(GenerateLabel(booleanFalseLabel), 1)
                )
            );
            OptionSetValue optionSetValue = new OptionSetValue(optionSetInt);
            ClassicAssert.AreEqual(null, ImportMap.ConvertValue(null, map, new AttributeMetadata(), service));
            ClassicAssert.AreEqual(1, ImportMap.ConvertValue(1, map, new IntegerAttributeMetadata(), service));
            ClassicAssert.AreEqual(1, ImportMap.ConvertValue(1D, map, new IntegerAttributeMetadata(), service));

            ClassicAssert.AreEqual("string", ImportMap.ConvertValue("string", map, new StringAttributeMetadata(), service));
            ClassicAssert.AreEqual("1", ImportMap.ConvertValue(1, map, new StringAttributeMetadata(), service));

            ClassicAssert.AreEqual(1.0m, ImportMap.ConvertValue(1.0m, map, new DecimalAttributeMetadata(), service));
            ClassicAssert.AreEqual(1.0m, ImportMap.ConvertValue(1.0f, map, new DecimalAttributeMetadata(), service));
            ClassicAssert.AreEqual(1.0m, ImportMap.ConvertValue(1D, map, new DecimalAttributeMetadata(), service));
            ClassicAssert.AreEqual(1.0m, ImportMap.ConvertValue(1, map, new DecimalAttributeMetadata(), service));

            ClassicAssert.AreEqual(1.0, ImportMap.ConvertValue(1.0, map, new DoubleAttributeMetadata(), service));
            ClassicAssert.AreEqual(1.0, ImportMap.ConvertValue(1.0f, map, new DoubleAttributeMetadata(), service));

            ClassicAssert.AreEqual(optionSetInt, ((OptionSetValue)ImportMap.ConvertValue(optionSetLabel, map, optionSetMetadata, service)).Value);
            ClassicAssert.AreEqual(optionSetValue, ImportMap.ConvertValue(optionSetValue, map, new PicklistAttributeMetadata(), service));

            ClassicAssert.AreEqual(null, ImportMap.ConvertValue("UNKNOWNThing", map, optionSetMetadata, service));
            ClassicAssert.AreEqual(true, ImportMap.ConvertValue("Yes", map, booleanMetadata, service));
            ClassicAssert.AreEqual(true, ImportMap.ConvertValue(true, map, booleanMetadata, service));

            ClassicAssert.AreEqual(entityRef, ImportMap.ConvertValue(entityRef, map, new LookupAttributeMetadata(), service));
            ClassicAssert.AreEqual(optionSetInt, ((OptionSetValue)ImportMap.ConvertValue(optionSetLabel, map, statusMetadata, service)).Value);
            ClassicAssert.AreEqual(null, ImportMap.ConvertValue("UNKNOWNThing", map, statusMetadata, service));
        }

        [Test]
        public void TestMissingConversion()
        {
            FieldMap map = new FieldMap("source", "dest");
            Assert.Throws<Exception>(() => ImportMap.ConvertValue(1, map, new LookupAttributeMetadata(), service));
        }

        [Test]
        public void TestWithConverter()
        {
            string trueValue = "Y";
            FieldMap map = new FieldMap("source", "dest", new StringToBool(trueValue));
            ClassicAssert.AreEqual(true, ImportMap.ConvertValue( trueValue, map, new BooleanAttributeMetadata(), service));
        }

        [Test]
        public void TestWithNullConverterList()
        {
            FieldMap map = new FieldMap("source", "dest", null);
            ClassicAssert.AreEqual(true, ImportMap.ConvertValue( true, map, new BooleanAttributeMetadata(), service));
        }

        [Test]
        public void TestWithEmptyConverterList()
        {
            FieldMap map = new FieldMap("source", "dest", new IConverter[0]);
            ClassicAssert.AreEqual(true, ImportMap.ConvertValue( true, map, new BooleanAttributeMetadata(), service));
        }

        [Test]
        public void TestWithMultipleConverters()
        {
            string trueValue = "Yes";
            StringMapTranslation stringMapConverter = new StringMapTranslation(new Dictionary<string, string> { ["Yes"] = "Y" });
            StringToBool toBoolConverter = new StringToBool("Y");
            FieldMap map = new FieldMap("source", "dest", stringMapConverter, toBoolConverter);
            ClassicAssert.AreEqual(true, ImportMap.ConvertValue( trueValue, map, new BooleanAttributeMetadata(), service));
        }

        private Label GenerateLabel(string labelText)
        {
            return new Label
            {
                UserLocalizedLabel = new LocalizedLabel
                {
                    Label = labelText
                },
            };
        }
        public class DummyClass
        {
            public string DummyField
            {
                get; set;
            }
        }
    }
}
