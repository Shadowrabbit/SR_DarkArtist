using System;
using RimWorld;
using Verse;
using SR.DA.Thing;
namespace SR.DA.Component
{
    public class CompRemoveEffectBondageChains : CompUseEffect
    {
        /// <summary>
        /// 作用效果 束缚
        /// </summary>
        /// <param name="usedBy"></param>
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            Building_BondageBed building_BondageBed = (Building_BondageBed)parent;
            HediffDef hediffChains = Hediff.HediffDefOf.SR_BondageChains;
            foreach (var hediff in usedBy.health.hediffSet.hediffs)
            {
                if (hediff.def.defName.Equals(hediffChains.defName))
                {
                    usedBy.health.RemoveHediff(hediff);
                }
            }
            building_BondageBed.RemoveOccupant();
        }
    }
}
