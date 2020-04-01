using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using SR.DA.Thing;

namespace SR.DA.Component
{
    public class CompUsableRemoveEffectChians :CompUsable{
        private bool isBondaged = false;//小人已经被捆绑了
        /// <summary>
        /// 菜单选项label
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        protected override string FloatMenuOptionLabel(Pawn pawn)
        {
            return "SR_CantRemove".Translate();
        }
        /// <summary>
        /// 选项
        /// </summary>
        /// <param name="myPawn"></param>
        /// <returns></returns>
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn myPawn)
        {
            if (!isBondaged)
            {
                yield break;
            }
            //地图不存在
            if (myPawn.Map == null || myPawn.Map != Find.CurrentMap)
            {
                yield break;
            }
            //无法接触
            if (!myPawn.CanReach(this.parent, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel(myPawn) + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
            }
            //无法保留
            else if (!myPawn.CanReserve(this.parent, 1, -1, null, false))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel(myPawn) + " (" + "Reserved".Translate() + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
            }
            //无法操作
            else if (!myPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel(myPawn) + " (" + "Incapable".Translate() + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
            }
            //监管工作被禁用
            else if (myPawn.WorkTagIsDisabled(WorkTypeDefOf.Warden.workTags))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel(myPawn) + " (" + "SR_CantWork".Translate() + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
            }
            else
            {
                Action action = delegate ()
                {
                    TryStartUseJob(myPawn, parent);
                };
                string str = TranslatorFormattedStringExtensions.Translate("SR_Release_BondageChains", parent.Label);
                yield return new FloatMenuOption(str, action, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
            }
        }
        /// <summary>
        /// 序列化
        /// </summary>
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref isBondaged, "isBondaged", false);
        }
        /// <summary>
        /// 分配job
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="extraTarget"></param>
        public override void TryStartUseJob(Pawn pawn, LocalTargetInfo extraTarget)
        {
            if (!pawn.CanReserveAndReach(extraTarget, PathEndMode.Touch, Danger.Some, 1, -1, null, false))
            {
                return;
            }
            //分配job
            Verse.AI.Job job = JobMaker.MakeJob(this.Props.useJob, extraTarget);
            job.count = 1;
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
        }
        /// <summary>
        /// 组件之间交互
        /// </summary>
        /// <param name="signal"></param>
        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);
            if (signal.Equals("SR_IsBondaged"))
            {
                isBondaged = true;
            }
        }
        /// <summary>
        /// 被使用的效果
        /// </summary>
        /// <param name="usedBy"></param>
        public new void UsedBy(Pawn usedBy) {
            HediffDef hediff = Hediff.HediffDefOf.SR_BondageChains;
            List<Verse.Hediff>.Enumerator enumerator;
            enumerator = (from x in usedBy.health.hediffSet.hediffs where x.def == hediff select x).ToList().GetEnumerator();//获取小人身上所有hediffBed
            while (enumerator.MoveNext())
            {
                Verse.Hediff h = enumerator.Current;//当前的hediff
                usedBy.health.RemoveHediff(h);
            }
            var thing = ThingMaker.MakeThing(Thing.ThingDefOf.SR_Chains);
            thing.stackCount = 1;
            GenPlace.TryPlaceThing(thing, usedBy.Position, usedBy.Map, ThingPlaceMode.Near);
            isBondaged = false;
        }
    }
}
