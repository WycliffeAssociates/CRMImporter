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
    public class StringMapTranslationTests
    {
        [TestMethod]
        public void Test()
        {
            XrmFakedContext context = new XrmFakedContext();
            IOrganizationService service = context.GetOrganizationService();
            string original = "Original";
            string converted = "New";
            StringMapTranslation converter = new StringMapTranslation(new Dictionary<string, string>
            {
                [original] = converted
            });
            Assert.AreEqual(converted, converter.Convert(original, service));
            Assert.AreEqual(null, converter.Convert("UNKNOWN", service));
        }
    }
}
