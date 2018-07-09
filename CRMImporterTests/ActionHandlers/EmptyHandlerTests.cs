using CRMImporter.ActionHandlers;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;

namespace CRMImporterTests.ActionHandlers
{
    [TestClass]
    public class EmptyHandlerTests
    {
        [TestMethod]
        public void Test()
        {
            string fieldName = "field";
            string fieldInitialValue = "value";
            string changedValue = "othervalue";
            XrmFakedContext context = new XrmFakedContext();
            IOrganizationService service = context.GetOrganizationService();
            EmptyHandler handler = new EmptyHandler();
            Entity initialEntity = new Entity("entity", Guid.NewGuid())
            {
                [fieldName] = fieldInitialValue
            };
            context.Initialize(initialEntity);
            initialEntity[fieldName] = changedValue;
            handler.Execute(initialEntity, service);
            Entity post = service.Retrieve(initialEntity.LogicalName, initialEntity.Id, new ColumnSet(fieldName));
            Assert.AreEqual(fieldInitialValue, post[fieldName]);
        }
    }
}
