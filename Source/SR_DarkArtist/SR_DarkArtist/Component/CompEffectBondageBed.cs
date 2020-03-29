using System;
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
            foreach (BodyPartRecord bpr in usedBy.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Arm))
            {
                if (bpr!=null)
                {
                    usedBy.health.AddHediff(hediffBed, bpr, null, null);
                }
            }
            foreach (BodyPartRecord bpr in usedBy.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Leg))
            {
                if (bpr!=null)
                {
                    usedBy.health.AddHediff(hediffBed, bpr, null, null);
                }
            }
            MoteMaker.ThrowText(usedBy.PositionHeld.ToVector3(), usedBy.MapHeld, "SR_Bondage".Translate(), 4f);
        }
    }
}
