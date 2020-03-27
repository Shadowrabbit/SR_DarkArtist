using System;
using RimWorld;
using Verse;
using SR.DA.Thing;

namespace SR.DA.Component
{
    public class CompRemoveEffectBondageBed : CompUseEffect
    {
        /// <summary>
        /// 作用效果 束缚
        /// </summary>
        /// <param name="usedBy"></param>
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            Building_BondageBed building_BondageBed = (Building_BondageBed)parent;
            HediffDef hediffBed = Hediff.HediffDefOf.SR_BondageBed;
            for (int i = 0; i < usedBy.health.hediffSet.hediffs.Count; i++)
            {
                if (usedBy.health.hediffSet.hediffs[i].def.defName.Equals(hediffBed.defName))
                {
                    usedBy.health.RemoveHediff(usedBy.health.hediffSet.hediffs[i]);
                }
            }
            building_BondageBed.RemoveOccupant();
        }
    }
}
