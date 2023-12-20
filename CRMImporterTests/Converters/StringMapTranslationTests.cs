using CRMImporter.Converters;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace CRMImporterTests.Converters
{
    public class StringMapTranslationTests
    {
        [Test]
        public void Test()
        {
            var context = Utils.GetContext();
            IOrganizationService service = context.GetOrganizationService();
            string original = "Original";
            string converted = "New";
            StringMapTranslation converter = new StringMapTranslation(new Dictionary<string, string>
            {
                [original] = converted
            });
            ClassicAssert.AreEqual(converted, converter.Convert(original, service));
            ClassicAssert.AreEqual(null, converter.Convert("UNKNOWN", service));
        }
    }
}
