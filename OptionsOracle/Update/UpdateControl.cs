/*
 * OptionsOracle
 * Copyright 2006-2012 SamoaSky (Shlomo Shachar & Oren Moshe)
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace OptionsOracle.Update
{
    public class UpdateControl
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public static string OptionsOracleUpdaterFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OptionsOracle Updater.exe");

        public enum ExitAndRestartModeT
        {
            None,
            Wait,
            Exit,
            ExitAndRestart
        }

        public static void EnableElevateIcon(Button button)
        {
            // Define BCM_SETSHIELD locally, declared originally in commctrl.h
            const uint BCM_SETSHIELD = 0x0000160C;

            // Set button style to the system style
            button.FlatStyle = FlatStyle.System;

            // Send the BCM_SETSHIELD message to the button control
            SendMessage(new HandleRef(button, button.Handle), BCM_SETSHIELD, new IntPtr(0), new IntPtr(1));
        }

        public static void StartUpdaterProcess(List<string> args, ExitAndRestartModeT mode)
        {
            string filename = Path.Combine(System.IO.Path.GetTempPath(), "OptionsOracle Updater.exe");

            try
            {
                File.Copy(OptionsOracleUpdaterFileName, filename, true);
            }
            catch { filename = OptionsOracleUpdaterFileName; }

            if (!File.Exists(filename))
                filename = OptionsOracleUpdaterFileName;

            Process pro = new Process();
            
            pro.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            pro.StartInfo.FileName = filename;

            if (mode == ExitAndRestartModeT.ExitAndRestart)
            {
                args.Add("/run");
                args.Add(Application.ExecutablePath.Trim().TrimEnd(new char[] { '\\' }));
            }

            string arg_line = "";
            foreach (string arg in args)
                if (arg.Trim().Contains(" "))
                    arg_line += "\"" + arg.Trim() + "\" ";
                else
                    arg_line += arg.Trim() + " ";

            if (arg_line.Contains("/update") && System.Environment.OSVersion.Version.Major > 5)
            {
                pro.StartInfo.Verb = "runas";
                pro.StartInfo.UseShellExecute = true;
            }

            pro.StartInfo.Arguments = arg_line.Trim();
            pro.Start();

            if (mode == ExitAndRestartModeT.Wait)
                pro.WaitForExit();

            if (mode == ExitAndRestartModeT.Exit || mode == ExitAndRestartModeT.ExitAndRestart)
                Environment.Exit(0);
        }

        public static bool ResetConfiguration(ExitAndRestartModeT mode)
        {
            List<string> args = new List<string>();

            try
            {
                args.Add(@"/reset");
                StartUpdaterProcess(args, mode);
            }
            catch { return false; }

            // reset application settings as well
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();

            return true;
        }

        public static bool FullUpgrade(string url)
        {
            List<string> args = new List<string>();

            try
            {
                string url_base = url.Substring(0, url.LastIndexOf('/'));
                string url_file = url.Substring(url.LastIndexOf('/') + 1);

                args.Add(@"/host");
                args.Add(url_base);
                args.Add(@"/full");
                args.Add(url_file);

                StartUpdaterProcess(args, ExitAndRestartModeT.ExitAndRestart);
            }
            catch { return false; }

            return true;
        }

        public static bool PartialUpgrade(List<UpdateInfo> update_list, List<UpdateInfo> remove_list)
        {
            List<string> args = new List<string>();

            try
            {
                if (update_list.Count > 0)
                {
                    args.Add(@"/host");
                    args.Add(update_list[0].FileName.Substring(0, update_list[0].FileName.LastIndexOf('/')));
                    args.Add(@"/update");

                    foreach (UpdateInfo info in update_list)
                        args.Add(info.FileName.Substring(info.FileName.LastIndexOf('/') + 1));
                }

                if (remove_list.Count > 0)
                {
                    args.Add(@"/remove");
                    foreach (UpdateInfo info in remove_list)
                        args.Add(info.FileName.Substring(info.FileName.LastIndexOf('/') + 1));
                }

                args.Add(@"/path");
                args.Add(AppDomain.CurrentDomain.BaseDirectory.Trim().TrimEnd(new char[] { '\\' }));

                StartUpdaterProcess(args, ExitAndRestartModeT.ExitAndRestart);
            }
            catch { return false; }

            return true;
        }
    }
}
