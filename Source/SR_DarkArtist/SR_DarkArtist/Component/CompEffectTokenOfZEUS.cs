using RimWorld;
using Verse;

namespace SR.DA.Component
{
    /// <summary>
    /// zeus令牌触发效果组件
    /// </summary>
    public class CompEffectTokenOfZEUS : CompUseEffect
    {
        /// <summary>
        /// 作用效果
        /// </summary>
        /// <param name="usedBy"></param>
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            usedBy.guest.resistance = 0f;
            parent.SplitOff(1).Destroy(DestroyMode.Vanish);
            Messages.Message("SR_BrainWashing".Translate(usedBy.Label), MessageTypeDefOf.NeutralEvent);
        }
    }
}
