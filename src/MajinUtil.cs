using RimWorld.Planet;
using SaiyanMod;
using TaranMagicFramework;
using Verse;

namespace Majin
{
    public static class MajinUtil
    {
        public static bool TryAbsorb(this Pawn pawn, Pawn target)
        {
            Hediff_MajinAbsorbption majinAbsorbption = (Hediff_MajinAbsorbption)pawn.health.GetOrAddHediff(MajinDefOf.SR_AbsorptionHediff);

            if (majinAbsorbption != null)
            {
                majinAbsorbption.RecordAbsorbption(target);
                return true;
            }

            return false;
        }


        public static void SafeMoveToWorld(this Pawn pawn)
        {
            if (pawn.Map != null && !Find.WorldPawns.Contains(pawn))
            {
                if (pawn.Spawned)
                {
                    pawn.DeSpawn();
                }

                Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.KeepForever);
            }
        }

        public static void SafeReturnToMap(this Pawn pawn, Map map, IntVec3 postiion)
        {
            if (Find.WorldPawns.Contains(pawn) && map != null && pawn.Map == null)
            {
                GenSpawn.Spawn(pawn, postiion, map);
            }
        }


        public static bool TryGetKiAbilityClass(this Pawn Pawn, out AbilityClassKI abilityClass)
        {
            abilityClass = null;

            var compAbility = Pawn.GetComp<CompAbilities>();

            if (compAbility == null) return false;

            if (compAbility.TryGetKIAbilityClass(out AbilityClassKI kiClass))
            {
                abilityClass = kiClass;
                return true;
            }

            return false;
        }
    }
}
