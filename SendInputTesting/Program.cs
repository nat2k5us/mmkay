using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendInputTesting
{
    using WindowsInput;
    using WindowsInput.Native;

    class Program
    {
        static KeyboardSimulator keyboardSimulator = new KeyboardSimulator(new InputSimulator());
        static void Main(string[] args)
        {
            ShoutHello();
            Console.WriteLine();
            Console.ReadKey();
        }


        public static void ShoutHello()
        {
            keyboardSimulator.KeyDown(VirtualKeyCode.VK_H);
            keyboardSimulator.KeyUp(VirtualKeyCode.VK_H);
            keyboardSimulator.KeyDown(VirtualKeyCode.VK_E);
            keyboardSimulator.KeyUp(VirtualKeyCode.VK_E);
            keyboardSimulator.KeyDown(VirtualKeyCode.VK_L);
            keyboardSimulator.KeyUp(VirtualKeyCode.VK_L);
            keyboardSimulator.KeyDown(VirtualKeyCode.VK_L);
            keyboardSimulator.KeyUp(VirtualKeyCode.VK_L);
            keyboardSimulator.KeyDown(VirtualKeyCode.VK_O);
            keyboardSimulator.KeyUp(VirtualKeyCode.VK_O);
            keyboardSimulator.KeyDown(VirtualKeyCode.ACCEPT);
            keyboardSimulator.KeyUp(VirtualKeyCode.ACCEPT);
        }
    }
}
