using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using SR.DA.Thing;
namespace SR.DA.Component
{
    /// <summary>
    /// 束缚床交互组件
    /// </summary>
    public class CompUsableBondageBed : CompUsable
    {
        /// <summary>
        /// 菜单选项label
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        protected override string FloatMenuOptionLabel(Pawn pawn)
        {
            return "SR_CantUse".Translate();
        }
        /// <summary>
        /// 选项菜单
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn pawn)
        {
            Building_BondageBed bbb = parent as Building_BondageBed;
            if (bbb==null)
            {
                yield break;
            }
            //地图不存在
            if (pawn.Map == null || pawn.Map != Find.CurrentMap)
            {
                yield break;
            }
            //床被使用中
            if (!pawn.CanReach(this.parent, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel(pawn) + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
            }
            //无法保留
            else if (!pawn.CanReserve(this.parent, 1, -1, null, false))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel(pawn) + " (" + "Reserved".Translate() + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
            }
            //无法操作
            else if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel(pawn) + " (" + "Incapable".Translate() + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
            }
            //监管工作被禁用
            else if (pawn.WorkTagIsDisabled(WorkTypeDefOf.Warden.workTags))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel(pawn) + " (" + "SR_Forbid".Translate() + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
            }
            //存在使用者
            else if (bbb.occupant != null)
            {
                //囚犯被使用中
                if (!pawn.CanReserve(bbb.occupant, 1, -1, null, false))
                {
                    yield return new FloatMenuOption(this.FloatMenuOptionLabel(bbb.occupant) + " (" + "SR_Reserved".Translate(bbb.occupant.Label) + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
                }
                //解除束缚
                else
                {
                    Action action = delegate ()
                    {
                        TryReleasePrisoner(pawn, bbb.occupant);
                    };
                    string str = TranslatorFormattedStringExtensions.Translate("SR_Release_BondageBed", bbb.occupant.Label);
                    yield return new FloatMenuOption(str, action, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
                }
            }
            //不存在使用者
            else
            {
                bool hasPrisoner = false;
                foreach (Pawn prisoner in pawn.Map.mapPawns.AllPawns)
                {
                    //存在可用的囚犯
                    if (prisoner != pawn && prisoner.Spawned && prisoner.IsPrisonerOfColony)
                    {
                        hasPrisoner = true;
                        //囚犯被使用中
                        if (!pawn.CanReserve(prisoner, 1, -1, null, false))
                        {
                            yield return new FloatMenuOption(this.FloatMenuOptionLabel(prisoner) + " (" + "SR_Reserved".Translate(prisoner.Label) + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
                        }
                        //束缚囚犯
                        else
                        {
                            Action action = delegate ()
                            {
                                TryStartUseJob(pawn, prisoner);
                            };
                            string str = TranslatorFormattedStringExtensions.Translate("SR_BondageBed", pawn.Named(pawn.Name.ToString()), prisoner.Named(prisoner.Name.ToString()));
                            yield return new FloatMenuOption(str, action, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
                        }
                    }
                }
                //没有可用囚犯
                if (!hasPrisoner)
                {
                    yield return new FloatMenuOption(this.FloatMenuOptionLabel(pawn) + " (" + "SR_NoPrisoner".Translate() + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
                }
            }
        }
        /// <summary>
        /// 释放囚犯
        /// </summary>
        /// <param name="pawn">操作者</param>
        /// <param name="extraTarget">囚犯</param>
        private void TryReleasePrisoner(Pawn pawn, LocalTargetInfo extraTarget) {
            //无法保留 接触家具
            if (!pawn.CanReserveAndReach(parent, PathEndMode.Touch, Danger.Some, 1, -1, null, false))
            {
                return;
            }
            //无法保留 接触囚犯
            if (!pawn.CanReserveAndReach(extraTarget, PathEndMode.Touch, Danger.Some, 1, -1, null, false))
            {
                return;
            }
            //分配job
            Verse.AI.Job job = extraTarget.IsValid ? JobMaker.MakeJob(Job.JobDefOf.SR_ReleaseBed, parent, extraTarget) : JobMaker.MakeJob(Job.JobDefOf.SR_ReleaseBed, this.parent);
            job.count = 1;
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
        }
        /// <summary>
        /// job开始
        /// </summary>
        /// <param name="pawn">操作者</param>
        /// <param name="extraTarget">囚犯</param>
        public override void TryStartUseJob(Pawn pawn, LocalTargetInfo extraTarget)
        {
            //无法保留 接触家具
            if (!pawn.CanReserveAndReach(parent, PathEndMode.Touch, Danger.Some, 1, -1, null, false))
            {
                return;
            }
            //无法保留 接触囚犯
            if (!pawn.CanReserveAndReach(extraTarget, PathEndMode.Touch, Danger.Some, 1, -1, null, false))
            {
                return;
            }
            //分配job
            Verse.AI.Job job = extraTarget.IsValid ? JobMaker.MakeJob(this.Props.useJob, this.parent, extraTarget) : JobMaker.MakeJob(this.Props.useJob, this.parent);
            job.count = 1;
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
        }
    }
}
