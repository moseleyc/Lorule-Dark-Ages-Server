using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Mundanes
{
    [Script("Learn Skills")]
    public class LearkSkills : MundaneScript
    {
        public LearkSkills(GameServer server, Mundane mundane)
            : base(server, mundane)
        {
        }

        public override void OnGossip(GameServer server, GameClient client, string message)
        {
        }

        public override void TargetAcquired(Sprite Target)
        {
        }


        public override void OnClick(GameServer server, GameClient client)
        {
            client.SendOptionsDialog(Mundane, "How may I assist you?",
                new OptionsDataItem(0x0001, "Learn Skill"));
        }

        public override void OnResponse(GameServer server, GameClient client, ushort responseID, string args)
        {
            switch (responseID)
            {
                // Skill Learn
                case 0x0001:
                    var skills = ServerContext.GlobalSkillTemplateCache.Values;
                    client.SendSkillLearnDialog(Mundane, "Which skill would you like to learn?", 0x0003, skills);
                    break;
                // Skill Confirmation
                case 0x0003:
                    client.SendOptionsDialog(Mundane, "Are you sure you want to learn " + args + "?", args,
                        new OptionsDataItem(0x0005, "Yes"),
                        new OptionsDataItem(0x0001, "No"));
                    break;
                // Skill Acquire
                case 0x0005:

                    var subject = ServerContext.GlobalSkillTemplateCache[args];
                    if (subject == null)
                        return;

                    if (subject.Prerequisites == null)
                    {
                        Skill.GiveTo(client, args);
                        client.SendOptionsDialog(Mundane, "Use this new skill wisely.");
                        client.Aisling.Show(Scope.NearbyAislings,
                            new ServerFormat29((uint)client.Aisling.Serial, (uint)Mundane.Serial, 0, 124, 64));
                    }
                    else
                    {
                        if (subject.Prerequisites.IsMet(client.Aisling))
                        {
                            Skill.GiveTo(client, args);

                            client.SendOptionsDialog(Mundane, "Use this new skill wisely.");
                            client.Aisling.Show(Scope.NearbyAislings,
                                new ServerFormat29((uint)client.Aisling.Serial, (uint)Mundane.Serial, 0, 124, 64));
                        }
                        else
                        {
                            client.SendOptionsDialog(Mundane, "You don't have what it takes.");
                            client.SendOptionsDialog(Mundane, string.Format("Requirements: {0}", subject.Prerequisites.ToString()));
                            client.CloseDialog();
                        }
                    }

                    break;
            }
        }
    }
}