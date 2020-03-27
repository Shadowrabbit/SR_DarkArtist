using Verse;
using HarmonyLib;
using System.Reflection;

namespace SR.DA.Patch
{
    [StaticConstructorOnStartup]
    public class PatchMain
    {
        public static Harmony instance;
        static PatchMain()
        {
            instance = new Harmony("SR.DarkArtist");
            instance.PatchAll(Assembly.GetExecutingAssembly());//对所有特性标签的方法进行patch
        }
    }
}
