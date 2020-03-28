using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
namespace SR.DA.Component
{
    public class CompUsableElectrocutionChair : CompUsable
    {
        /// <summary>
        /// 菜单选项label
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        protected override string FloatMenuOptionLabel(Pawn pawn)
        {
            return "SR_CantTeach".Translate();
        }
        /// <summary>
        /// 选项菜单
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn pawn)
        {
            //地图不存在
            if (pawn.Map == null || pawn.Map != Find.CurrentMap)
            {
                yield break;
            }
            //无法接触
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
                yield return new FloatMenuOption(this.FloatMenuOptionLabel(pawn) + " (" + "SR_CantWork".Translate() + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
            }
            else
            {
                bool hasPrisoner = false;
                foreach (Pawn prisoner in pawn.Map.mapPawns.AllPawns)
                {
                    if (prisoner != pawn && prisoner.Spawned && prisoner.IsPrisonerOfColony)
                    {
                        hasPrisoner = true;
                        Action action = delegate ()
                        {
                            TryStartUseJob(pawn, prisoner);
                        };
                        string str = TranslatorFormattedStringExtensions.Translate("SR_ElectocutionChair", pawn.Named(pawn.Name.ToString()), prisoner.Named(prisoner.Name.ToString()));
                        yield return new FloatMenuOption(str, action, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
                    }
                }
                if (!hasPrisoner)
                {
                    yield return new FloatMenuOption(this.FloatMenuOptionLabel(pawn) + " (" + "SR_NoPrisoner".Translate() + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
                }
            }
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
            //取消禁用
            CompForbiddable comp = parent.GetComp<CompForbiddable>();
            if (comp != null)
            {
                comp.Forbidden = false;
            }
            //分配job
            Verse.AI.Job job = extraTarget.IsValid ? JobMaker.MakeJob(this.Props.useJob, this.parent, extraTarget) : JobMaker.MakeJob(this.Props.useJob, this.parent);
            job.count = 1;
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
        }
    }
}
