using HarmonyLib;
using Verse;

namespace Majin
{
    [StaticConstructorOnStartup]
    public static class MajinPatchClass
    {
        static MajinPatchClass ()
        {
            var harmony = new Harmony("com.majin.majinpatches");
            harmony.PatchAll();
        }


        [HarmonyPatch(typeof(BodyPartDef), "GetMaxHealth")]
        public class GetMaxHealth_Patch
        {
            [HarmonyPriority(Priority.Last)]
            private static void Postfix(BodyPartDef __instance, Pawn pawn, ref float __result)
            {
                if (pawn.IsMajin())
                {
                    __result *= 2.2f;
                }
            }
        }
    }
}
