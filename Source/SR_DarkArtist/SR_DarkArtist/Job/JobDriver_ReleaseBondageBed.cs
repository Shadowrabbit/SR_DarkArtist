using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using SR.DA.Component;

namespace SR.DA.Job
{
    public class JobDriver_ReleaseBondageBed : JobDriver_UseItem
    {
        protected Verse.Thing Thing
        {
            get
            {
                return job.GetTarget(TargetIndex.A).Thing;
            }
        }
        protected Verse.Thing Target
        {
            get
            {
                return job.GetTarget(TargetIndex.B).Thing;
            }
        }
        /// <summary>
        /// 保留犯人和束缚床
        /// </summary>
        /// <param name="errorOnFailed"></param>
        /// <returns></returns>
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            //当殖民者睡在床上时，床会被laydown保留，其他工作无法再次保留床
            return pawn.Reserve(Target, job, 1, -1, null, errorOnFailed);
        }
        /// <summary>
        /// 行为过程
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.B);
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);//床被禁止使用
            this.FailOnAggroMentalStateAndHostile(TargetIndex.B);//B精神不正常
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnForbidden(TargetIndex.A);//走到dark家具旁边
            Pawn prisoner = (Pawn)Target;
            //捆绑操作
            if (!prisoner.Dead)
            {
                yield return Toils_General.WaitWith(TargetIndex.A, 60, true, true); //交互1秒
                yield return Toils_Reserve.Release(TargetIndex.B);
                //解除效果
                yield return new Toil
                {
                    initAction = delegate ()
                    {
                        if (Thing != null)
                        {
                            CompRemoveEffectBondageBed compUseEffect = Thing.TryGetComp<CompRemoveEffectBondageBed>();//解除束缚床效果
                            if (compUseEffect != null)
                            {
                                compUseEffect.DoEffect(prisoner);
                                MoteMaker.ThrowText(Target.PositionHeld.ToVector3(), Target.MapHeld, "SR_Release".Translate(), 4f);
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
