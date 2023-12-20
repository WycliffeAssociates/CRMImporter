using Microsoft.Xrm.Sdk;
using System;
using Microsoft.Xrm.Sdk.Query;
using CRMImporter.ActionHandlers;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace CRMImporterTests.ActionHandlers
{
    public class DefaultCreateHandlerTests
    {
        [Test]
        public void Test()
        {
            string fieldName = "field";
            string fieldInitialValue = "value";
            var context = Utils.GetContext();
            IOrganizationService service = context.GetOrganizationService();
            DefaultCreateHandler handler = new DefaultCreateHandler();
            Entity initialEntity = new Entity("entity", Guid.NewGuid())
            {
                [fieldName] = fieldInitialValue
            };
            handler.Execute(initialEntity, service);
            Entity post = service.Retrieve(initialEntity.LogicalName, initialEntity.Id, new ColumnSet(fieldName));
            ClassicAssert.AreEqual(fieldInitialValue, post[fieldName]);
        }
    }
}
