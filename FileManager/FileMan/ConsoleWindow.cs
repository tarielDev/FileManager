using System.Runtime.InteropServices;

namespace FileMan
{

    public static class ConsoleWindow
    {
        private const int MF_BYCOMMAND = 0x00000000;
        //private const int SC_CLOSE = 0xF060;
        //private const int SC_MINIMIZE = 0xF020;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        private static IntPtr handle = GetConsoleWindow();
        private static IntPtr sysMenu = GetSystemMenu(handle, false);

        public static void DisableConsoleSizingAndMinimizing()
        {
            if (handle != IntPtr.Zero)
            {
                //DeleteMenu(sysMenu, SC_CLOSE, MF_BYCOMMAND);
                //DeleteMenu(sysMenu, SC_MINIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }
        }

        public static void DrawConsole(string dir, int x, int y, int width, int height)
        {
            DrawWindow(x, y, width, height);
            Console.SetCursorPosition(x + 2, y + height / 2);
            Console.Write($"{dir}");
        }

        public static void DrawWindow(int x, int y, int width, int height)
        {
            // header
            Console.SetCursorPosition(x, y);
            Console.Write("╔");
            for (int i = 0; i < width - 2; i++)
            {
                Console.Write("═");
            }
            Console.Write("╗");

            // window
            Console.SetCursorPosition(x, y + 1);
            for (int i = 0; i < height - 2; i++)
            {
                Console.SetCursorPosition(x, y + 1 + i);
                Console.Write("║");

                for (int j = x + 1; j < x + width - 1; j++)
                {
                    Console.Write(" ");
                }

                Console.Write("║");
            }

            //footer
            Console.SetCursorPosition(x, y + height - 1);
            Console.Write("╚");
            for (int i = 0; i < width - 2; i++)
            {
                Console.Write("═");
            }
            Console.Write("╝");
            Console.SetCursorPosition(x, y);

        }
    }

}
