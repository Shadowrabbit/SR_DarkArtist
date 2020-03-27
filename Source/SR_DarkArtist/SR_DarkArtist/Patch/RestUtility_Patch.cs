//using HarmonyLib;
//using Verse;
//using RimWorld;
//using SR.DA.Thing;

//class RestUtility_Patch {
//    [HarmonyPatch(typeof(RestUtility), "IsValidBedFor")]
//    class RestUtility_Patch1
//    {
//        [HarmonyPrefix]
//        static bool Prefix(ref bool __result, Thing bedThing, Pawn sleeper, Pawn traveler, bool sleeperWillBePrisoner, bool checkSocialProperness, bool allowMedBedEvenIfSetToNoCare = false, bool ignoreOtherReservations = false)
//        {
//            if (bedThing is Building_BondageBed)
//            {
//                __result = false;
//                return false;
//            }
//            else
//            {
//                return true;
//            }
//        }
//    }

//}