using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchService.Launcher
{
    public class MeetingLauncher : IMeetingLauncher
    {
        public void Launch(string service)
        {
            string browserPath = GetBrowserPath();
            if (browserPath == string.Empty)
                browserPath = "iexplore";
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo(browserPath)
            };
            process.StartInfo.Arguments = "\"" + service + "\"";
            process.Start();
        }

        protected static string GetBrowserPath()
        {
            string browser = string.Empty;
            try
            {
                // try location of default browser path in XP
#pragma warning disable CA1416 // Validate platform compatibility
                RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);
#pragma warning restore CA1416 // Validate platform compatibility

                // try location of default browser path in Vista
                if (key == null)
                {
#pragma warning disable CA1416 // Validate platform compatibility
                    key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http", false); ;
#pragma warning restore CA1416 // Validate platform compatibility
                }

                if (key != null)
                {
                    //trim off quotes
#pragma warning disable CA1416 // Validate platform compatibility
                    browser = key.GetValue(null)
#pragma warning restore CA1416 // Validate platform compatibility
                        .ToString()
                        .ToLower()
                        .Replace("\"", "");
                    if (!browser.EndsWith("exe"))
                    {
                        //get rid of everything after the ".exe"
                        browser = browser.Substring(0, browser.LastIndexOf(".exe") + 4);
                    }

#pragma warning disable CA1416 // Validate platform compatibility
                    key.Close();
#pragma warning restore CA1416 // Validate platform compatibility
                }
            }
            catch
            {
                return string.Empty;
            }

            return browser;
        }
    }
}
