using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using CRMImporter.Converters;

namespace CRMImporterTests.Converters
{
    [TestClass]
    public class LookupValueTests
    {
        private XrmFakedContext context;
        private IOrganizationService service;

        [TestInitialize]
        public void SetUp()
        {
            this.context = new XrmFakedContext();
            this.service = this.context.GetOrganizationService();
        }

        [TestMethod]
        public void TestLookups()
        {
            string keyValue = "target";
            string missingValue = "small sheep";
            Entity lookup = new Entity("entity", Guid.NewGuid())
            {
                ["key"] = keyValue
            };

            LookupValue target = new LookupValue("entity", "key");

            this.context.Initialize(lookup);

            Assert.AreEqual(lookup.Id, ((EntityReference)target.Convert(keyValue, this.service)).Id);
            Assert.AreEqual(null, (EntityReference)target.Convert(null, this.service));
            Assert.AreEqual(null, (EntityReference)target.Convert(missingValue, this.service));
        }
    }
}
