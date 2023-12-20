using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRMImporter.Converters;
using FakeXrmEasy;
using FakeXrmEasy.Abstractions;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace CRMImporterTests.Converters
{
    public class StringToBoolTests
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
        public void TestDefault()
        {
            string trueValue = "Yes";
            string falseValue = "No";
            StringToBool target = new StringToBool(trueValue);
            ClassicAssert.AreEqual(true, target.Convert(trueValue, this.service));
            ClassicAssert.AreEqual(false, target.Convert(falseValue, this.service));
        }

        [Test]
        public void TestStrict()
        {
            string trueValue = "Yes";
            string falseValue = "No";
            StringToBool target = new StringToBool(trueValue, falseValue);
            ClassicAssert.AreEqual(true, target.Convert(trueValue, this.service));
            ClassicAssert.AreEqual(false, target.Convert(falseValue, this.service));
        }

        [Test]
        public void TestStrictMissingValue()
        {
            string trueValue = "Yes";
            string falseValue = "No";
            string missingValue = "A small sheep";
            StringToBool target = new StringToBool(trueValue, falseValue);
            Assert.Throws<Exception>(() => target.Convert(missingValue, this.service));
        }
    }
}
