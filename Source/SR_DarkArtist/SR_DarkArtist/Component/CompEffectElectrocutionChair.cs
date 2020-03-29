using RimWorld;
using Verse;

namespace SR.DA.Component
{
    public class CompEffectElectrocutionChair : CompUseEffect
    {
        /// <summary>
        /// 作用效果 电击
        /// </summary>
        /// <param name="usedBy"></param>
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            var damageInfo = new DamageInfo(Damage.DamageDefOf.SR_DamageElectrocution,5);
            usedBy.TakeDamage(damageInfo);
            MoteMaker.ThrowText(usedBy.PositionHeld.ToVector3(), usedBy.MapHeld, "电击".Translate(), 12f);
        }
    }
}
