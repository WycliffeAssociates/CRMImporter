using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace CRMImporter.ActionHandlers
{
    public class DefaultUpdateHandler : IActionHandler
    {
        public void Execute(Entity data, IOrganizationService service)
        {
            if (data.Attributes.Count > 0)
            {
                service.Update(data);
            }
        }
    }
}
