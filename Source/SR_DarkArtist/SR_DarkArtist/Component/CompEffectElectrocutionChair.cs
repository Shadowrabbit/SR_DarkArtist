using RimWorld;
using Verse;

namespace SR.DA.Component
{
    public class CompEffectElectrocutionChair : CompUseEffect
    {
        /// <summary>
        /// 作用效果 麻痹
        /// </summary>
        /// <param name="usedBy"></param>
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            HediffDef h = Hediff.HediffDefOf.SR_ElectrocutionChair;
            usedBy.health.AddHediff(h, null, null, null);
        }
    }
}
