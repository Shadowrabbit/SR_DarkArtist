using HarmonyLib;
using RimWorld;
using Verse;
using SR.DA.Thing;
namespace SR.DA.Patch
{
    class Patch_Room
    {
        /// <summary>
        /// 设置床可以给犯人使用时，会强制让房间内的殖民者下床，让这个设定在束缚床上不生效
        /// </summary>
        [HarmonyPatch(typeof(Room), "Notify_RoomShapeOrContainedBedsChanged")]
        class Patch1
        {
            [HarmonyPostfix]
            static void Postfix(ref Room __instance)
            {
                foreach (var bed in __instance.ContainedBeds)
                {
                    if (bed is Building_BondageBed)
                    {
                        bed.ForPrisoners = true;
                    }
                    if (bed is Building_AdvancedBondageBed)
                    {
                        bed.ForPrisoners=false;
                    }
                }
            }
        }
    }
}