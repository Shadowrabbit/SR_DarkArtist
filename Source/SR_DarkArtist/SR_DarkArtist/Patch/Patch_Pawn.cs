using HarmonyLib;
using Verse;
using RimWorld;
using SR.DA.Thing;
using SR.DA.Component;

class Patch_Pawn
{
    [HarmonyPatch(typeof(Pawn), "Kill")]
    class Patch1
    {
        [HarmonyPrefix]
        static bool Prefix(ref Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
        {
            bool hasBondageBed = false;//没有被束缚床束缚
            for (int i = 0; i < __instance.health.hediffSet.hediffs.Count; i++)
            {
                if (__instance.health.hediffSet.hediffs[i].def == SR.DA.Hediff.HediffDefOf.SR_BondageBed)
                {
                    hasBondageBed = true;
                    break;
                }
            }
            //如果已经被束缚
            if (hasBondageBed)
            {
                Building_BondageBed bbb = (Building_BondageBed)__instance.CurrentBed();//获取当前躺着的束缚床
                CompRemoveEffectBondageBed crebb = bbb.GetComp<CompRemoveEffectBondageBed>();
                if (crebb != null)
                {
                    crebb.DoEffect(__instance);//解除束缚
                    return false;//解除成功是会通知Pawn_HealthTracker重新检测死亡性，所以本次跳过,否则会多次kill
                }
            }
            return true;
        }
    }
}