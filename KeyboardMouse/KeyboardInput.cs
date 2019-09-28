namespace KeyboardMouse
{
    public static class KeyboardInput
    {
       public static void PressKey(byte keyCode)
        {
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;
            NativeMethods.keybd_event(keyCode, 0x45, KEYEVENTF_EXTENDEDKEY, 0);
            NativeMethods.keybd_event(keyCode, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }
    }
}
