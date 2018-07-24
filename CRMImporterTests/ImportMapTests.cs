using CRMImporter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Metadata;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using CRMImporter.Converters;
using CRMImporter.ActionHandlers;

namespace CRMImporterTests
{
    [TestClass]
    public class ImportMapTests
    {
        XrmFakedContext context;
        IOrganizationService service;

        [TestInitialize]
        public void SetUp()
        {
            context = new XrmFakedContext();
            service = context.GetOrganizationService();
        }
        [TestMethod]
        public void TestConstructor()
        {
            string entityname = "entity";
            FieldMap fieldMapping = new FieldMap("source", "target");
            ImportMap map = new ImportMap(entityname, fieldMapping);
            Assert.AreEqual(entityname, map.EntityName);
            Assert.AreEqual(fieldMapping, map.Key);
        }

        [TestMethod]
        public void TestConstructorWithHandlers()
        {
            string entityname = "entity";
            IActionHandler createHandler = new EmptyHandler();
            IActionHandler updateHandler = new EmptyHandler();
            FieldMap fieldMapping = new FieldMap("source", "target");
            ImportMap map = new ImportMap(entityname, fieldMapping, createHandler, updateHandler);
            Assert.AreEqual(entityname, map.EntityName);
            Assert.AreEqual(fieldMapping, map.Key);
            Assert.AreEqual(createHandler, map.CreateHandler);
            Assert.AreEqual(updateHandler, map.UpdateHandler);
        }

        [TestMethod]
        public void ConvertToDictionaryTests()
        {
            string fieldValue = "Value";
            var result = ImportMap.ConvertToDictionary(new DummyClass { DummyField = fieldValue });
            Assert.IsTrue(result.ContainsKey("DummyField"));
            Assert.AreEqual(result["DummyField"], fieldValue);
        }

        [TestMethod]
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
            Assert.AreEqual(null, ImportMap.ConvertValue(null, map, new AttributeMetadata(), service));
            Assert.AreEqual(1, ImportMap.ConvertValue(1, map, new IntegerAttributeMetadata(), service));
            Assert.AreEqual(1, ImportMap.ConvertValue(1D, map, new IntegerAttributeMetadata(), service));

            Assert.AreEqual("string", ImportMap.ConvertValue("string", map, new StringAttributeMetadata(), service));
            Assert.AreEqual("1", ImportMap.ConvertValue(1, map, new StringAttributeMetadata(), service));

            Assert.AreEqual(1.0m, ImportMap.ConvertValue(1.0m, map, new DecimalAttributeMetadata(), service));
            Assert.AreEqual(1.0m, ImportMap.ConvertValue(1.0f, map, new DecimalAttributeMetadata(), service));
            Assert.AreEqual(1.0m, ImportMap.ConvertValue(1D, map, new DecimalAttributeMetadata(), service));
            Assert.AreEqual(1.0m, ImportMap.ConvertValue(1, map, new DecimalAttributeMetadata(), service));

            Assert.AreEqual(1.0, ImportMap.ConvertValue(1.0, map, new DoubleAttributeMetadata(), service));
            Assert.AreEqual(1.0, ImportMap.ConvertValue(1.0f, map, new DoubleAttributeMetadata(), service));

            Assert.AreEqual(optionSetInt, ((OptionSetValue)ImportMap.ConvertValue(optionSetLabel, map, optionSetMetadata, service)).Value);
            Assert.AreEqual(optionSetValue, ImportMap.ConvertValue(optionSetValue, map, new PicklistAttributeMetadata(), service));

            Assert.AreEqual(null, ImportMap.ConvertValue("UNKNOWNThing", map, optionSetMetadata, service));
            Assert.AreEqual(true, ImportMap.ConvertValue("Yes", map, booleanMetadata, service));
            Assert.AreEqual(true, ImportMap.ConvertValue(true, map, booleanMetadata, service));

            Assert.AreEqual(entityRef, ImportMap.ConvertValue(entityRef, map, new LookupAttributeMetadata(), service));
            Assert.AreEqual(optionSetInt, ((OptionSetValue)ImportMap.ConvertValue(optionSetLabel, map, statusMetadata, service)).Value);
            Assert.AreEqual(null, ImportMap.ConvertValue("UNKNOWNThing", map, statusMetadata, service));
        }

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void TestMissingConversion()
        {
            FieldMap map = new FieldMap("source", "dest");
            ImportMap.ConvertValue(1, map, new LookupAttributeMetadata(), service);
        }

        [TestMethod]
        public void TestWithConverter()
        {
            string trueValue = "Y";
            FieldMap map = new FieldMap("source", "dest", new StringToBool(trueValue));
            Assert.AreEqual(true, ImportMap.ConvertValue( trueValue, map, new BooleanAttributeMetadata(), service));
        }

        [TestMethod]
        public void TestWithNullConverterList()
        {
            FieldMap map = new FieldMap("source", "dest", null);
            Assert.AreEqual(true, ImportMap.ConvertValue( true, map, new BooleanAttributeMetadata(), service));
        }

        [TestMethod]
        public void TestWithEmptyConverterList()
        {
            FieldMap map = new FieldMap("source", "dest", new IConverter[0]);
            Assert.AreEqual(true, ImportMap.ConvertValue( true, map, new BooleanAttributeMetadata(), service));
        }

        [TestMethod]
        public void TestWithMultipleConverters()
        {
            string trueValue = "Yes";
            StringMapTranslation stringMapConverter = new StringMapTranslation(new Dictionary<string, string> { ["Yes"] = "Y" });
            StringToBool toBoolConverter = new StringToBool("Y");
            FieldMap map = new FieldMap("source", "dest", stringMapConverter, toBoolConverter);
            Assert.AreEqual(true, ImportMap.ConvertValue( trueValue, map, new BooleanAttributeMetadata(), service));
        }

        [TestMethod]
        public void TestCreateForUpdate()
        {

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
