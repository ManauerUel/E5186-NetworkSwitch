using Mono.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace E5186_NetworkSwitch
{
    class Program
    {

        // **HACK** This lines are a hack because of console and Window App at once
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        [STAThread]
        static void Main(string[] args)
        {
            // **HACK** redirect console output to parent process;
            // must be before any calls to Console.WriteLine()
            AttachConsole(ATTACH_PARENT_PROCESS);

            if (args.Length == 0)
            {
                // Start Windowed Application Here
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain());
            }
            else
            {
                // do console App
                bool showHelp = false;
                IPAddress ipAddress = null;
                String password = null;
                E5186API.NET_MODE? netMode = null;

                try
                {
                    var p = new OptionSet() {
                    { "a|ipAddress=", "The IP-Address of the Device (Huawei E5186)", v => ipAddress = IPAddress.Parse(v) },
                    { "p|password=", "The Password to login to the Device", v => password = v },
                    { "m|mode=", "LTE | LTE_PREFFERED | HSDPA | HSDPA_PREFFERED | EDGE | AUTO", v =>  netMode = (E5186API.NET_MODE) System.Enum.Parse(typeof(E5186API.NET_MODE), v) },
                    { "h|?|help",  "show this message and exit",v => showHelp = v != null }
                };

                    List<string> extra = p.Parse(args);

                    if (showHelp || ipAddress == null || netMode == null || password == null)
                    {

                        Console.WriteLine("Usage Example: " + System.AppDomain.CurrentDomain.FriendlyName + " -a 192.168.8.1 -p test123 -m HSDPA");
                        Console.WriteLine();
                        Console.WriteLine("This Program switches the NetworkMode of a Huawei E5186 Device.");

                        p.WriteOptionDescriptions(Console.Out);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Try `--help' for more information.");
                    return;
                }

                Console.WriteLine();
                Console.WriteLine("==========================================");
                Console.WriteLine("========== E5186 Network Switch ==========");
                Console.WriteLine("==========================================");
                Console.WriteLine();


                E5186API api = new E5186API(ipAddress, password);
                api.messageHandler += delegate (String message, MessageType type)
                {
                    if (type == MessageType.ERROR) Console.WriteLine("ERROR: " + message);
                };
                api.connectionHandler += delegate (ConnectionType type)
                {
                    // TODO: Without connectionHandler --> Exception
                };


                Console.WriteLine("Connect to Device " + ipAddress + " ...");
                api.connect();

                if (api.Connected == ConnectionType.CONNECTED)
                {
                    E5186API.NET_MODE? currentMode = api.getNetMode();
                    Console.WriteLine("Current NetMode: " + currentMode);

                    if (currentMode == netMode)
                    {
                        Console.WriteLine("Device is already in given NetworkMode!");
                        Console.WriteLine("No need to set new NetworkMode ...");
                    }
                    else {
                        Console.WriteLine("Set NetworkMode to " + netMode.ToString() + "...");
                        api.setNetMode(netMode.Value);
                    }
                }
                else
                {
                    Console.WriteLine("Was not able to connect to device!");
                }
                Console.WriteLine("Programm Finished!");

                System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                Application.Exit();

            }
        }
    }
}
