using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Darkages.IO;
using Darkages.Storage;
using Darkages.Types;

namespace Darkages
{
    public partial class ObjectCreation : Form
    {
        public ObjectCreation()
        {
            InitializeComponent();
        }

        internal void SetObject<T>(T obj) where T : new()
        {
            propertyGrid1.SelectedObject = null;
            propertyGrid1.SelectedObject = obj;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (propertyGrid1.SelectedObject == null)
                return;

            if (propertyGrid1.SelectedObject is MonsterTemplate)
            {
                var template = (MonsterTemplate) propertyGrid1.SelectedObject;

                try
                {
                    StorageManager.MonsterBucket.Save(template);
                    Console.WriteLine("New MonsterTemplate Added. You must restart server to take affect.");
                    Close();
                }
                catch
                {
                    Console.WriteLine("{0} was not created. Something went wrong.", template.Name);
                }
            }

            if (propertyGrid1.SelectedObject is Area)
            {
                var template = (Area) propertyGrid1.SelectedObject;

                try
                {
                    if (!Directory.Exists(ServerContext.STORAGE_PATH + "\\maps"))
                        Directory.CreateDirectory(ServerContext.STORAGE_PATH + "\\maps");

                    var mapFile = Directory.GetFiles($@"{ServerContext.STORAGE_PATH}\maps", $"lor{template.ID}.map",
                        SearchOption.TopDirectoryOnly).FirstOrDefault();

                    redo:

                    if (mapFile != null)
                    {
                        template.Data = File.ReadAllBytes(mapFile);
                        template.Hash = Crc16Provider.ComputeChecksum(template.Data);

                        StorageManager.AreaBucket.Save(template);
                        Console.WriteLine("New Area Added. You must restart server to take affect.");
                        Close();
                    }
                    else
                    {
                        using (var fod = new OpenFileDialog())
                        {
                            fod.Multiselect = false;
                            fod.CheckFileExists = true;
                            fod.AutoUpgradeEnabled = true;
                            fod.Filter = "MAP File|*.map";
                            if (fod.ShowDialog() == DialogResult.OK)
                            {
                                mapFile = fod.FileName;

                                var file = ServerContext.STORAGE_PATH + "\\maps\\" + Path.GetFileName(mapFile);
                                if (!File.Exists(file))
                                {
                                    Console.WriteLine("Copying over map file to storage....");
                                    File.Copy(mapFile, file);
                                    Console.WriteLine("Copying over map file to storage....  Done");
                                }

                                goto redo;
                            }
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("{0} was not created. Something went wrong.", template.Name);
                }
            }


            if (propertyGrid1.SelectedObject is SkillTemplate)
            {
                var template = (SkillTemplate) propertyGrid1.SelectedObject;
                try
                {
                    StorageManager.SKillBucket.Save(template);
                    Console.WriteLine("New Skill Added. You must restart server to take affect.");
                    Close();
                }
                catch
                {
                    Console.WriteLine("{0} was not created. Something went wrong.", template.Name);
                }
            }

            if (propertyGrid1.SelectedObject is SpellTemplate)
            {
                var template = (SpellTemplate) propertyGrid1.SelectedObject;
                try
                {
                    StorageManager.SpellBucket.Save(template);
                    Console.WriteLine("New Spell Added. You must restart server to take affect.");
                    Close();
                }
                catch
                {
                    Console.WriteLine("{0} was not created. Something went wrong.", template.Name);
                }
            }

            if (propertyGrid1.SelectedObject is ItemTemplate)
            {
                var template = (ItemTemplate) propertyGrid1.SelectedObject;
                try
                {
                    StorageManager.ItemBucket.Save(template);
                    Console.WriteLine("New Item Template Added. You must restart server to take affect.");
                    Close();
                }
                catch
                {
                    Console.WriteLine("{0} was not created. Something went wrong.", template.Name);
                }
            }

            if (propertyGrid1.SelectedObject is MundaneTemplate)
            {
                var template = (MundaneTemplate) propertyGrid1.SelectedObject;
                try
                {
                    StorageManager.MundaneBucket.Save(template);
                    Console.WriteLine("New Mundane Added. You must restart server to take affect.");
                    Close();
                }
                catch
                {
                    Console.WriteLine("{0} was not created. Something went wrong.", template.Name);
                }
            }
        }

        private static ushort[] GetDimensions(byte[] mapData)
        {
            int mapLength = mapData.Length / 6, divRemainder, divResult;
            var maxLength = Math.Sqrt(mapLength);
            var segments = new List<ushort[]>();

            for (var i = 1; i <= maxLength; i++)
            {
                divResult = Math.DivRem(mapLength, i, out divRemainder);
                if (divRemainder == 0)
                    segments.Add(new[] {(ushort) i, (ushort) divResult});
            }

            foreach (var pair in segments.ToArray())
                if (pair[0] != pair[1])
                    segments.Add(pair.Reverse().ToArray());

            return segments[segments.Count / 2];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}