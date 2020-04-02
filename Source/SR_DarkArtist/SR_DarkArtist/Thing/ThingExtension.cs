using RimWorld;
using SR.DA.Component;
using Verse;


namespace SR.DA.Thing
{
    public static class ThingExtension
    {
        /// <summary>
        /// 小人身上是否存在锁链
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public static bool HasChains(this Pawn pawn) {
            CompUsableRemoveEffectChians comp = pawn.GetComp<CompUsableRemoveEffectChians>()?? throw new System.Exception("cant find comp:CompUsableRemoveEffectChians");
            if (comp!=null)
            {
                return comp.IsBondaged;
            }
            return false;
        }
    }
}
