using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace CRMImporter.ActionHandlers
{
    public class EmptyHandler : IActionHandler
    {
        public void Execute(Entity data, IOrganizationService service)
        {
            // This is just a dummy handler it shouldn't actually do anything
        }
    }
}
