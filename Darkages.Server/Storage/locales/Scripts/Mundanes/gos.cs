using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Darkages.Storage.locales.Scripts.Mundanes
{
    [Script("gos")]
    public class Gos : MundaneScript
    {
        public Dialog SequenceMenu = new Dialog();

        public Gos(GameServer server, Mundane mundane) : base(server, mundane)
        {
            Mundane.Template.QuestKey = "gos_quest";

            SequenceMenu.DisplayImage = (ushort) Mundane.Template.Image;
            SequenceMenu.Sequences.Add(new DialogSequence
            {
                Title = Mundane.Template.Name,
                DisplayText = "Man, Look around, can you believe this shit?! fuckin rats everywhere!"
            });
            SequenceMenu.Sequences.Add(new DialogSequence
            {
                Title = Mundane.Template.Name,
                DisplayText = "Looks like you need to level, so why not go kill some."
            });
            SequenceMenu.Sequences.Add(new DialogSequence
            {
                Title = Mundane.Template.Name,
                DisplayText = "Interested?",
                HasOptions = true,
                Callback = (sender, args) =>
                {
                    if (args.HasOptions)
                        sender.Client.SendOptionsDialog(Mundane, "Want to help?",
                            new OptionsDataItem(0x0010, "Yeah, I'll help."),
                            new OptionsDataItem(0x0011, "what's in it for me?"),
                            new OptionsDataItem(0x0012, "nah, I'm not killing rats for you. fuck off.")
                        );
                },
            });
            SequenceMenu.Sequences.Add(new DialogSequence
            {
                Title = Mundane.Template.Name,
                DisplayText = string.Empty,
                HasOptions = true,
                Callback = (sender, args) =>
                {
                    if (args.HasOptions)
                        sender.Client.SendOptionsDialog(Mundane, string.Format("oye {0}, Nice job.", sender.Path.ToString()),
                            new OptionsDataItem(0x0017, "Hand over the rat shit")
                        );
                },
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
            var quest = client.Aisling.Quests.FirstOrDefault(i => i.Name == Mundane.Template.QuestKey);

            if (quest == null)
            {
                quest = new Quest{ Name = Mundane.Template.QuestKey };
                quest.LegendRewards.Add(new Legend.LegendItem
                {
                    Category = "Quest",
                    Color = (byte) LegendColor.Blue,
                    Icon = (byte) LegendIcon.Victory,
                    Value = "Helped Gos kill some rats."
                });

                if (client.Aisling.Path == Class.Monk)
                    quest.SkillRewards.Add(ServerContext.GlobalSkillTemplateCache["Wolf Fang Fist"]);
                if (client.Aisling.Path == Class.Warrior)
                    quest.SkillRewards.Add(ServerContext.GlobalSkillTemplateCache["Crasher"]);

                client.Aisling.Quests.Add(quest);
            }

            quest.QuestStages = new List<QuestStep<Template>>();

            var q1 = new QuestStep<Template> { Type = QuestType.Accept };
            var q2 = new QuestStep<Template> { Type = QuestType.ItemHandIn };

            q2.Prerequisites.Add(new QuestRequirement()
            {
                Type = QuestType.ItemHandIn,
                Amount = 10,
                TemplateContext = ServerContext.GlobalItemTemplateCache["rat shit"]                
            });

            quest.QuestStages.Add(q1);
            quest.QuestStages.Add(q2);

            if (!quest.Started)
            {
                SequenceMenu.Invoke(client);
            }
            else if (quest.Started && !quest.Completed && !quest.Rewarded)
            {
                client.SendOptionsDialog(Mundane, "Where is the rat shit i need?");
                quest.HandleQuest(client, SequenceMenu);
            }
            else if (quest.Completed)
            {
                client.SendOptionsDialog(Mundane, "What a legend!");
            }
            else
            {
                SequenceMenu.Invoke(client);
            }
        }

        public override void OnResponse(GameServer server, GameClient client, ushort responseID, string args)
        {
            var quest = client.Aisling.Quests.FirstOrDefault(i =>
                i.Name == Mundane.Template.QuestKey);

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
                            SequenceMenu.MoveNext(client);
                            SequenceMenu.Invoke(client);
                        };
                        break;
                    case 0x0010:
                        client.SendOptionsDialog(Mundane, "sweet, Bring me some rat shit. (10)");

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
                        client.SendOptionsDialog(Mundane, "well fuck you then.");
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
                    case 0x0015:
                        if (quest != null && !quest.Rewarded && !quest.Completed)
                            quest.OnCompleted(client.Aisling);
                        break;
                    case 0x0016:
                        if (quest != null && !quest.Rewarded && !quest.Completed)
                            quest.OnCompleted(client.Aisling);
                        break;
                    case 0x0017:
                        if (quest != null && !quest.Rewarded && !quest.Completed)
                        {
                            quest.OnCompleted(client.Aisling);
                            client.SendOptionsDialog(Mundane, "Thank you.");
                        }
                        break;

                }
        }
    }
}