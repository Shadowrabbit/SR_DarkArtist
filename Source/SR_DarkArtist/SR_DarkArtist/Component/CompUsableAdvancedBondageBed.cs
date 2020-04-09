using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using SR.DA.Thing;
namespace SR.DA.Component
{
    /// <summary>
    /// 高级束缚床交互组件
    /// </summary>
    public class CompUsableAdvancedBondageBed : CompUsableBondageBed
    {
        /// <summary>
        /// 选项菜单
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn pawn)
        {
            Building_BondageBed bbb = parent as Building_BondageBed;
            if (bbb == null)
            {
                yield break;
            }
            //地图不存在
            if (pawn.Map == null || pawn.Map != Find.CurrentMap)
            {
                yield break;
            }
            //无法接触床
            if (!pawn.CanReach(this.parent, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel(pawn) + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
            }
            ////无法保留
            //else if (!pawn.CanReserve(this.parent, 1, -1, null, false))
            //{
            //    yield return new FloatMenuOption(this.FloatMenuOptionLabel(pawn) + " (" + "Reserved".Translate() + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
            //}
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
                //使用者被使用中
                if (!pawn.CanReserve(bbb.occupant, 1, -1, null, false))
                {
                    yield return new FloatMenuOption(this.FloatMenuOptionLabel(bbb.occupant) + " (" + "SR_Reserved".Translate() + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
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
                bool hasTarget = false;
                foreach (Pawn target in pawn.Map.mapPawns.AllPawns)
                {
                    //存在可用的囚犯或殖民者
                    if (target != pawn && target.Spawned && target.IsColonist && !target.IsPrisoner)
                    {
                        hasTarget = true;
                        //目标被使用中
                        if (!pawn.CanReserve(target, 1, -1, null, false))
                        {
                            yield return new FloatMenuOption(this.FloatMenuOptionLabel(target) + " (" + "SR_Reserved".Translate(target.Label) + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
                        }
                        //束缚目标
                        else
                        {
                            Action action = delegate ()
                            {
                                TryStartUseJob(pawn, target);
                            };
                            string str = TranslatorFormattedStringExtensions.Translate("SR_BondageBed", pawn.Named(pawn.Name.ToString()), target.Named(target.Name.ToString()));
                            yield return new FloatMenuOption(str, action, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
                        }
                    }
                }
                //没有可用囚犯
                if (!hasTarget)
                {
                    yield return new FloatMenuOption(this.FloatMenuOptionLabel(pawn) + " (" + "SR_NoTarget".Translate() + ")", null, MenuOptionPriority.DisabledOption, null, null, 0f, null, null);
                }
            }
        }
    }
}
