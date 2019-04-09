using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hummingbird.CustomTools
{
    public class JwtClaim
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public JwtClaimType ClaimType { get; set; }

        public Func<object> DefaultValue { get; set; }
    }
}
