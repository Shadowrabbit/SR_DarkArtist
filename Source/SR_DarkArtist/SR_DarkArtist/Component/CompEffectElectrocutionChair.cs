using RimWorld;
using Verse;

namespace SR.DA.Component
{
    public class CompEffectElectrocutionChair : CompUseEffect
    {
        private static readonly float dmgAmount = 5f;
        public float DmgAmount { get; set; }
        /// <summary>
        /// 作用效果 电击
        /// </summary>
        /// <param name="usedBy"></param>
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            var damageInfo = new DamageInfo(Damage.DamageDefOf.SR_DamageElectrocution, DmgAmount);
            usedBy.TakeDamage(damageInfo);
        }
        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);
            //高压电模式
            if (signal.Equals("HighVoltageOn"))
            {
                DmgAmount = dmgAmount * 10;
                Messages.Message("SR_HighVoltageOn".Translate(), MessageTypeDefOf.NeutralEvent);
            }
            //普通模式
            else if (signal.Equals("HighVoltageOff"))
            {
                DmgAmount = dmgAmount;
                Messages.Message("SR_HighVoltageOff".Translate(), MessageTypeDefOf.NeutralEvent);
            }
        }
    }
}
