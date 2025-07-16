using RimWorld.Planet;
using SaiyanMod;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using TaranMagicFramework;
using Verse;
using static UnityEngine.GraphicsBuffer;

namespace Majin
{
    public class Hediff_MajinAbsorbption : HediffWithComps
    {
        protected Gene_Majin MajinGene => this.pawn.genes.GetFirstGeneOfType<Gene_Majin>();
        public override string Description
        {
            get
            {
                if (this.MajinGene == null)
                {
                    return base.Description;
                }

                StringBuilder description = new StringBuilder(base.Description);
                description.AppendLine("\n\nAbsorbed beings:");

                if (this.MajinGene.AbsorbedPawns == null || this.MajinGene.AbsorbedPawns.Count == 0)
                {
                    description.AppendLine("None... yet.");
                    return description.ToString();
                }

                if (this.MajinGene.AbsorbedPawns.Count <= 20)
                {
                    description.AppendLine(string.Join("\n", this.MajinGene.AbsorbedPawns));
                }
                else
                {
                    description.AppendLine(string.Join("\n", this.MajinGene.AbsorbedPawns.Take(20)));
                    description.AppendLine("...and countless others lost to the void.");
                }

                return description.ToString();
            }
        }

        public override void PostTick()
        {
            base.PostTick();
        }

    }


    //public class HediffDef_MajinAbsorbption : HediffDef
    //{
    //    public List<SeverityUpgrade> upgrades;
    //}


    //public class SeverityUpgrade
    //{
    //    public float MinSeverity = 1;
    //    public TaranMagicFramework.AbilityDef Ability;
    //    public bool CanLoseAbility = true;
    //}

    //public class Hediff_MajinAbsorbption : HediffWithComps, IThingHolder
    //{
    //    public List<Pawn> AbsorbedPawns => innerContainer != null ?  innerContainer.InnerListForReading : new List<Pawn>();

    //    private List<TaranMagicFramework.AbilityDef> _activeAbilities = new List<AbilityDef>();

    //    protected ThingOwner<Pawn> innerContainer;

    //    public Hediff_MajinAbsorbption()
    //    {
    //        innerContainer = new ThingOwner<Pawn>(this, false, LookMode.Deep);
    //    }

    //    public override void Notify_Spawned()
    //    {
    //        base.Notify_Spawned();

    //        if (innerContainer == null)
    //        {
    //            innerContainer = new ThingOwner<Pawn>(this, false, LookMode.Deep);
    //        }
    //    }

    //    public override string Description
    //    {
    //        get
    //        {
    //            StringBuilder description = new StringBuilder(base.Description);
    //            description.AppendLine("\n\nAbsorbed beings:");

    //            if (AbsorbedPawns == null || AbsorbedPawns.Count == 0)
    //            {
    //                description.AppendLine("None... yet.");
    //                return description.ToString();
    //            }

    //            if (AbsorbedPawns.Count <= 20)
    //            {
    //                description.AppendLine(string.Join("\n", AbsorbedPawns));
    //            }
    //            else
    //            {
    //                description.AppendLine(string.Join("\n", AbsorbedPawns.Take(20)));
    //                description.AppendLine("...and countless others lost to the void.");
    //            }

    //            return description.ToString();
    //        }
    //    }

    //    public IThingHolder ParentHolder => this.pawn;

    //    public void IncreaseSeverity(float Amount)
    //    {
    //        this.Severity += Amount;


    //        OnSeverityChange();
    //    }

    //    public void RecordAbsorbption(Pawn PawnToAbsorb)
    //    {
    //        Map lastMap = PawnToAbsorb.Map;
    //        IntVec3 lastPosition = PawnToAbsorb.Position;

    //        if (PawnToAbsorb.Spawned)
    //        {
    //            PawnToAbsorb.DeSpawn();
    //        }

    //        if (innerContainer.TryAdd(PawnToAbsorb, true))
    //        {
    //            //stored pawn
    //            IncreaseSeverity(GetSeverity(PawnToAbsorb));
    //            Log.Message("stored pawn");
    //        }
    //        else
    //        {
    //            GenSpawn.Spawn(PawnToAbsorb, lastPosition, lastMap);
    //            Log.Message("Failed to store pawn, respawning");
    //        }


    //    }

    //    public void ReleaseAllPawns()
    //    {
    //        foreach (var item in AbsorbedPawns.ToList())
    //        {
    //            ReleaseAbsorbedPawn(item);
    //        }
    //    }

    //    public void ReleaseAbsorbedPawn(Pawn Pawn, bool returnToMap = true)
    //    {
    //        if (innerContainer.Contains(Pawn))
    //        {
    //            if (innerContainer.TryDrop(Pawn, ThingPlaceMode.Near, out Pawn droppedPawn))
    //            {
    //                ReduceSeverity(GetSeverity(droppedPawn));
    //            }
    //        }
    //    }

    //    protected void OnSeverityChange()
    //    {
    //        float current = this.Severity;
    //        var upgradesDef = def as HediffDef_MajinAbsorbption;

    //        if (upgradesDef?.upgrades == null || upgradesDef.upgrades.Count == 0)
    //            return;


    //        if (this.pawn.TryGetKiAbilityClass(out AbilityClassKI kiClass))
    //        {

    //            foreach (var upgrade in upgradesDef.upgrades)
    //            {
    //                bool hasAbility = _activeAbilities.Contains(upgrade.Ability);
    //                bool shouldHaveAbility = current >= upgrade.MinSeverity;

    //                if (shouldHaveAbility && !hasAbility)
    //                {
    //                    kiClass.LearnAbility(upgrade.Ability, false, 0);
    //                    _activeAbilities.Add(upgrade.Ability);
    //                }
    //                else if (!shouldHaveAbility && hasAbility && upgrade.CanLoseAbility)
    //                {
    //                    kiClass.RemoveAbility(kiClass.GetLearnedAbility(upgrade.Ability));
    //                    _activeAbilities.Remove(upgrade.Ability);
    //                }
    //            }

    //        }
    //        else return;
    //    }


    //    private float GetSeverity(Pawn PawnToAbsorb)
    //    {
    //        float severity = 1;
    //        if (PawnToAbsorb.TryGetKiAbilityClass(out AbilityClassKI abilityClassKI))
    //        {
    //            severity += 1;
    //        }

    //        return severity;
    //    }

    //    public override void Notify_Downed()
    //    {
    //        base.Notify_Downed();
    //        ReleaseAllPawns();
    //    }


    //    public void ReduceSeverity(float Amount)
    //    {
    //        this.Severity -= Amount;

    //        OnSeverityChange();
    //    }


    //    public override void ExposeData()
    //    {
    //        base.ExposeData();


    //        Scribe_Deep.Look(ref innerContainer, "pawnContainer");
    //        Scribe_Collections.Look(ref _activeAbilities, "activeAbilities", LookMode.Def);

    //        if (Scribe.mode == LoadSaveMode.PostLoadInit)
    //        {
    //            if (_activeAbilities == null)
    //            {
    //                _activeAbilities = new List<AbilityDef>();
    //            }
    //        }

    //    }

    //    public void GetChildHolders(List<IThingHolder> outChildren)
    //    {
    //        ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
    //    }

    //    public ThingOwner GetDirectlyHeldThings()
    //    {
    //        return innerContainer;
    //    }
    //}
}
