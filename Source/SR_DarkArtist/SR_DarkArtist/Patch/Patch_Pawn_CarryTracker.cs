﻿using HarmonyLib;
using Verse;
using RimWorld;
using SR.DA.Thing;
using SR.DA.Component;
using System;

class Patch_Pawn_CarryTracker
{
    [HarmonyPatch(typeof(Pawn_CarryTracker), "TryStartCarry", new Type[] { typeof(Thing), typeof(int), typeof(bool) })]
    class Patch1
    {
        /// <summary>
        /// 搬运小人时检测小人是否在束缚床上，在的话进行解绑
        /// </summary>
        /// <param name="__result"></param>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <param name="reserve"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        static bool Prefix(ref int __result, Thing item, int count, bool reserve = true)
        {
            if (item!=null)
            {
                if (item.GetType() == typeof(Pawn))
                {
                    Pawn p = (Pawn)item;//搬运的是人形
                    bool hasBondageBed = false;//没有被束缚床束缚
                    for (int i = 0; i < p.health.hediffSet.hediffs.Count; i++)
                    {
                        if (p.health.hediffSet.hediffs[i].def == SR.DA.Hediff.HediffDefOf.SR_BondageBed)
                        {
                            hasBondageBed = true;
                            break;
                        }
                    }
                    //如果已经被束缚
                    if (hasBondageBed)
                    {
                        Building_BondageBed bbb = (Building_BondageBed)p.CurrentBed();//获取当前躺着的束缚床
                        CompRemoveEffectBondageBed crebb = bbb.GetComp<CompRemoveEffectBondageBed>();
                        if (crebb != null)
                        {
                            crebb.DoEffect(p);//解除束缚
                        }
                    }
                }
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Pawn_CarryTracker), "TryStartCarry", new Type[] { typeof(Thing) })]
    class Patch2
    {
        [HarmonyPrefix]
        static bool Prefix(ref bool __result, Thing item)
        {
            if (item!=null)
            {
                if (item.GetType() == typeof(Pawn))
                {
                    Pawn p = (Pawn)item;//搬运的是人形
                    bool hasBondageBed = false;//没有被束缚床束缚
                    for (int i = 0; i < p.health.hediffSet.hediffs.Count; i++)
                    {
                        if (p.health.hediffSet.hediffs[i].def == SR.DA.Hediff.HediffDefOf.SR_BondageBed)
                        {
                            hasBondageBed = true;
                            break;
                        }
                    }
                    //如果已经被束缚
                    if (hasBondageBed)
                    {
                        Building_BondageBed bbb = (Building_BondageBed)p.CurrentBed();//获取当前躺着的束缚床
                        CompRemoveEffectBondageBed crebb = bbb.GetComp<CompRemoveEffectBondageBed>();
                        if (crebb != null)
                        {
                            crebb.DoEffect(p);//解除束缚
                        }
                    }
                }
            }
            return true;
        }
    }
}