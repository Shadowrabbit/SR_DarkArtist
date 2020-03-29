using System;
using RimWorld;
using Verse;
using SR.DA.Thing;
using System.Collections.Generic;
using System.Linq;

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
            bool isFinishedRemove = false;
            List<Verse.Hediff>.Enumerator enumerator;
            enumerator = (from x in usedBy.health.hediffSet.hediffs where x.def == hediffBed select x).ToList<Verse.Hediff>().GetEnumerator();//获取小人身上所有hediffBed
            while (enumerator.MoveNext())
            {
                Verse.Hediff h = enumerator.Current;//当前的hediffBed
                usedBy.health.RemoveHediff(h);
            }
            building_BondageBed.RemoveOccupant();
            MoteMaker.ThrowText(usedBy.PositionHeld.ToVector3(), usedBy.MapHeld, "SR_Release".Translate(), 4f);
        }
    }
}
