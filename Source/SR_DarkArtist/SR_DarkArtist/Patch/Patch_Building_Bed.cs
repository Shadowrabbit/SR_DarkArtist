using HarmonyLib;
using Verse;
using RimWorld;
using SR.DA.Thing;

namespace SR.DA.Patch
{
    class Patch_Building_Bed
    {
        [HarmonyPatch(typeof(Building_Bed), "DeSpawn")]
        class Patch1
        {
            [HarmonyPrefix]
            static bool Prefix(ref Building_Bed __instance, DestroyMode mode = DestroyMode.Vanish)
            {
                if (__instance is Building_BondageBed)
                {
                    var type = __instance.GetType();
                    var func = type.GetMethod("RemoveAllOwners",System.Reflection.BindingFlags.NonPublic) ?? throw new System.Exception("Reflection fail on method RemoveAllOwners");
                    func?.Invoke(__instance,null);//反射调用私有函数
                    __instance.ForPrisoners = false;
                    __instance.Medical = false;
                    //__instance.alreadySetDefaultMed = false;
                    var pro = type.GetProperty("alreadySetDefaultMed", System.Reflection.BindingFlags.NonPublic) ?? throw new System.Exception("Reflection fail on property alreadySetDefaultMed");
                    pro.SetValue(__instance,false);
                    __instance.DeSpawn(mode);
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
