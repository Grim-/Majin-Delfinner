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
    public class HediffDef_MajinAbsorbption : HediffDef
    {
        public List<SeverityUpgrade> upgrades;
    }


    public class SeverityUpgrade
    {
        public float MinSeverity = 1;
        public TaranMagicFramework.AbilityDef Ability;
        public bool CanLoseAbility = true;
    }

    public class Hediff_MajinAbsorbption : HediffWithComps
    {
        private List<Pawn> _AbsorbedPawns = new List<Pawn>();
        public List<Pawn> AbsorbedPawns => _AbsorbedPawns.ToList();

        private List<TaranMagicFramework.AbilityDef> _activeAbilities = new List<AbilityDef>();

        public override string Description
        {
            get
            {
                StringBuilder description = new StringBuilder(base.Description);
                description.AppendLine("\n\nAbsorbed beings:");

                if (_AbsorbedPawns == null || _AbsorbedPawns.Count == 0)
                {
                    description.AppendLine("None... yet.");
                    return description.ToString();
                }

                if (_AbsorbedPawns.Count <= 20)
                {
                    description.AppendLine(string.Join("\n", _AbsorbedPawns));
                }
                else
                {
                    description.AppendLine(string.Join("\n", _AbsorbedPawns.Take(20)));
                    description.AppendLine("...and countless others lost to the void.");
                }

                return description.ToString();
            }
        }

        public void IncreaseSeverity(float Amount)
        {
            this.Severity += Amount;


            OnSeverityChange();
        }

        public void RecordAbsorbption(Pawn PawnToAbsorb)
        {
            _AbsorbedPawns.Add(PawnToAbsorb);     

            IncreaseSeverity(GetSeverity(PawnToAbsorb));

            PawnToAbsorb.SafeMoveToWorld();
        }


        protected void OnSeverityChange()
        {
            float current = this.Severity;
            var upgradesDef = def as HediffDef_MajinAbsorbption;

            if (upgradesDef?.upgrades == null || upgradesDef.upgrades.Count == 0)
                return;


            if (this.pawn.TryGetKiAbilityClass(out AbilityClassKI kiClass))
            {

                foreach (var upgrade in upgradesDef.upgrades)
                {
                    bool hasAbility = _activeAbilities.Contains(upgrade.Ability);
                    bool shouldHaveAbility = current >= upgrade.MinSeverity;

                    if (shouldHaveAbility && !hasAbility)
                    {
                        kiClass.LearnAbility(upgrade.Ability, false, 0);
                        _activeAbilities.Add(upgrade.Ability);
                    }
                    else if (!shouldHaveAbility && hasAbility && upgrade.CanLoseAbility)
                    {
                        kiClass.RemoveAbility(kiClass.GetLearnedAbility(upgrade.Ability));
                        _activeAbilities.Remove(upgrade.Ability);
                    }
                }

            }
            else return;
        }


        private float GetSeverity(Pawn PawnToAbsorb)
        {
            float severity = 1;
            if (PawnToAbsorb.TryGetKiAbilityClass(out AbilityClassKI abilityClassKI))
            {
                severity += 1;
            }

            return severity;
        }

        public override void Notify_Downed()
        {
            base.Notify_Downed();
            ReleaseAllPawns();
        }

        public void ReleaseAllPawns()
        {
            foreach (var item in _AbsorbedPawns.ToList())
            {
                ReleaseAbsorbedPawn(item);
            }
        }

        public void ReleaseAbsorbedPawn(Pawn Pawn, bool returnToMap = true)
        {
            if (_AbsorbedPawns.Contains(Pawn))
            {
                ReduceSeverity(GetSeverity(Pawn));

               if(returnToMap) 
                    Pawn.SafeReturnToMap(this.pawn.Map, this.pawn.Position);

                _AbsorbedPawns.Remove(Pawn);
            }
        }

        public void ReduceSeverity(float Amount)
        {
            this.Severity -= Amount;

            OnSeverityChange();
        }


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref _AbsorbedPawns, "absorbedPawns");
            Scribe_Collections.Look(ref _activeAbilities, "activeAbilities", LookMode.Def);
        }

    }
}
