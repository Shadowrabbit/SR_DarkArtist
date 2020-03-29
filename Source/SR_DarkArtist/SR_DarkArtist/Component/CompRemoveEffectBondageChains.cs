using RimWorld;
using Verse;
using SR.DA.Thing;
using System.Linq;
using System.Collections.Generic;

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
            List<Verse.Hediff>.Enumerator enumerator;
            enumerator = (from x in usedBy.health.hediffSet.hediffs where x.def == hediffChains select x).ToList().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Verse.Hediff h = enumerator.Current;
                usedBy.health.RemoveHediff(h);
            }
            building_BondageBed.RemoveOccupant();
            MoteMaker.ThrowText(usedBy.PositionHeld.ToVector3(), usedBy.MapHeld, "SR_Release".Translate(), 4f);
        }
    }
}
