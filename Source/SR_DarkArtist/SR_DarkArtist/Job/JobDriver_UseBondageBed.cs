using System.Collections.Generic;
using RimWorld;
using SR.DA.Component;
using Verse;
using Verse.AI;

namespace SR.DA.Job
{
    public class JobDriver_UseBondageBed : JobDriver_UseItem
    {
        protected Verse.Thing Thing {
            get {
                return job.GetTarget(TargetIndex.A).Thing;
            }
        }
        protected Verse.Thing Target {
            get {
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
            return this.pawn.Reserve(Thing, job, 1, -1, null, errorOnFailed) && this.pawn.Reserve(Target, job, 1, -1, null, errorOnFailed);
        }
        /// <summary>
        /// 行为过程
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.B);
            this.FailOnAggroMentalStateAndHostile(TargetIndex.B);//B精神不正常
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch);//走到囚犯身边
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, false, false);//搬运囚犯
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnForbidden(TargetIndex.A);//走到dark家具旁边
            Pawn prisoner = (Pawn)Target;
            //捆绑操作
            if (!prisoner.Dead)
            {
                yield return Toils_General.WaitWith(TargetIndex.A, 60, true, true); //交互1秒
                yield return new Toil
                {
                    initAction = delegate ()
                    {
                        this.pawn.carryTracker.TryDropCarriedThing(this.Thing.Position, ThingPlaceMode.Direct, out Verse.Thing thing, null);//把囚犯扔下去
                    },
                    defaultCompleteMode = ToilCompleteMode.Instant
                };
                yield return Toils_Reserve.Release(TargetIndex.A);//释放
                yield return Toils_Reserve.Release(TargetIndex.B);
                //家具的效果
                yield return new Toil
                {
                    initAction = delegate ()
                    {
                        if (Thing != null)
                        {
                            CompEffectBondageBed compUseEffect = Thing.TryGetComp<CompEffectBondageBed>();//触发束缚效果
                            if (compUseEffect != null)
                            {
                                compUseEffect.DoEffect(prisoner);
                                MoteMaker.ThrowText(Target.PositionHeld.ToVector3(), Target.MapHeld, "SR_Bondage".Translate(), 4f);
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
