namespace PtcAdProcessor
{
    using System.Drawing;
    using System.Windows.Forms;

    public static class DisplayIndex
    {
        
        static DisplayIndex()
        {
            PrimaryX = 0;
            PrimaryY = 0;
            PrimaryWidth = 1920;
            PrimaryHeight = 1080;
        }

        public static Rectangle GetDisplayRectangle(int idx = 0)
        {
            if (idx == 0) return Screen.PrimaryScreen.Bounds;
            if (idx == 1) return GetRightOfPrimary();
            if (idx == -1) return GetLeftOfPrimary();

            return Screen.PrimaryScreen.Bounds;
        }

        public static int PrimaryX { get; set; }
        public static int PrimaryY { get; set; }

        public static int PrimaryWidth { get; set; }

        public static int PrimaryHeight { get; set; }

        public static Rectangle GetLeftOfPrimary()
        {
            Rectangle rect = new Rectangle(PrimaryX,PrimaryY,PrimaryWidth,PrimaryHeight);
            rect.Offset(new Point(-PrimaryWidth, 0));
            return rect;
        }

       
        public static Rectangle GetLeftOf(Rectangle rectangle)
        {

            rectangle.Offset(new Point(-PrimaryWidth, 0));
            return rectangle;
        }

        public static Rectangle GetRightOfPrimary()
        {
            Rectangle rect = new Rectangle(PrimaryX, PrimaryY, PrimaryWidth, PrimaryHeight);
            rect.Offset(new Point(PrimaryWidth, 0));
            return rect;
        }

        public static Rectangle GetRightOf(Rectangle rectangle)
        {

            rectangle.Offset(new Point(PrimaryWidth, 0));
            return rectangle;
        }

    }
}