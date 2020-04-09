using HarmonyLib;
using Verse;
using RimWorld;
using SR.DA.Thing;

namespace SR.DA.Patch
{
    class Patch_RestUtility
    {
        [HarmonyPatch(typeof(RestUtility), "IsValidBedFor")]
        class Patch1
        {
            /// <summary>
            /// 不会把束缚床看做能休息的床
            /// </summary>
            /// <param name="__result"></param>
            /// <param name="bedThing"></param>
            /// <param name="sleeper"></param>
            /// <param name="traveler"></param>
            /// <param name="sleeperWillBePrisoner"></param>
            /// <param name="checkSocialProperness"></param>
            /// <param name="allowMedBedEvenIfSetToNoCare"></param>
            /// <param name="ignoreOtherReservations"></param>
            /// <returns></returns>
            [HarmonyPrefix]
            static bool Prefix(ref bool __result, Verse.Thing bedThing, Pawn sleeper, Pawn traveler, bool sleeperWillBePrisoner, bool checkSocialProperness, bool allowMedBedEvenIfSetToNoCare = false, bool ignoreOtherReservations = false)
            {
                //如果目标床是束缚床则本次查找失败
                if (bedThing.def.defName.Equals("SR_BondageBed"))
                {
                    __result = false;
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

    }
}