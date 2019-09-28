using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindImageInImage
{
       public class StopAllProcessingEventArgs : EventArgs
        {
           public StopAllProcessingEventArgs(bool stopAllProcessing)
           {
               this.StopProcessing = stopAllProcessing;
           }

           public bool StopProcessing { get; set; }
        }
}
