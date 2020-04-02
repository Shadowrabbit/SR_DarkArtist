using Verse;

namespace SR.DA.Component
{
    /// <summary>
    /// 高电压组件参数
    /// </summary>
    public class CompProperties_HighVoltage : CompProperties
    {
        public CompProperties_HighVoltage()
        {
            compClass = typeof(CompHighVoltage);
        }
        [NoTranslate]
        public string commandTexture = "UI/Commands/HighVoltage";
        [NoTranslate]
        public string commandLabelKey = "SR_CommandDesignateHighVoltageLabel";
        [NoTranslate]
        public string commandDescKey = "SR_CommandDesignateHighVoltageDesc";
    }
}
