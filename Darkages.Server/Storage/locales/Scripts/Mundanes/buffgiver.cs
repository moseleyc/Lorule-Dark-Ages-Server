using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Mundanes
{
    [Script("buff")]
    public class buffgiver : MundaneScript
    {
        public Dialog SequenceMenu = new Dialog();

        public buffgiver(GameServer server, Mundane mundane) : base(server, mundane)
        {
            Mundane.Template.QuestKey = "gos_quest";

            SequenceMenu.DisplayImage = (ushort) Mundane.Template.Image;
            SequenceMenu.Sequences.Add(new DialogSequence
            {
                Title = Mundane.Template.Name,
                DisplayText = "Hey, Have you seen my stuff???"
            });
            SequenceMenu.Sequences.Add(new DialogSequence
            {
                Title = Mundane.Template.Name,
                DisplayText = "Someone came past here and stole my shit!!"
            });
            SequenceMenu.Sequences.Add(new DialogSequence
            {
                Title = Mundane.Template.Name,
                DisplayText = "Can you help me?",
                HasOptions = true
            });
        }

        public override void OnClick(GameServer server, GameClient client)
        {
            if (client.DlgSession == null)
                client.DlgSession = new DialogSession(client.Aisling, SequenceMenu.Serial)
                {
                    Callback = OnResponse,
                    StateObject = SequenceMenu
                };

            if (client.DlgSession.Serial != SequenceMenu.Serial)
                client.DlgSession = new DialogSession(client.Aisling, SequenceMenu.Serial)
                {
                    Callback = OnResponse,
                    StateObject = SequenceMenu
                };


            if (!client.Aisling.Position.IsNearby(client.DlgSession.SessionPosition))
                return;

            if (!SequenceMenu.CanMoveNext)
                SequenceMenu.SequenceIndex = 0;

            QuestComposite(client);
        }

        private void QuestComposite(GameClient client)
        {
            var quest = client.Aisling.CurrentHandInQuests.FirstOrDefault(i => i.Name == Mundane.Template.QuestKey);

            if (quest == null)
            {
                quest = new Quest<ItemTemplate> {Name = Mundane.Template.QuestKey};
                quest.LegendRewards.Add(new Legend.LegendItem
                {
                    Category = "Quest",
                    Color = (byte) LegendColor.Blue,
                    Icon = (byte) LegendIcon.Victory,
                    Value = "Helped Gos find his shit."
                });

                client.Aisling.CurrentHandInQuests.Add(quest);
            }

            quest.QuestStages = new List<QuestStep<ItemTemplate>>();
            var q1 = new QuestStep<ItemTemplate> {Name = "Accepted"};
            var q2 = new QuestStep<ItemTemplate> {Name = "Hand in"};

            q1.RequirementsToProgress.Add(i => true); // no reqs to start quest.
            q2.RequirementsToProgress.Add(i => i.Name == "Shirt"); // reqs to finish quest.

            quest.QuestStages.Add(q1);
            quest.QuestStages.Add(q2);

            if (!quest.Started)
            {
                client.Send(new ServerFormat30(client, SequenceMenu));
            }
            else if (quest.Started && !quest.Completed)
            {
                client.SendOptionsDialog(Mundane, "Any luck finding it?");
                HandleQuest(client);
            }
            else if (quest.Completed)
            {
                client.SendOptionsDialog(Mundane, "Thanks so much!");
            }
        }

        private void HandleQuest(GameClient client)
        {
            var items = client.Aisling.Inventory.Items
                .Where(i => i.Value != null)
                .Select(i => i.Value).ToList();

            var valid = false;
            var quest = client.Aisling.CurrentHandInQuests.FirstOrDefault(i => i.Name == Mundane.Template.QuestKey);

            if (quest != null)
                foreach (var req in quest.QuestStages[quest.StageIndex].RequirementsToProgress)
                {
                    var obj = items.Find(i => req(i.Template));

                    if (obj != null)
                    {
                        valid = true;
                        client.Aisling.EquipmentManager.RemoveFromInventory(obj, true);

                        break;
                    }
                }


            if (valid)
            {
                quest.Completed = true;
                quest?.OnCompleted(client.Aisling);
            }
        }

        public override void OnResponse(GameServer server, GameClient client, ushort responseID, string args)
        {
            if (client.DlgSession != null && client.DlgSession.Serial == SequenceMenu.Serial)
                switch (responseID)
                {
                    case 0:
                        SequenceMenu.SequenceIndex = 0;
                        client.DlgSession = null;

                        break;
                    case 1:
                        if (SequenceMenu.CanMoveNext)
                        {
                            var idx = (ushort) (SequenceMenu.SequenceIndex + 1);

                            SequenceMenu.SequenceIndex = idx;
                            client.DlgSession.Sequence = idx;

                            client.Send(new ServerFormat30(client, SequenceMenu));
                        }

                        if (SequenceMenu.Current.HasOptions)
                            client.SendOptionsDialog(Mundane, "Can you help me find my shit?",
                                new OptionsDataItem(0x0010, "Yeah, I'll help."),
                                new OptionsDataItem(0x0011, "what's in it for me?"),
                                new OptionsDataItem(0x0012, "nah, go fuck yourself.")
                            );

                        break;

                    case 0x0010:
                        client.SendOptionsDialog(Mundane, "Awesome!, let me know if you come across it!");
                        var quest = client.Aisling.CurrentHandInQuests.FirstOrDefault(i =>
                            i.Name == Mundane.Template.QuestKey);

                        if (quest != null)
                        {
                            quest.Started = true;
                            quest.TimeStarted = DateTime.UtcNow;
                        }

                        break;
                    case 0x0011:
                        client.SendOptionsDialog(Mundane, "I'll hook you up with something good!",
                            new OptionsDataItem(0x0010, "Sure, Ok then!"),
                            new OptionsDataItem(0x0012, "Don't need anything mate.")
                        );
                        break;
                    case 0x0012:
                        client.SendOptionsDialog(Mundane, "woah, ok then :(.");
                        break;
                    case ushort.MaxValue:
                        if (SequenceMenu.CanMoveBack)
                        {
                            var idx = (ushort) (SequenceMenu.SequenceIndex - 1);

                            SequenceMenu.SequenceIndex = idx;
                            client.DlgSession.Sequence = idx;

                            client.Send(new ServerFormat30(client, SequenceMenu));
                        }
                        break;
                }
        }
    }
}