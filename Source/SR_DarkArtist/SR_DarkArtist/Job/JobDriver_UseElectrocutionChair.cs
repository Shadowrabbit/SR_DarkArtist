using System;
using System.Collections.Generic;
using RimWorld;
using SR.DA.Component;
using SR.DA.Thing;
using Verse;
using Verse.AI;

namespace SR.DA.Job
{
    public class JobDriver_UseElectrocutionChair : JobDriver_UseItem
    {
        protected Verse.Thing thing
        {
            get
            {
                return job.GetTarget(TargetIndex.A).Thing;
            }
        }
        protected Verse.Thing target
        {
            get
            {
                return job.GetTarget(TargetIndex.B).Thing;
            }
        }
        /// <summary>
        /// 保留犯人和电椅
        /// </summary>
        /// <param name="errorOnFailed"></param>
        /// <returns></returns>
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(thing, job, 1, -1, null, errorOnFailed) && this.pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
        }
        /// <summary>
        /// 行为过程
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.B);
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);//电椅被禁止使用
            this.FailOnAggroMentalStateAndHostile(TargetIndex.B);//B精神不正常
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch);//走到囚犯身边
            Toil toil = Toils_Haul.StartCarryThing(TargetIndex.B, false, false, false);//搬运囚犯
            yield return toil;
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnForbidden(TargetIndex.A);//走到dark家具旁边
            yield return new Toil
            {
                initAction = delegate ()
                {
                    this.pawn.carryTracker.TryDropCarriedThing(this.thing.Position, ThingPlaceMode.Direct, out Verse.Thing thing, null);//把囚犯扔下去
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            Pawn prisoner = (Pawn)target;
            Building_ElectrocutionChair chair = (Building_ElectrocutionChair)thing;
            if (!prisoner.Dead)
            {
                Toil toilWaitWith = Toils_General.WaitWith(TargetIndex.B, 180, true, true); //交互3秒
                var cpt = chair.GetComp<CompPowerTrader>() ?? throw new Exception("cant find comp:CompPowerTrader");
                toilWaitWith.AddPreInitAction(()=> { chair.OnOrOff(true); });
                toilWaitWith.AddFinishAction(()=> { chair.OnOrOff(false); });
                toilWaitWith.tickAction = delegate () {
                    if (!cpt.PowerOn)
                    {
                        ////Power interruption leads to { 0 } electrocution failure
                        ///电力中断导致{0}电刑失败
                        Messages.Message("SR_ElectrocutionFailure".Translate(prisoner.Label), MessageTypeDefOf.NeutralEvent);
                        pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
                    }
                };
                yield return toilWaitWith; //交互1秒
            }
            yield return Toils_Reserve.Release(TargetIndex.A);//释放
            yield return Toils_Reserve.Release(TargetIndex.B);
            //家具的效果
            yield return new Toil
            {
                initAction = delegate ()
                {
                    if (thing != null)
                    {
                        CompEffectElectrocutionChair compUseEffect = thing.TryGetComp<CompEffectElectrocutionChair>();//触发电椅效果
                        if (compUseEffect != null)
                        {
                            compUseEffect.DoEffect(prisoner);
                        }
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield break;
        }
    }
}
