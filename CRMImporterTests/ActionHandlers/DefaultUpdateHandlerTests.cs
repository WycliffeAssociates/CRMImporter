using CRMImporter.ActionHandlers;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMImporterTests.ActionHandlers
{
    [TestClass]
    public class DefaultUpdateHandlerTests
    {
        [TestMethod]
        public void Test()
        {
            string fieldName = "field";
            string fieldInitialValue = "value";
            string changedValue = "changedValue";
            XrmFakedContext context = new XrmFakedContext();
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
            Assert.AreEqual(changedValue, post[fieldName]);
        }

        [TestMethod]
        public void TestUpdateWithNoChanges()
        {
            string fieldName = "field";
            string fieldInitialValue = "value";
            string changedValue = "changedValue";
            DateTime lastModified = new DateTime(2011, 1, 1);
            XrmFakedContext context = new XrmFakedContext();
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
            Assert.AreEqual(lastModified, post["modifiedon"]);
        }
    }
}
