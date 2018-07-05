using CRMImporter.Converters;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMImporterTests.Converters
{
    [TestClass]
    public class NullToStringTests
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
        public void TestConvert()
        {
            NullToString test = new NullToString();
            Assert.AreEqual("",test.Convert(null, service));
            test = new NullToString(" ");
            Assert.AreEqual(" ",test.Convert(null, service));
        }
    }
}
