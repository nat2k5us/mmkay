using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistryHelperTestApp
{
    using RegistryHelper;

    public class Dummy 
    {
        readonly IRegistryHelper registryHelper = new RegistryHelper();
        public Dummy()
        {
            this.registryHelper.CheckRegistry("LocalMachine");
        }
    }
}
