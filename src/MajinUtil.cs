using LudeonTK;
using RimWorld;
using RimWorld.Planet;
using SaiyanMod;
using System.Collections.Generic;
using System.Linq;
using TaranMagicFramework;
using UnityEngine;
using UnityEngine.PlayerLoop;
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
                Hediff_MajinAbsorbption targetMajinAbsorbption = target.health.hediffSet.GetFirstHediffOfDef(MajinDefOf.SR_AbsorptionHediff) as Hediff_MajinAbsorbption;
                if (targetMajinAbsorbption != null)
                {
                    var absorbedPawns = targetMajinAbsorbption.AbsorbedPawns?.ToList() ?? new List<Pawn>();

                    foreach (var absorbedPawn in absorbedPawns)
                    {
                        majinAbsorbption.RecordAbsorbption(absorbedPawn);
                    }

                    targetMajinAbsorbption.AbsorbedPawns?.Clear();
                }

                majinAbsorbption.RecordAbsorbption(target);
                return true;
            }
            return false;
        }

        [DebugAction("Majin", "Generate Random Absorbed Pawns", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void GenerateAndAbsorbRandomPawns(Pawn pawn)
        {
            Hediff_MajinAbsorbption majinAbsorbption = pawn.health.hediffSet.GetFirstHediffOfDef(MajinDefOf.SR_AbsorptionHediff) as Hediff_MajinAbsorbption;
            if (majinAbsorbption == null)
            {
                return;
            }

            int pawnsToGenerate = 20;
            List<PawnKindDef> availablePawnKinds = DefDatabase<PawnKindDef>.AllDefs
                .Where(def => def.RaceProps.Humanlike && def != MajinDefOf.SR_MajinRace)
                .ToList();

            for (int i = 0; i < pawnsToGenerate; i++)
            {
                PawnKindDef randomPawnKind = availablePawnKinds.RandomElement();
                PawnGenerationRequest request = new PawnGenerationRequest(
                    kind: SR_DefOf.SR_HalfSaiyanColonist,
                    faction: Find.FactionManager.RandomEnemyFaction(),
                    context: PawnGenerationContext.NonPlayer,
                    forceGenerateNewPawn: true,
                    allowDead: false,
                    allowDowned: false,
                    canGeneratePawnRelations: true,
                    mustBeCapableOfViolence: true,
                    colonistRelationChanceFactor: 0f,
                    forceAddFreeWarmLayerIfNeeded: false,
                    allowGay: true,
                    allowFood: true,
                    allowAddictions: true,
                    inhabitant: false,
                    certainlyBeenInCryptosleep: false,
                    forceRedressWorldPawnIfFormerColonist: false,
                    worldPawnFactionDoesntMatter: false,
                    biocodeWeaponChance: 0f
                );

                Pawn generatedPawn = PawnGenerator.GeneratePawn(request);

                if (generatedPawn != null)
                {
                    majinAbsorbption.RecordAbsorbption(generatedPawn);

  
                    if (Prefs.DevMode)
                    {
                        Log.Message($"Generated and absorbed pawn: {generatedPawn.Name} ({generatedPawn.kindDef.defName})");
                    }
                }
            }

            Messages.Message($"{pawn.LabelShort} has absorbed {pawnsToGenerate} random beings.",
                MessageTypeDefOf.NeutralEvent);
        }

        public static Thing TurnPawnIntoCandy(this Pawn pawn)
        {
            IntVec3 spawnPosition = pawn.Position;
            Map spawnMap = pawn.Map;

            Thing candy = ThingMaker.MakeThing(MajinDefOf.MajinCandy);

            if (candy.TryGetComp<CompStoredPawn>(out CompStoredPawn compStoredPawn))
            {
                compStoredPawn.StorePawn(pawn);
            }

           return GenSpawn.Spawn(candy, spawnPosition, spawnMap);
        }


        public static void SafeMoveToWorld(this Pawn pawn)
        {
            if (Find.WorldPawns.Contains(pawn))
            {
                Log.Message($"Cant move {pawn.Label} to world, it is already there.");
                return;
            }

            if (pawn.Spawned)
            {
                pawn.DeSpawn();
            }

            Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.KeepForever);
        }
        public static bool TryTransferAbsorption(this Pawn recipient, Pawn donor, int maxTransfers = int.MaxValue)
        {
            Hediff_MajinAbsorbption recipientAbsorption = (Hediff_MajinAbsorbption)recipient.health.GetOrAddHediff(MajinDefOf.SR_AbsorptionHediff);
            if (recipientAbsorption == null)
            {
                return false;
            }

            Hediff_MajinAbsorbption donorAbsorption = donor.health.hediffSet.GetFirstHediffOfDef(MajinDefOf.SR_AbsorptionHediff) as Hediff_MajinAbsorbption;
            if (donorAbsorption == null || donorAbsorption.AbsorbedPawns == null || !donorAbsorption.AbsorbedPawns.Any())
            {
                return false;
            }

            var pawnsToTransfer = donorAbsorption.AbsorbedPawns.Take(maxTransfers).ToList();

            foreach (var pawn in pawnsToTransfer)
            {
                donorAbsorption.ReleaseAbsorbedPawn(pawn);
                recipientAbsorption.RecordAbsorbption(pawn);
            }

            return pawnsToTransfer.Any();
        }

        public static bool TryTransferAbsorption(this Pawn recipient, Pawn donor, float transferPercent = 100f)
        {

            Hediff_MajinAbsorbption recipientAbsorption = (Hediff_MajinAbsorbption)recipient.health.GetOrAddHediff(MajinDefOf.SR_AbsorptionHediff);
            if (recipientAbsorption == null)
            {
                return false;
            }


            Hediff_MajinAbsorbption donorAbsorption = donor.health.hediffSet.GetFirstHediffOfDef(MajinDefOf.SR_AbsorptionHediff) as Hediff_MajinAbsorbption;
            if (donorAbsorption == null || donorAbsorption.AbsorbedPawns == null || !donorAbsorption.AbsorbedPawns.Any())
            {
                return false;
            }


            int numToTransfer = Mathf.RoundToInt((transferPercent / 100f) * donorAbsorption.AbsorbedPawns.Count());

            var pawnsToTransfer = donorAbsorption.AbsorbedPawns.Take(numToTransfer).ToList();

            foreach (var pawn in pawnsToTransfer)
            {
                donorAbsorption.ReleaseAbsorbedPawn(pawn);
                recipientAbsorption.RecordAbsorbption(pawn);
            }

            return pawnsToTransfer.Any();
        }

        public static bool IsMajin(this Pawn pawn)
        {
            if (pawn.kindDef == MajinDefOf.SR_MajinRace)
            {
                return true;
            }

            return false;
        }

        public static void SafeReturnToMap(this Pawn pawn, Map map, IntVec3 postiion)
        {
            if (map == null)
            {
                Log.Error($"Cant Return {pawn.Label} to a null map.");
                return;
            }

            if (!Find.WorldPawns.Contains(pawn))
            {
                Log.Message($"{pawn.Label} is not stored in WorldPawns, attempting to spawn anyway.");
            }

            IntVec3 spawnPos = CellFinder.StandableCellNear(postiion, map, 3f);
            GenSpawn.Spawn(pawn, spawnPos, map);        
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
