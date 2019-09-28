namespace PtcAdProcessor
{
    using System;

    public class CaptureCatImageEventArgs : EventArgs
    {
        public bool CaptureCatImage { get; set; }

        public CaptureCatImageEventArgs(bool captureCatImage)
        {
            this.CaptureCatImage = captureCatImage;
        }
    }
}