using RimWorld;
using Verse;

namespace SR.DA.Component
{
    /// <summary>
    /// 电磁熊手触发效果组件
    /// </summary>
    public class CompEffectElectromagneticBearHand : CompUseEffect
    {
        private static readonly float dmgAmount = 1f;
        /// <summary>
        /// 作用效果
        /// </summary>
        /// <param name="usedBy"></param>
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            usedBy.needs.mood.thoughts.memories.TryGainMemory(Thought.ThoughtDefOf.SR_Thought_Mistreated);
            var damageInfo = new DamageInfo(Damage.DamageDefOf.SR_Damage_ElecticShock, dmgAmount);
            usedBy.TakeDamage(damageInfo);
        }
    }
}
