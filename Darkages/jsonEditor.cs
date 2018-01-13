using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Darkages.Common;
using Darkages.Storage;
using Darkages.Types;
using ZTn.Json.JsonTreeView;
using ZTn.Json.JsonTreeView.Controls;

namespace Darkages
{
    public partial class jsonEditor : Form
    {
        private readonly JTokenTreeUserControl _configtree;

        public int LastSelected = -1;

        public jsonEditor()
        {
            InitializeComponent();


            LastSelected = 0;

            _configtree = new JTokenTreeUserControl
            {
                Dock = DockStyle.Fill,
                Enabled = true
            };

            _configtree.AfterSelect += _configtree_AfterSelect;
        }

        private void _configtree_AfterSelect(object sender, AfterSelectEventArgs e)
        {
            textBox1.Text = e.TypeName;

            if (!textBox1.Focused)
                textBox1.Text = e.GetJsonString();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            if (listView1.SelectedItems.Count == 0)
                return;

            LastSelected = listView1.SelectedIndices[0];

            var file = listView1.SelectedItems[0].SubItems[2].Text;

            if (file != string.Empty)
            {
                var src = File.ReadAllText(file);
                _configtree.SetJsonSource(src);
            }
        }

        private void jsonEditor_Load(object sender, EventArgs e)
        {
            panel1.Controls.Add(_configtree);

            if (!Directory.Exists(ServerContext.StoragePath))
                return;

            var content_files = Directory.GetFiles(ServerContext.StoragePath, "*.json", SearchOption.AllDirectories);

            foreach (var _file in content_files)
            {
                var file = new FileInfo(_file).FullName;

                var type = File.ReadLines(file).ToArray();
                var stype = type.FirstOrDefault(i => i.Contains("$type"))
                    ?.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .ToArray();

                var typestr = "";

                if (stype.Length > 0)
                    typestr = string.Join(",", stype[0], stype[1]);

                typestr = typestr.Replace("{", "");
                typestr = typestr.Replace("}", "");
                typestr = typestr.Replace("\"$type\": \"", string.Empty).TrimEnd('\"').Trim();
                typestr = typestr.Replace("\"$type\":\"", string.Empty).TrimEnd('\"').Trim();

                var tobj = Reflection.Create(typestr);
                if (tobj != null)
                    if (tobj is Template)
                    {
                        if (tobj is ItemTemplate)
                        {
                            var obj = StorageManager.LoadFrom<ItemTemplate>(file);
                            listView1.Items.Add(new ListViewItem(new[] {obj.Name, obj.GetType().Name, file}));
                        }

                        if (tobj is MundaneTemplate)
                        {
                            var obj = StorageManager.LoadFrom<MundaneTemplate>(file);
                            listView1.Items.Add(new ListViewItem(new[] {obj.Name, obj.GetType().Name, file}));
                        }

                        if (tobj is MonsterTemplate)
                        {
                            var obj = StorageManager.LoadFrom<MonsterTemplate>(file);
                            listView1.Items.Add(new ListViewItem(new[] {obj.Name, obj.GetType().Name, file}));
                        }

                        if (tobj is SkillTemplate)
                        {
                            var obj = StorageManager.LoadFrom<SkillTemplate>(file);
                            listView1.Items.Add(new ListViewItem(new[] {obj.Name, obj.GetType().Name, file}));
                        }

                        if (tobj is SpellTemplate)
                        {
                            var obj = StorageManager.LoadFrom<SpellTemplate>(file);
                            listView1.Items.Add(new ListViewItem(new[] {obj.Name, obj.GetType().Name, file}));
                        }

                        if (tobj is WarpTemplate)
                        {
                            var obj = StorageManager.LoadFrom<WarpTemplate>(file);
                            listView1.Items.Add(new ListViewItem(new[] {obj.Name, obj.GetType().Name, file}));
                        }
                    }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (_configtree != null)
                {
                    var item = listView1.Items[LastSelected].SubItems[2].Text;
                    if (!string.IsNullOrEmpty(item) && !string.IsNullOrEmpty(textBox1.Text))
                    {
                        _configtree.UpdateSelected(textBox1.Text);

                        using (var stream = new FileStream(item, FileMode.Truncate))
                        {
                            _configtree.SaveJson(stream);
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error Saving, is everything correct?");
            }
        }
    }
}