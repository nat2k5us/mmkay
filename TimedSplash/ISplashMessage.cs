using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimedSplash
{
    using System.Drawing;

    public interface ISplashMessage
    {
        TimedSpalshScreen SplashScreen { get; set; }

        void ShowSplashMessage(Point point, string message, int showForMilliseconds);

        void UpdateSplashMessage(string message);

        void CloseSplash();

    }
}
