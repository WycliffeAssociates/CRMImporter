using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;
using FakeXrmEasy;
using CRMImporter.ActionHandlers;

namespace CRMImporterTests.ActionHandlers
{
    [TestClass]
    public class DefaultCreateHandlerTests
    {
        [TestMethod]
        public void Test()
        {
            string fieldName = "field";
            string fieldInitialValue = "value";
            XrmFakedContext context = new XrmFakedContext();
            IOrganizationService service = context.GetOrganizationService();
            DefaultCreateHandler handler = new DefaultCreateHandler();
            Entity initialEntity = new Entity("entity", Guid.NewGuid())
            {
                [fieldName] = fieldInitialValue
            };
            handler.Execute(initialEntity, service);
            Entity post = service.Retrieve(initialEntity.LogicalName, initialEntity.Id, new ColumnSet(fieldName));
            Assert.AreEqual(fieldInitialValue, post[fieldName]);
        }
    }
}
