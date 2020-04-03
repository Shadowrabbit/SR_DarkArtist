using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using SR.DA.Thing;

namespace SR.DA.Job
{
    public class JobDriver_UseBondageChains : JobDriver_UseItem
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
        /// 保留犯人和道具
        /// </summary>
        /// <param name="errorOnFailed"></param>
        /// <returns></returns>
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(Thing, job, 1, -1, null, errorOnFailed) && this.pawn.Reserve(Target, job, 1, -1, null, errorOnFailed);
        }
        /// <summary>
        /// 行为过程
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Toil> MakeNewToils()
        {
            Pawn prisoner = (Pawn)Target;
            //小人身上已经存在锁链
            if (prisoner.HasChains())
            {
                yield break;
            }
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.B);
            //this.FailOnDespawnedNullOrForbidden(TargetIndex.A);//如果物品没有forbidden组件千万不要用这个条件，会直接判断失败
            this.FailOnAggroMentalStateAndHostile(TargetIndex.B);//B精神不正常
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);//走到道具
            yield return new Toil
            {
                initAction = () => { pawn.carryTracker.TryStartCarry(Thing, 1, true); },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch);//走到囚犯
            //捆绑操作
            if (!prisoner.Dead)
            {
                yield return Toils_General.WaitWith(TargetIndex.B, 60, true, true); //交互1秒
                yield return Toils_Reserve.Release(TargetIndex.A);//释放
                yield return Toils_Reserve.Release(TargetIndex.B);
                //家具的效果
                yield return new Toil
                {
                    initAction = delegate ()
                    {
                        if (Thing != null)
                        {
                            CompUseEffect compUseEffect = Thing.TryGetComp<CompUseEffect>();
                            if (compUseEffect != null)
                            {
                                compUseEffect.DoEffect(prisoner);
                                MoteMaker.ThrowText(Target.PositionHeld.ToVector3(), Target.MapHeld, "SR_Bound".Translate(), 4f);
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
