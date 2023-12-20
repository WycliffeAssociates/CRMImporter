using System;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using CRMImporter.Converters;
using FakeXrmEasy.Abstractions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace CRMImporterTests.Converters
{
    public class LookupValueTests
    {
        private IXrmFakedContext context;
        private IOrganizationService service;

        [SetUp]
        public void SetUp()
        {
            this.context = Utils.GetContext();
            this.service = this.context.GetOrganizationService();
        }

        [Test]
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

            ClassicAssert.AreEqual(lookup.Id, ((EntityReference)target.Convert(keyValue, this.service)).Id);
            ClassicAssert.AreEqual(null, (EntityReference)target.Convert(null, this.service));
            ClassicAssert.AreEqual(null, (EntityReference)target.Convert(missingValue, this.service));
        }
    }
}
