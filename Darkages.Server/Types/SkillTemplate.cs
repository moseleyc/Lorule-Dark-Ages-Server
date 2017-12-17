namespace Darkages.Types
{
    public class SkillTemplate : Template
    {
        public int ID { get; set; }
        public byte Status { get; set; }
        public byte Icon { get; set; }

        public string Desciption { get; set; }
        public string ScriptName { get; set; }

        public double Exponent { get; set; }
        public double Coponent { get; set; }
        public double Factor { get; set; }
        public double LevelRate { get; set; }
        public double SuccessRate { get; set; }

        public byte Level { get; set; }
        public int Cooldown { get; set; }
        public int MaxLevel { get; set; }

        public ushort SuccessAnimation { get; set; }
        public ushort FailureAnimation { get; set; }
        public ushort MissAnimation { get; set; }
        public ushort TargetAnimation { get; set; }
        public string FailMessage { get; set; }
        public string SuccessMessage { get; set; }
        public string LevelUpMessage { get; set; }

        public SkillScope Type { get; set; }
        public SkillModifiers Modifiers { get; set; }
        public SkillPane Pane { get; set; }
        public TargetQualifiers TargetQualifiers { get; set; }
        public SKillTier Tier { get; set; }
        public PreQualifer PreQualifers { get; set; }
        public PostQualifer PostQualifers { get; set; }
        public byte Sound { get; set; }

        public Debuff Debuff { get; set; }
        public Buff Buff { get; set; }


        public SkillTemplate()
        {
        }
    }
}