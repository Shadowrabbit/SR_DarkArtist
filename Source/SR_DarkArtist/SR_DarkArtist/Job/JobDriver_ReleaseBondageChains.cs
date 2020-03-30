using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using SR.DA.Component;

namespace SR.DA.Job
{
    public class JobDriver_ReleaseBondageChains : JobDriver_UseItem
    {
        protected Verse.Thing Target
        {
            get
            {
                return job.GetTarget(TargetIndex.A).Thing;
            }
        }
        /// <summary>
        /// 保留犯人和束缚床
        /// </summary>
        /// <param name="errorOnFailed"></param>
        /// <returns></returns>
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(Target, job, 1, -1, null, errorOnFailed);
        }
        /// <summary>
        /// 行为过程
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnAggroMentalStateAndHostile(TargetIndex.A);//B精神不正常
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Pawn prisoner = (Pawn)Target;
            if (prisoner!=null && !prisoner.Dead)
            {
                yield return Toils_General.WaitWith(TargetIndex.A, 60, true, true); //交互1秒
                yield return Toils_Reserve.Release(TargetIndex.A);//释放
                yield return new Toil
                {
                    initAction = delegate ()
                    {
                        if (prisoner != null)
                        {
                            CompUsableRemoveEffectChians compUseEffect = prisoner.TryGetComp<CompUsableRemoveEffectChians>();//触发效果
                            if (compUseEffect != null)
                            {
                                compUseEffect.UsedBy(prisoner);
                                MoteMaker.ThrowText(prisoner.PositionHeld.ToVector3(), prisoner.MapHeld, "SR_Release".Translate(), 4f);
                            }
                        }
                    },
                    defaultCompleteMode = ToilCompleteMode.Instant
                };
            }
            yield break;
        }
    }
}
