using CRMImporter.ActionHandlers;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace CRMImporterTests.ActionHandlers
{
    public class EmptyHandlerTests
    {
        [Test]
        public void Test()
        {
            string fieldName = "field";
            string fieldInitialValue = "value";
            string changedValue = "othervalue";
            var context = Utils.GetContext();
            var service = context.GetOrganizationService();
            EmptyHandler handler = new EmptyHandler();
            Entity initialEntity = new Entity("entity", Guid.NewGuid())
            {
                [fieldName] = fieldInitialValue
            };
            context.Initialize(initialEntity);
            initialEntity[fieldName] = changedValue;
            handler.Execute(initialEntity, service);
            Entity post = service.Retrieve(initialEntity.LogicalName, initialEntity.Id, new ColumnSet(fieldName));
            ClassicAssert.AreEqual(fieldInitialValue, post[fieldName]);
        }
    }
}
