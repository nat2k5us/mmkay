using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassWithConfig
{
    public class TestClass
    {
        public TestClass(string paramStr)
        {
            this.PropStr = paramStr;
        }

        public string PropStr { get; private set; }
    }
}
