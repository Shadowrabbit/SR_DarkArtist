using RimWorld;
using Verse;

namespace SR.DA.Component
{
    public class CompEffectBondageChains : CompUseEffect
    {
        [NoTranslate]
        private static readonly string signal = "SR_IsBondaged";
        /// <summary>
        /// 作用效果 捆绑
        /// </summary>
        /// <param name="usedBy"></param>
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            HediffDef h = Hediff.HediffDefOf.SR_BondageChains;
            var arms = usedBy.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Arm);
            bool isBondaged = false;//是否成功捆绑
            if (arms != null)
            {
                foreach (BodyPartRecord bpr in arms)
                {
                    //该部位没有缺失
                    if (bpr != null && !usedBy.health.hediffSet.PartIsMissing(bpr))
                    {
                        usedBy.health.AddHediff(h, bpr, null, null);
                        isBondaged = true;
                    }
                }
            }
            var legs = usedBy.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Leg);
            if (legs != null)
            {
                foreach (BodyPartRecord bpr in legs)
                {
                    if (bpr != null && !usedBy.health.hediffSet.PartIsMissing(bpr))
                    {
                        usedBy.health.AddHediff(h, bpr, null, null);
                        isBondaged = true;
                    }
                }
            }
            if (isBondaged)
            {
                usedBy.BroadcastCompSignal(signal);
                parent.SplitOff(1).Destroy(DestroyMode.Vanish);
            }
            else
            {
                Messages.Message("fail." + usedBy + "dont have arms and legs.", MessageTypeDefOf.NeutralEvent);//缺失全部部位 无法使用锁链
            }
        }
    }
}
