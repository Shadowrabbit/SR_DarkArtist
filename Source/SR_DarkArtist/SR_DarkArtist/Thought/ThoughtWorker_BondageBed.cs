using RimWorld;
using Verse;
using SR.DA.Thing;

namespace SR.DA.Thought
{
    /// <summary>
    /// 束缚床对殖民者想法的影响
    /// </summary>
    public class ThoughtWorker_BondageBed:ThoughtWorker{
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            Building_Bed bed = p.CurrentBed() as Building_AdvancedBondageBed;
            if (bed==null)
            {
                bed = p.CurrentBed() as Building_BondageBed;
                if (bed==null)
                {
                    return ThoughtState.Inactive;//不激活这个想法
                }
                //殖民者躺在束缚床
                else
                {
                    return ThoughtState.ActiveAtStage(1);
                }
            }
            //殖民者躺在高级束缚床
            else
            {
                return ThoughtState.ActiveAtStage(0);
            }
        }
    }
}
