namespace NetworkLibTestConsoleApp
{
    using System;

    public class UserMessages
    {
        public static void HelpKeysMessage()
        {
            Console.WriteLine("Press Esc to Quit this program ");
            Console.WriteLine("Press S to Change Static IP ");
            Console.WriteLine("Press D to Change to Dhcp ");
            Console.WriteLine("Press E to Show all Nics ");
            Console.WriteLine("Press V for Verbose Info on all Nics ");
            Console.WriteLine("Press I to Show Internet Interface");
            Console.WriteLine("Press G to Change the Gateway ");
            Console.WriteLine("Press N (Names) to Show All Nic Names ");
            Console.WriteLine("Press M (Main Menu) to Choose Different Network Adaptor ");
        }

        public static void WelcomeMessage()
        {
            Console.WriteLine("========= Welcome to Windows Network Management Tool ==========");
            Console.WriteLine("Select a Network Adaptor, For example, for the network adaptor ");
            Console.WriteLine("1. Microsoft Hosted Network Virtual Adapter, you would press 1");
        }
    }
}