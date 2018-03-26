using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRMImporter.Converters;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;

namespace CRMImporterTests.Converters
{
    [TestClass]
    public class StringToBoolTests
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
        public void TestDefault()
        {
            string trueValue = "Yes";
            string falseValue = "No";
            StringToBool target = new StringToBool(trueValue);
            Assert.AreEqual(true, target.Convert(trueValue, this.service));
            Assert.AreEqual(false, target.Convert(falseValue, this.service));
        }

        [TestMethod]
        public void TestStrict()
        {
            string trueValue = "Yes";
            string falseValue = "No";
            StringToBool target = new StringToBool(trueValue, falseValue);
            Assert.AreEqual(true, target.Convert(trueValue, this.service));
            Assert.AreEqual(false, target.Convert(falseValue, this.service));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestStrictMissingValue()
        {
            string trueValue = "Yes";
            string falseValue = "No";
            string missingValue = "A small sheep";
            StringToBool target = new StringToBool(trueValue, falseValue);
            target.Convert(missingValue, this.service);
        }
    }
}
