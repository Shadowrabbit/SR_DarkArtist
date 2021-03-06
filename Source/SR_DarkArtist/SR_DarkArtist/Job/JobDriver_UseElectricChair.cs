﻿using System;
using System.Collections.Generic;
using RimWorld;
using SR.DA.Component;
using SR.DA.Thing;
using Verse;
using Verse.AI;

namespace SR.DA.Job
{
    public class JobDriver_UseElectricChair : JobDriver_UseItem
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
        /// 保留犯人和电椅
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
            Building_ElectricChair chair = (Building_ElectricChair)Thing;
            if (!prisoner.Dead)
            {
                Toil toilWaitWith = Toils_General.WaitWith(TargetIndex.A, 180, true, true); //交互3秒
                var cpt = chair.GetComp<CompPowerTrader>() ?? throw new Exception("cant find comp:CompPowerTrader");
                toilWaitWith.AddPreInitAction(()=> { chair.OnOrOff(true); });//启动电椅 提高电力负载
                toilWaitWith.AddFinishAction(()=> { chair.OnOrOff(false); });//恢复电力负载
                toilWaitWith.tickAction = delegate () {
                    if (!cpt.PowerOn)
                    {
                        ////Power interruption leads to { 0 } electrocution failure
                        Messages.Message("SR_ElectrocutionFailure".Translate(prisoner.Label), MessageTypeDefOf.NeutralEvent);
                        pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
                    }
                };
                yield return toilWaitWith; //交互
                yield return new Toil
                {
                    initAction = delegate ()
                    {
                        this.pawn.carryTracker.TryDropCarriedThing(this.Thing.Position, ThingPlaceMode.Direct, out Verse.Thing thing, null);//把囚犯扔下去
                    },
                    defaultCompleteMode = ToilCompleteMode.Instant
                };
            }
            yield return Toils_Reserve.Release(TargetIndex.A);//释放
            yield return Toils_Reserve.Release(TargetIndex.B);
            //家具的效果
            yield return new Toil
            {
                initAction = delegate ()
                {
                    if (Thing != null)
                    {
                        CompEffectElectricChair compUseEffect = Thing.TryGetComp<CompEffectElectricChair>();//触发电椅效果
                        if (compUseEffect != null)
                        {
                            compUseEffect.DoEffect(prisoner);
                            pawn.needs.mood.thoughts.memories.TryGainMemory(Thought.ThoughtDefOf.SR_Thought_Maltreat);
                            MoteMaker.ThrowText(Target.PositionHeld.ToVector3(), Target.MapHeld, "SR_ElectricShock".Translate(), 4f);
                        }
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield break;
        }
    }
}
