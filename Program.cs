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
