using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsoft.Sample.RowLevelSecurity
{
    public interface ISecuredByTenant
    {
        Guid? SecuredByTenantId { get; set; }
    }
}
