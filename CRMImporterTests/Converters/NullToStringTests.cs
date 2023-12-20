using CRMImporter.Converters;
using FakeXrmEasy;
using NUnit.Framework;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeXrmEasy.Abstractions;
using NUnit.Framework.Legacy;

namespace CRMImporterTests.Converters
{
    public class NullToStringTests
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
        public void TestConvert()
        {
            NullToString test = new NullToString();
            ClassicAssert.AreEqual("",test.Convert(null, service));
            test = new NullToString(" ");
            ClassicAssert.AreEqual(" ",test.Convert(null, service));
        }
    }
}