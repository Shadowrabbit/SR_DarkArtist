using RimWorld;
using Verse;

namespace SR.DA.Component
{
    /// <summary>
    /// 鞭子触发效果组件
    /// </summary>
    public class CompEffectDarkWhip : CompUseEffect
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
            var damageInfo = new DamageInfo(DamageDefOf.Crush, dmgAmount);
            usedBy.TakeDamage(damageInfo);
        }
    }
}
