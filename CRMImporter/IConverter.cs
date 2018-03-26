using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMImporter
{
    public interface IConverter
    {
        object Convert(object input, IOrganizationService service);
    }
}
