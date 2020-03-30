using RimWorld;
using Verse;
using SR.DA.Thing;

namespace SR.DA.Component
{
    public class CompEffectBondageBed: CompUseEffect
    {
        /// <summary>
        /// 作用效果 束缚
        /// </summary>
        /// <param name="usedBy"></param>
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            Building_BondageBed building_BondageBed = (Building_BondageBed)parent;
            building_BondageBed.SetOccupant(usedBy);//设置使用者
            HediffDef hediffBed = Hediff.HediffDefOf.SR_BondageBed;
            var arms = usedBy.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Arm);
            if (arms!=null)
            {
                foreach (BodyPartRecord bpr in arms)
                {
                    //该部位没有缺失
                    if (bpr != null && !usedBy.health.hediffSet.PartIsMissing(bpr))
                    {
                        usedBy.health.AddHediff(hediffBed, bpr, null, null);
                    }
                }
            }
            var legs = usedBy.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Leg);
            if (legs!=null)
            {
                foreach (BodyPartRecord bpr in legs)
                {
                    if (bpr != null && !usedBy.health.hediffSet.PartIsMissing(bpr))
                    {
                        usedBy.health.AddHediff(hediffBed, bpr, null, null);
                    }
                }
            }
        }
    }
}
