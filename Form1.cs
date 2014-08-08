using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

using MSEAHackUtility.Tool;

namespace MSEAHackUtility
{
    public partial class Form1 : Form
    {
        public static Dictionary<int, KeyValuePair<string, string>>
            HexJumpList = new Dictionary<int, KeyValuePair<string, string>>();

        public static Dictionary<int, int>
            JumpList_Map = new Dictionary<int, int>();

        public Form1()
        {
            InitializeComponent();

            this.Load += new EventHandler(Form1_Load);
        }

        void Form1_Load(object sender, EventArgs e)
        {
            // From map
            textBox1.Text = Properties.Settings.Default.teleport_frommap.ToString();
            // To map
            textBox2.Text = Properties.Settings.Default.teleport_tomap.ToString();
            // Map jump list
            textBox4.Text = Properties.Settings.Default.teleport_jumplist;

            // Map name load
            WZFactory.CacheMapData();

            // EXE Path
            txtFilePath.Text = Properties.Settings.Default.launchpath;
            // DLL Path
            txtDLLPath.Text = Properties.Settings.Default.launchdll;
            // DLL 2 path
            txtDLLPath2.Text = Properties.Settings.Default.launchdll2;
            // Process name
            textBox_procName.Text = Properties.Settings.Default.processname;
            // Injection delay
            numericUpDown_injectdelay.Value = Properties.Settings.Default.launchdelay;

            // HWID
            textBox_HWID.Text = Properties.Settings.Default.hwid_default;
            // HWID acc
            textBox_user.Text = Properties.Settings.Default.hwid_acc;
            // HWID Pass
            textBox_password.Text = Properties.Settings.Default.hwid_pass;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int frommap = Int32.Parse(textBox1.Text);
                int tomap = Int32.Parse(textBox2.Text);

                int map = 0; ;
                if (frommap <= tomap)
                {
                    map = (tomap - frommap);
                    label3.Text = map.ToString();
                }
                else
                {
                    map = -(frommap - tomap);
                    label3.Text = map.ToString();
                }
                Clipboard.SetText(label3.Text);
                textBox3.Text = BitConverter.ToString(INT2LE(map)).Replace("-", " ");

                Properties.Settings.Default.teleport_frommap = frommap;
                Properties.Settings.Default.teleport_tomap = tomap;
                Properties.Settings.Default.Save();
            }
            catch (Exception ee)
            {
                label3.Text = "Error!";
            } // ignore
        }

        byte[] INT2LE(int data)
        {
            byte[] b = new byte[4];
            b[0] = (byte)data;
            b[1] = (byte)(((uint)data >> 8) & 0xFF);
            b[2] = (byte)(((uint)data >> 16) & 0xFF);
            b[3] = (byte)(((uint)data >> 24) & 0xFF);
            return b;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox1_TextChanged(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // From
            int count = 0;
            int selecIndex = combobox.SelectedIndex;
            foreach (KeyValuePair<int, int> data in JumpList_Map)
            {
                if (count == selecIndex)
                {
                    textBox1.Text = data.Value.ToString();
                    textBox1_TextChanged(null, null);
                    break;
                }
                count++;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // To
            int count = 0;
            int selecIndex = combobox.SelectedIndex;
            foreach (KeyValuePair<int, int> data in JumpList_Map)
            {
                if (count == selecIndex)
                {
                    textBox2.Text = data.Value.ToString();
                    textBox2_TextChanged(null, null);
                    break;
                }
                count++;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            JumpList_Map.Clear();
            combobox.Items.Clear();

            string lower = textBox4.Text.ToLower();

            int order = 0;

            foreach (KeyValuePair<int, string> mapname in WZFactory.map_name)
            {
                if (mapname.Value.ToLower().Contains(lower))
                {
                    combobox.Items.Add(String.Format("{0} - {1}", mapname.Key, mapname.Value));
                    JumpList_Map.Add(order, mapname.Key); // Order / mapid
                    order++;
                }
            }
            //          Properties.Settings.Default.teleport_jumplist = textBox4.Text;
            Properties.Settings.Default.Save();
        }

        private void in_textchanged(object sender, EventArgs e)
        {
            int data = 0;
            try
            {
                data = Int32.Parse(textBox6.Text);
            }
            catch (Exception) { }
            textBox5.Text = BitConverter.ToString(INT2LE(data)).Replace("-", " ");

            Clipboard.SetText(textBox5.Text);
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            textBox1_TextChanged(null, null);
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
            textBox1_TextChanged(null, null);
        }

        #region itemidSearcher
        private void comboBox_hexlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selecIndex = comboBox_hexlist.SelectedIndex;
            if (selecIndex >= 0)
            {
                int count = 0;

                foreach (KeyValuePair<int, KeyValuePair<string, string>> data in HexJumpList)
                {
                    if (count == selecIndex)
                    {
                        textBox6.Text = data.Key.ToString();

                        label_itemname.Text = data.Value.Key;
                        label_itemdesc.Text = data.Value.Value;
                        break;
                    }
                    count++;
                }
            }
        }

        private void textBox_itemidFind_TextChanged(object sender, EventArgs e)
        {
            HexJumpList.Clear();
            comboBox_hexlist.Items.Clear();

            string query = textBox_itemidFind.Text.ToLower();

            if (checkBox_searcheq.Checked)
            {
                WZFactory.SearchEQ(query, HexJumpList);
            }
            if (checkBox_searchuse.Checked)
            {
                WZFactory.SearchUse(query, HexJumpList);
            }
            if (checkBox_searchsetup.Checked)
            {
                WZFactory.SearchSetup(query, HexJumpList);
            }

            if (checkBox_searchetc.Checked)
            {
                WZFactory.SearchETC(query, HexJumpList);
            }

            if (checkBox_searchcash.Checked)
            {
                WZFactory.SearchCash(query, HexJumpList);
            }

            if (HexJumpList.Count == 0)
            {
                textBox6.Text = "0";
                label_itemname.Text = "Not found";
                label_itemdesc.Text = "Not found";
            }
            else
            {
                bool first = true;
                foreach (KeyValuePair<int, KeyValuePair<string, string>> data in HexJumpList)
                {
                    if (first)
                    {
                        textBox6.Text = data.Key.ToString();

                        label_itemname.Text = data.Value.Key;
                        label_itemdesc.Text = data.Value.Value;

                        first = false;
                    }
                    comboBox_hexlist.Items.Add(String.Format("{0} - {1}", data.Value.Key, data.Value.Value));
                }
            }
        }
        #endregion

        #region injection
        private void numericUpDown_injectdelay_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.launchdelay = (int)numericUpDown_injectdelay.Value;
            Properties.Settings.Default.Save();
        }

        private void textBox_procName_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.processname = textBox_procName.Text;
            Properties.Settings.Default.Save();
        }

        private void button_EXEpathSelect_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog diag = new OpenFileDialog();
                diag.Filter = "Program Exe (*.exe)|*.exe";
                diag.Title = "Select Executable";
                if (diag.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                txtFilePath.Text = diag.FileName;

                Properties.Settings.Default.launchpath = txtFilePath.Text;
                Properties.Settings.Default.Save();
            }
            catch { }
        }

        private void button_DLLpathSelect2_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog diag = new OpenFileDialog();
                diag.Filter = "Lib (*.DLL)|*.DLL";
                diag.Title = "Select Library";
                if (diag.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    txtDLLPath2.Text = String.Empty;
                    return;
                }
                txtDLLPath2.Text = diag.FileName;

                Properties.Settings.Default.launchdll2 = txtDLLPath2.Text;
                Properties.Settings.Default.Save();
            }
            catch { }
        }

        private void button_DLLpathSelect_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog diag = new OpenFileDialog();
                diag.Filter = "Lib (*.DLL)|*.DLL";
                diag.Title = "Select Library";
                if (diag.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                txtDLLPath.Text = diag.FileName;

                Properties.Settings.Default.launchdll = txtDLLPath.Text;
                Properties.Settings.Default.Save();
            }
            catch { }
        }

        public static System.Threading.Timer TimerReference;
        public static Process Maple;

        private void button_injectdll_Click(object sender, EventArgs e)
        {
            Maple = new Process();
            Maple.StartInfo.FileName = txtFilePath.Text;
            try
            {
                Maple.Start();
                callTimer();
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to start process..");
            }
        }

        private void button_dllinject_active_Click(object sender, EventArgs e)
        {
            Process[] processList = Process.GetProcessesByName(textBox_procName.Text);
            if (processList[0] != null)
            {
                Maple = processList[0];
                callTimer();
            }
            else
            {
                MessageBox.Show("Process is not running! Please check again");
            }
        }

        private void callTimer()
        {
            button_injectdll.Enabled = false;
            button_dllinject_active.Enabled = false;
            textBox_procName.Enabled = false;

            TimerCallback TimerDelegate = new TimerCallback(TimerTask);

            // Create a timer that calls a procedure every 2 seconds.
            // Note: There is no Start method; the timer starts running as soon as 
            // the instance is created.
            System.Threading.Timer TimerItem = new System.Threading.Timer(TimerDelegate, null, Properties.Settings.Default.launchdelay, Properties.Settings.Default.launchdelay);

            // Save a reference for Dispose.
            TimerReference = TimerItem;
        }

        private void TimerTask(object StateObj)
        {
            string sError = String.Empty;

            string path1 = null;
            string path2 = null;
            this.txtDLLPath.Invoke(new MethodInvoker(() => path1 = this.txtDLLPath.Text));
            this.txtDLLPath2.Invoke(new MethodInvoker(() => path2 = this.txtDLLPath2.Text));

            if (!Injector.DoInject(Maple, path1, path2, out sError))
                MessageBox.Show("The following error occured while injecting the \n" +
                    "dll into the application: \n" +
                    sError);

            TimerReference.Dispose();
            TimerReference = null;
            Debug.WriteLine("Injection " + DateTime.Now.ToString());

            button_injectdll.Enabled = true;
            button_dllinject_active.Enabled = true;
            textBox_procName.Enabled = true;
        }
        #endregion

        private void button6_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            string texts = "0123456789ABCDEF";
            string out_ = string.Empty;

            for (int i = 0; i < 12; i++)
            {
                out_ += texts[r.Next(texts.Length)];
            }
            out_ += "_";
            for (int i = 0; i < 8; i++)
            {
                out_ += texts[r.Next(texts.Length)];
            }
            textBox_HWID.Text = out_;//"0100EC9F4152_C8226412";
        }

        private void button_RSA_Click(object sender, EventArgs e)
        {
            if (!File.Exists("WzCrypto.dll"))
            {
                MessageBox.Show("WzCrypto.dll is missing, please place it in the executable directory.");
                return;
            }
            if (textBox_user.Equals(String.Empty) || textBox_RSA.Equals(String.Empty) || textBox_HWID.Equals(String.Empty))
            {
                MessageBox.Show("Password, HWID or RSA cannot be empty!");
                return;
            }
            else if (textBox_HWID.Text.Length != 21)
            {
                MessageBox.Show("Desired HWID length must be 21! To prevent banning, please also follow the format..");
                return;
            }
            // Read
            PacketReader read = new PacketReader(ByteUtils.HexToBytes(textBox_RSA.Text));
  //          read.Skip(2); // Header
            int len = read.ReadShort();
            byte[] RSAKey = read.ReadBytes(len);

            // Write
            StringBuilder sb = new StringBuilder("Header : ");
            sb.Append((int)SendOps.LOGIN);
            Packet packet = new Packet(SendOps.LOGIN);

            sb.Append(" User : ").Append(textBox_user.Text);
            packet.WriteString(textBox_user.Text);

            packet.WriteString(Encoding.ASCII.GetString(WzRSAEncrypt.Encrypt(RSAKey, RSAKey.Length, textBox_password.Text)));

            sb.Append(" HWID : ").Append(textBox_HWID.Text);
            packet.WriteString(textBox_HWID.Text);
            packet.WriteHexAsBytes("00 00 00 00 00 00 A3 27 64 60 00 00 00 00 0E 92 00 00 00 00 02 00");
    //        packet.WriteHexAsBytes("00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");

            textBox_RSAOutput.Text = packet.ToPacketsString();
            textBox_hwidoutputRead.Text = sb.ToString();

            Properties.Settings.Default.hwid_acc = textBox_user.Text;
            Properties.Settings.Default.hwid_pass = textBox_password.Text;
            Properties.Settings.Default.hwid_default = textBox_HWID.Text;
            Properties.Settings.Default.Save();
        }

        #region servertools
        private void button3_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(Compute1));
            t.Priority = ThreadPriority.AboveNormal;
            t.Start();

            Thread t2 = new Thread(new ThreadStart(Compute2));
            t2.Priority = ThreadPriority.AboveNormal;
            t2.Start();

            Thread t3 = new Thread(new ThreadStart(Compute3));
            t3.Priority = ThreadPriority.AboveNormal;
            t3.Start();
        }

        private List<string> FlagDuplicates = new List<string>();
        int[] mapids = { 100000000, 200000000, 103000000, 101000000, 104000000, 105000000, 102000000, 260000000, 261000000, 222000000, 230000000, 250000000, 251000000 };
        string[] mapstrs = { "Henesys", "Orbis", "Kerning", "Ellinia", "Lith Harbour", "Sleepywood", "Perion", "Ariant", "Magatia", "Korean Folks town", "Aquarium", "Mu Lung", "Herb Town" };

        private void Compute1()
        {
            Compute(0, 700000000);
        }

        private void Compute2()
        {
            Compute(700000001, 1400000000);
        }

        private void Compute3()
        {
            Compute(1400000001, 2147483647);
        }

        private void Compute(int min, int max)
        {
            Random r = new Random();
            StringBuilder sb = new StringBuilder();
            int pendingCounts = 0;

            for (int i = min; i < max; i++)
            {
                int order = 0;
                foreach (int z in mapids)
                {
                    if (z != i)
                    {
                        int TargetMap = z | i;
                        if (z != TargetMap)
                        {
                            string hashIndex = string.Format("{0}{1}", TargetMap, z);
                            if (FlagDuplicates.Contains(hashIndex))
                            {
                                continue; // already there
                            }

                            string mapname = WZFactory.getMapNameEx(TargetMap);
                            if (mapname != null)
                            {
                                Packet packet = new Packet();
                                packet.WriteInt(i);

                                // Start with indexes
                                sb.Append(z); // From map
                                sb.Append(" ");
                                sb.Append(TargetMap); // ToMap
                                sb.Append(" [");
                                sb.Append(packet.ToPacketsString());
                                sb.Append("]");

                                // Human readable
                                sb.Append(" ||| ");
                                sb.Append(mapstrs[order]);
                                sb.Append(" > ");
                                sb.Append(mapname);

                                if (!FlagDuplicates.Contains(hashIndex))
                                    FlagDuplicates.Add(hashIndex);


                                pendingCounts++;
                                if (pendingCounts > 50)
                                {
                                    Debug.WriteLine(sb.ToString());
                                    sb.Clear();
                                }
                                else
                                {
                                    sb.Append("\r\n");
                                }
                            }
                        }
                    }
                    order++;
                }
            }
            // others
            Debug.WriteLine(sb.ToString());
            sb.Clear();
        }
        #endregion

        #region AutoClicker
        private static System.Timers.Timer aTimer;
        private static string foregroundWindow;

        private void button4_Click(object sender, EventArgs e)
        {
            foregroundWindow = textBox_msAppTitle.Text;
            if ((AutoClicker.hWnd = AutoClicker.FindWindow(foregroundWindow, null)) != IntPtr.Zero)
            {
                AutoClicker.RegisterHotKey(this.Handle, 10, 0x0002, 0x11);//Ctrl
                AutoClicker.SetForegroundWindow(AutoClicker.hWnd);


                System.Timers.Timer aTimer = new System.Timers.Timer();
                aTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
                // Set the Interval to 5 seconds.
                aTimer.Interval = (double)numericUpDown1.Value;
                aTimer.Enabled = true;
            }
            else
            {
                MessageBox.Show("Application Window not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private static void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            System.IntPtr foreground;
            if ((foreground = AutoClicker.FindWindow(foregroundWindow, null)) == AutoClicker.hWnd)
            {
                SendKeys.SendWait("{Y}");
            }
            else
            {
                System.Timers.Timer aTimer = (System.Timers.Timer)source;
                aTimer.Stop();
            }
        }

        #endregion

        #region PacketBuilder
        private void TextBox_BuilderStrChanged(object sender, EventArgs e)
        {
            label_PBuilder_Length.Text = textBox_PBuilder.Text.Length.ToString();
        }

        private void button_int64_Click(object sender, EventArgs e)
        {
            Packet packet = new Packet();

            if (checkBox_int64.Checked)
            {
                ulong s = UInt64.Parse(textBox_PBuilder_int64.Text);
                packet.WriteLong((long) (s & 0xFFFFFFFFFFFFFFFF));
            }
            else
            {
                long s = Int64.Parse(textBox_PBuilder_int64.Text);
                packet.WriteLong(s);
            }
            textBox_PBuilderOut.Text += packet.ToPacketsString();
        }

        private void button_int32_Click(object sender, EventArgs e)
        {
            Packet packet = new Packet();

            try
            {
                if (checkBox_int32.Checked)
                {
                    uint s = UInt32.Parse(textBox_PBuilder_int32.Text);
                    packet.WriteUInt(s);
                }
                else
                {
                    int s = Int32.Parse(textBox_PBuilder_int32.Text);
                    packet.WriteInt(s);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("Error parsing!\r\n\r\n" + ee.ToString());
            }

            textBox_PBuilderOut.Text += packet.ToPacketsString();
        }

        private void button_int16_Click(object sender, EventArgs e)
        {
            Packet packet = new Packet();

            try
            {
                if (checkBox_int16.Checked)
                {
                    ushort s = UInt16.Parse(textBox_PBuilder_int16.Text);
                    packet.WriteUShort(s);
                }
                else
                {
                    short s = Int16.Parse(textBox_PBuilder_int16.Text);
                    packet.WriteShort(s);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("Error parsing!\r\n\r\n" + ee.ToString());
            }
            textBox_PBuilderOut.Text += packet.ToPacketsString();
        }

        private void button_int8_Click(object sender, EventArgs e)
        {
            Packet packet = new Packet();

            try
            {
                if (checkBox_int8.Checked)
                {
                    sbyte s = SByte.Parse(textBox_PBuilder_int8.Text);
                    packet.WriteByte((byte) (s & 0xFF));
                }
                else
                {
                    byte s = Byte.Parse(textBox_PBuilder_int8.Text);
                    packet.WriteByte(s);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("Error parsing!\r\n\r\n" + ee.ToString());
            }

            textBox_PBuilderOut.Text += packet.ToPacketsString();
        }

        private void button_string_Click(object sender, EventArgs e)
        {
            Packet packet = new Packet();
            packet.WriteString(textBox_PBuilder.Text);

            textBox_PBuilderOut.Text += packet.ToPacketsString();
        }

        private void checkBox_int64_CheckedChanged(object sender, EventArgs e)
        {
            label_int64.Text = !checkBox_int64.Checked ? "Value [int64] :" : "Value [uint64] :";
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            label_int32.Text = !checkBox_int32.Checked ? "Value [int32] :" : "Value [uint32] :";
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            label_int16.Text = !checkBox_int16.Checked ? "Value [int16] :" : "Value [uint16] :";
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            label_int8.Text = !checkBox_int8.Checked ? "Value [int8] :" : "Value [uint8] :";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show(/*"1) Values can also be in HEX, just specify it by starting with '0xSOME.VALUE'.\r\n" +*/
"1) Packet headers will always be uint16!\r\n" + 
"");
        }

        #endregion
    }
}