

namespace Transit.Business.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;

    using Transit.Data.Repository;

    public static class MefLoader
    {
        public static CompositionContainer Init()
        {
            AggregateCatalog catalog = new AggregateCatalog();
          //  catalog.Catalogs.Add(new AssemblyCatalog(typeof(AccountRepository).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(SignRepository).Assembly));

            CompositionContainer compositionContainer = new CompositionContainer(catalog);
            return compositionContainer;
        }
    }
}
