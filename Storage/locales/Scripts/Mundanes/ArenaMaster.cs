using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;
using System.Collections.Generic;

namespace Darkages.Storage.locales.Scripts.Mundanes
{
    [Script("Arena Master")]
    public class ArenaMaster: MundaneScript
    {
        NormalPopup dialogs = null;


        public ArenaMaster(GameServer server, Mundane mundane)
            : base(server, mundane)
        {
            var Steps = new List<Step>();
            Steps.Add(new Step() { Body = "Go. kill them all!", ScriptId = 0x0004, Serial = (uint)Mundane.Serial, StepId = 0x0004, Title = this.Mundane.Template.Name, HasBack = false, HasNext = false, Image = (ushort)Mundane.Template.Image });

            dialogs = new NormalPopup()
            {
                Steps = Steps,
                CurrentStep = 0,
                TotalSteps = Steps.Count,
            };

        }

        public override void OnClick(GameServer server, GameClient client)
        {
            if (client.Aisling.Dead)
            {
                var options = new List<OptionsDataItem>();
                options.Add(new OptionsDataItem(0x0001, "Yes."));
                options.Add(new OptionsDataItem(0x0002, "No."));

                client.SendOptionsDialog(base.Mundane, "You seek redemption?", options.ToArray());
            }
            else
            {
                if (dialogs != null)
                {
                    dialogs.CurrentStep = 0;

                    var step = dialogs.Steps[0];
                    if (step != null)
                        client.Send(new ServerFormat30(step));
                }
            }
        }
        public override void OnResponse(GameServer server, GameClient client, short responseID, string args)
        {
            if (responseID == 0x0001)
            {
                client.SendOptionsDialog(base.Mundane, "Beg for Life.",
                                        new OptionsDataItem(0x0005, "Please my lord."),
                                        new OptionsDataItem(0x0001, "No, Fuck you."));
            }

            if (responseID == 0x0005)
            {
                client.Aisling.CurrentHp = client.Aisling.MaximumHp;
                client.SendStats(StatusFlags.All);
            }
        }
    }
}