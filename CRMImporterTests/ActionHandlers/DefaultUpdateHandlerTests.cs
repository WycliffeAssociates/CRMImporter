using CRMImporter.ActionHandlers;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace CRMImporterTests.ActionHandlers
{
    public class DefaultUpdateHandlerTests
    {
        [Test]
        public void Test()
        {
            string fieldName = "field";
            string fieldInitialValue = "value";
            string changedValue = "changedValue";
            var context = Utils.GetContext();
            IOrganizationService service = context.GetOrganizationService();
            DefaultUpdateHandler handler = new DefaultUpdateHandler();
            Entity initialEntity = new Entity("entity", Guid.NewGuid())
            {
                [fieldName] = fieldInitialValue
            };
            context.Initialize(initialEntity);
            initialEntity[fieldName] = changedValue;
            handler.Execute(initialEntity, service);
            Entity post = service.Retrieve(initialEntity.LogicalName, initialEntity.Id, new ColumnSet(fieldName));
            ClassicAssert.AreEqual(changedValue, post[fieldName]);
        }

        [Test]
        public void TestUpdateWithNoChanges()
        {
            string fieldName = "field";
            string fieldInitialValue = "value";
            string changedValue = "changedValue";
            DateTime lastModified = new DateTime(2011, 1, 1);
            var context = Utils.GetContext();
            IOrganizationService service = context.GetOrganizationService();
            DefaultUpdateHandler handler = new DefaultUpdateHandler();
            Entity initialEntity = new Entity("entity", Guid.NewGuid())
            {
                [fieldName] = fieldInitialValue,
                ["modifiedon"] = lastModified
            };
            context.Initialize(initialEntity);
            Entity changedEntity = new Entity("entity", initialEntity.Id);
            handler.Execute(changedEntity, service);
            Entity post = service.Retrieve(initialEntity.LogicalName, initialEntity.Id, new ColumnSet("modifiedon"));
            ClassicAssert.AreEqual(lastModified, post["modifiedon"]);
        }
    }
}
