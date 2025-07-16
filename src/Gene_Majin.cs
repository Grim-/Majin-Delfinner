using SaiyanMod;
using System.Collections.Generic;
using System.Linq;
using TaranMagicFramework;
using UnityEngine;
using Verse;

namespace Majin
{
    public class GeneDef_Majin : GeneDef
    {
        public List<AbilityUpgrade> abilityUpgrades;
        public int maxAbsorbed = 10;
    }

    public class AbilityUpgrade
    {
        public int requiredCount = 1;
        public TaranMagicFramework.AbilityDef ability;
        public bool canLoseAbility = true;
    }
    public class Gene_Majin : Gene, IThingHolder
    {
        public List<Pawn> AbsorbedPawns => innerContainer?.InnerListForReading ?? new List<Pawn>();

        private List<TaranMagicFramework.AbilityDef> _activeAbilities = new List<AbilityDef>();
        protected ThingOwner<Pawn> innerContainer;
        private bool wasDowned = false;

        private GeneDef_Majin Def => def as GeneDef_Majin;

        public Gene_Majin()
        {
            innerContainer = new ThingOwner<Pawn>(this, false, LookMode.Deep);
        }

        public override void PostAdd()
        {
            base.PostAdd();
            this.pawn.story.skinColorOverride = new Color(1f, 1f, 1f, 1f);

            if (innerContainer == null)
            {
                innerContainer = new ThingOwner<Pawn>(this, false, LookMode.Deep);
            }
        }

        public IThingHolder ParentHolder => this.pawn;

        public void RecordAbsorbption(Pawn pawnToAbsorb)
        {
            if (AbsorbedPawns.Count >= Def.maxAbsorbed)
            {
                Log.Message($"{pawn.LabelShort}'s absorption capacity is full.");
                return;
            }

            Map lastMap = pawnToAbsorb.Map;
            IntVec3 lastPosition = pawnToAbsorb.Position;

            if (pawnToAbsorb.Spawned)
            {
                pawnToAbsorb.DeSpawn();
            }

            if (innerContainer.TryAdd(pawnToAbsorb))
            {
                UpdateHediffSeverity();
                UpdateAbilitiesOnCountChange();
            }
            else
            {
                GenSpawn.Spawn(pawnToAbsorb, lastPosition, lastMap);
                Log.Warning($"Failed to absorb {pawnToAbsorb.LabelShort}, respawning.");
            }
        }

        public void ReleaseAllPawns()
        {
            foreach (var item in AbsorbedPawns.ToList())
            {
                ReleaseAbsorbedPawn(item);
            }
        }

        public void ReleaseAbsorbedPawn(Pawn pawnToRelease)
        {
            if (innerContainer.Contains(pawnToRelease))
            {
                innerContainer.TryDrop(pawnToRelease, this.pawn.Position, this.pawn.Map, ThingPlaceMode.Near, out Pawn _);

                UpdateHediffSeverity();
                UpdateAbilitiesOnCountChange();
            }
        }

        private void UpdateHediffSeverity()
        {
            HediffWithComps hediff = (HediffWithComps)this.pawn.health.GetOrAddHediff(MajinDefOf.SR_AbsorptionHediff);

            if (hediff != null)
            {
                if (innerContainer.InnerListForReading.Count == 0 && this.pawn.health.hediffSet.HasHediff(MajinDefOf.SR_AbsorptionHediff))
                {
                    this.pawn.health.RemoveHediff(hediff);
                }
                else
                {
                    hediff.Severity = Mathf.Clamp01((float)innerContainer.InnerListForReading.Count / (float)Def.maxAbsorbed);
                }                
            }

        }

        private void UpdateAbilitiesOnCountChange()
        {
            if (Def?.abilityUpgrades == null || !pawn.TryGetKiAbilityClass(out AbilityClassKI kiClass))
            {
                return;
            }

            int currentCount = AbsorbedPawns.Count;

            foreach (var upgrade in Def.abilityUpgrades)
            {
                bool hasAbility = _activeAbilities.Contains(upgrade.ability);
                bool shouldHaveAbility = currentCount >= upgrade.requiredCount;

                if (shouldHaveAbility && !hasAbility)
                {
                    kiClass.LearnAbility(upgrade.ability, false, 0);
                    _activeAbilities.Add(upgrade.ability);
                }
                else if (!shouldHaveAbility && hasAbility && upgrade.canLoseAbility)
                {
                    kiClass.RemoveAbility(kiClass.GetLearnedAbility(upgrade.ability));
                    _activeAbilities.Remove(upgrade.ability);
                }
            }
        }

        public override void Tick()
        {
            base.Tick();
            if (pawn.Downed && !wasDowned)
            {
                ReleaseAllPawns();
                wasDowned = true;
            }
            else if (!pawn.Downed && wasDowned)
            {
                wasDowned = false;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref innerContainer, "pawnContainer");
            Scribe_Collections.Look(ref _activeAbilities, "activeAbilities", LookMode.Def);
            Scribe_Values.Look(ref wasDowned, "wasDowned", false);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (_activeAbilities == null)
                {

                    _activeAbilities = new List<AbilityDef>();
                }
            }
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return innerContainer;
        }
    }
}
