﻿using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using SR.DA.Thing;
using UnityEngine;

namespace SR.DA.Job
{
    /// <summary>
    /// 刑具使用行为
    /// </summary>
    public class JobDriver_UseTorture : JobDriver_UseItem
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
            if (!prisoner.Dead)
            {
                yield return Toils_General.WaitWith(TargetIndex.B, 60, true, true); //交互1秒
                yield return Toils_Reserve.Release(TargetIndex.A);//释放
                yield return Toils_Reserve.Release(TargetIndex.B);
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
                                //获得快感
                                pawn.needs.mood.thoughts.memories.TryGainMemory(Thought.ThoughtDefOf.SR_Thought_Maltreat);
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
