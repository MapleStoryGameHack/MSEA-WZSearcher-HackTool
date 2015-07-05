/*   MSEA-WZSearcher-HackTool - A handy tool for MapleStory packet editing
    Copyright (C) 2012~2015 eaxvac/lastBattle https://github.com/eaxvac

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MSEAHackUtility
{
    static class Program
    {
        public static string gateway = "";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form1 form = new Form1();
            if (MessageBox.Show("Yes for MapleSEA V135;\r\nNo for EMS v97 data", "Selection", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Program.gateway = "StringWZ";
                form.Text = "Hack Utility [SEA 135]";
            }
            else
            {
                Program.gateway = "StringWZEMS";
                form.Text = "Hack Utility [EMS 97]";
            }
            Application.Run(form);
        }
    }
}
