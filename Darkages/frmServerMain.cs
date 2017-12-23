using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Darkages.Common;
using Darkages.Network.Game;
using Darkages.Storage;
using Darkages.Types;
using Newtonsoft.Json;
using ZTn.Json.JsonTreeView;
using ZTn.Json.JsonTreeView.Controls;
using static Darkages.Network.Object.ObjectManager;

namespace Darkages
{
    public partial class frmServerMain : Form
    {
        private JTokenTreeUserControl _configtree;
        private Instance _proxy = new Instance();

        public frmServerMain()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
            {
                if (ServerContext.Config.DebugMode)
                {
                    Debug.WriteLine(eventArgs.Exception.ToString());
                }
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _proxy = new Instance();

            _configtree = new JTokenTreeUserControl
            {
                Dock = DockStyle.Fill,
                Enabled = true
            };


            foreach (Control c in _configtree.Controls)
            {
                c.Font = new System.Drawing.Font("Arial,Verdana,Helvetica,sans-serif", 10);
                c.BackColor = System.Drawing.Color.Black;
                c.ForeColor = System.Drawing.Color.Orange;
            }

            _configtree.AfterSelect += _configtree_AfterSelect;

            var configsrc = ServerContext.Config.ToString();

            if (configsrc != string.Empty)
            {
                _configtree.SetJsonSource(configsrc);
                panel1.Controls.Add(_configtree);
            }

            timer1.Enabled = true;
        }

        private void _configtree_AfterSelect(object sender, AfterSelectEventArgs eventArgs)
        {
            textBox1.Text = eventArgs.TypeName;

            if (!textBox1.Focused)
                textBox1.Text = eventArgs.GetJsonString();
        }

        private void monsterTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var objCreateWindow = new ObjectCreation();
            objCreateWindow.SetObject(new MonsterTemplate
            {
                SpellScripts =
                    new Collection<string>(ServerContext.GlobalSpellTemplateCache.Keys
                        .ToList())
            });
            objCreateWindow.ShowDialog();
        }

        private void skillTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var objCreateWindow = new ObjectCreation();
            objCreateWindow.SetObject(new SkillTemplate());
            objCreateWindow.ShowDialog();
        }

        private void mapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var objCreateWindow = new ObjectCreation();
            objCreateWindow.SetObject(new Area());
            objCreateWindow.ShowDialog();
        }

        private void aislingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var objCreateWindow = new ObjectCreation();
            objCreateWindow.SetObject(new Aisling());
            objCreateWindow.ShowDialog();
        }


        private void monsterTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ServerContext.Game != null)
                ServerContext.Game.DelObjects(ServerContext.Game.GetObjects<Monster>(i => true));

            ServerContext.GlobalMonsterTemplateCache.Clear();
            ServerContext.LoadMonsterTemplates();
        }

        private void speedUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerContext.Game.Frames += 10;

            if (ServerContext.Game.Frames > 60)
                ServerContext.Game.Frames = 60;
        }

        private void slowDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerContext.Game.Frames =
                ServerContext.Game.Frames.Clamp(5, ServerContext.Game.Frames--);
        }

        private void spellTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var objCreateWindow = new ObjectCreation();
            objCreateWindow.SetObject(new SpellTemplate());
            objCreateWindow.ShowDialog();
        }

        private void rebootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Are you Sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Restart();
                Environment.Exit(0);
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            var item_idx = listBox1.SelectedIndex;
            if (item_idx < 0)
                return;

            var item = listBox1.Items[item_idx].ToString();

            var client = ServerContext.Game.Clients.Where(i =>
                    i != null && i.Aisling != null && i.Aisling.Username.ToLower().Equals(item.ToLower()))
                .FirstOrDefault();

            if (client != null)
            {
                groupBox2.Visible = true;
                groupBox2.Text = client.Aisling.Username;
                propertyGrid1.SelectedObject = null;
                propertyGrid1.SelectedObject = client.Aisling;

                client.SendMessage(0x02, "((LORULE GAME MASTER)): Inspecting your file. Please stand by.");
            }
        }

        private void groupBox2_Leave(object sender, EventArgs e)
        {
            groupBox2.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (propertyGrid1.SelectedObject == null)
                return;

            var item_idx = listBox1.SelectedIndex;
            if (item_idx < 0)
                return;

            var item = listBox1.Items[item_idx].ToString();

            var client = ServerContext.Game.Clients.Where(i =>
                    i != null && i.Aisling != null && i.Aisling.Username.ToLower().Equals(item.ToLower()))
                .FirstOrDefault();

            if (client != null)
            {
                client.SendMessage(0x02, "((LORULE GAME MASTER)): Your file has been modified. Please re-log.");

                var obj = (Aisling) propertyGrid1.SelectedObject;

                if (obj != null)
                {
                    client.Aisling = obj;
                    client.Server.ClientDisconnected(client);
                    client.Save();

                    propertyGrid1.SelectedObject = null;
                    groupBox2.Visible = false;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_proxy == null || !_proxy.IsRunning)
                return;

            listBox1.DataSource = null;
            listBox1.DataSource = ServerContext.Game.Clients
                .Where(i => i != null && i.Aisling != null)
                .Select(i => i.Aisling.Username).ToList();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO: implement a graceful shutdown.
            Process.GetCurrentProcess().Kill();
        }

        private void mundaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var objCreateWindow = new ObjectCreation();
            objCreateWindow.SetObject(new MundaneTemplate());
            objCreateWindow.ShowDialog();
        }

        private void mundanesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ServerContext.Game != null)
                ServerContext.Game.DelObjects(ServerContext.Game.GetObjects<Mundane>(i => true));

            ServerContext.GlobalMundaneTemplateCache.Clear();
            ServerContext.LoadMundaneTemplates();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ServerContext.Game == null)
                return;

            label1.Text = ServerContext.Game.GetObjects(i => true, Get.All).Length + " Objects in Memory.";
        }

        private void itemTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var objCreateWindow = new ObjectCreation();
            objCreateWindow.SetObject(new ItemTemplate());
            objCreateWindow.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_proxy == null)
                _proxy = new Instance();

            if (checkBox1.Checked)
                _proxy.Flush();
            else
                _proxy.Cache();

            if (!_proxy.IsRunning)
            {
                _proxy = new Instance();
                _proxy.Start();

                panel1.Visible = false;
                _configtree.Enabled = false;
                textBox1.Visible = false;
                button4.Visible = false;
                button3.Text = "Stop Server";
            }
            else
            {
                _configtree.Enabled = true;
                panel1.Visible = true;
                textBox1.Visible = true;
                button4.Visible = true;

                button3.Text = "Start Server";

                ServerContext.Game?.Abort();
                ServerContext.Lobby?.Abort();
                ServerContext.Running = false;

                _proxy = null;

                GC.Collect();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (_configtree != null)
                {
                    _configtree.UpdateSelected(textBox1.Text);

                    var tmp = Path.GetRandomFileName();
                    var pth = Path.Combine(Environment.CurrentDirectory, tmp);

                    using (var stream = new FileStream(pth, FileMode.OpenOrCreate))
                    {
                        _configtree.SaveJson(stream);
                    }

                    var newjson = File.ReadAllText(tmp);
                    {
                        File.Delete(tmp);
                    }

                    var newobj = JsonConvert.DeserializeObject<ServerConstants>(newjson, StorageManager.Settings);
                    if (newobj != null)
                    {
                        StorageManager.Save(newobj);
                        ServerContext.Config = newobj;

                        Console.WriteLine("config has been successfully modified.");
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error, config was not updated.");
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (_proxy != null && _proxy.IsRunning)
            {
                _proxy.Flush();
                return;
            }

            MessageBox.Show("Server must be running first.");
        }

        public class Instance : ServerContext
        {
            public Instance()
            {
                LoadConstants();
            }

            public bool IsRunning => Running;
        }

        private void loadTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void modifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new jsonEditor().ShowDialog();
        }
    }
}