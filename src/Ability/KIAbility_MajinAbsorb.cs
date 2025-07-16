using RimWorld;
using SaiyanMod;
using TaranMagicFramework;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace Majin
{
    public class KIAbility_MajinAbsorb : KIAbility
    {
        private AbilityTier_MajinAbsorb Tier => (AbilityTier_MajinAbsorb)AbilityTier;

        public override bool CanHitTarget(LocalTargetInfo target)
        {
            return base.CanHitTarget(target) && target.Pawn != null && target.Pawn.RaceProps.Humanlike && target.Pawn.Downed;
        }

        public override void Start(bool consumeEnergy = true)
        {
            base.Start(consumeEnergy);

            Pawn targetPawn = this.curTarget.Pawn;

            if (targetPawn == null)
            {
                Messages.Message("Cannot absorb. Target is null.", MessageTypeDefOf.RejectInput);
                return;
            }

            if (!targetPawn.Downed || !targetPawn.RaceProps.Humanlike)
            {
                Messages.Message("Cannot absorb. Target must be a downed, humanlike creature.", MessageTypeDefOf.RejectInput);
                return;
            }

            var frameworkAbilities = new List<TaranMagicFramework.AbilityDef>();
            if (targetPawn.TryGetKiAbilityClass(out AbilityClassKI targetKiClass) && this.pawn.TryGetKiAbilityClass(out AbilityClassKI userKiClass))
            {
                frameworkAbilities.AddRange(targetKiClass.LearnedAbilities.Select(a => a.def).Where(d => !userKiClass.Learned(d)));
            }

            var rimworldAbilities = targetPawn.abilities?.abilities
                .Select(a => a.def)
                .Where(def => this.pawn.abilities.abilities.All(a => a.def != def))
                .ToList() ?? new List<RimWorld.AbilityDef>();

            void AbsorbAndLearn(Def selectedAbilityDef)
            {
                if (this.pawn.TryAbsorb(targetPawn))
                {
                    string learnedAbilityLabel = "";
                    bool learnedSomething = false;

                    if (selectedAbilityDef is TaranMagicFramework.AbilityDef frameworkDef)
                    {
                        if (this.pawn.TryGetKiAbilityClass(out AbilityClassKI kiClass) && !kiClass.Learned(frameworkDef))
                        {
                            kiClass.LearnAbility(frameworkDef, false, 0);
                            learnedAbilityLabel = frameworkDef.LabelCap;
                            learnedSomething = true;
                        }
                    }
                    else if (selectedAbilityDef is RimWorld.AbilityDef rimworldDef)
                    {
                        if (this.pawn.abilities.abilities.All(a => a.def != rimworldDef))
                        {
                            this.pawn.abilities.GainAbility(rimworldDef);
                            learnedAbilityLabel = rimworldDef.LabelCap;
                            learnedSomething = true;
                        }
                    }

                    if (learnedSomething)
                    {
                        Messages.Message($"{this.pawn.LabelShortCap} absorbed {targetPawn.LabelShort} and learned '{learnedAbilityLabel}'!", MessageTypeDefOf.PositiveEvent);
                    }
                    else
                    {
                        Messages.Message($"{this.pawn.LabelShortCap} absorbed {targetPawn.LabelShort}.", MessageTypeDefOf.PositiveEvent);
                    }
                }
            }

            Find.WindowStack.Add(new Window_AbilitySelection(
                targetPawn,
                frameworkAbilities,
                rimworldAbilities,
                onFrameworkAbilitySelected: (selectedAbility) => AbsorbAndLearn(selectedAbility),
                onRimWorldAbilitySelected: (selectedAbility) => AbsorbAndLearn(selectedAbility),
                onCanceled: () => Messages.Message("Absorption canceled.", MessageTypeDefOf.NeutralEvent)
            ));
        }
    }

    public class AbilityTier_MajinAbsorb : AbilityTierKIDef
    {
        public override TargetingParameters TargetingParameters(Pawn pawn)
        {
            return new TargetingParameters
            {
                canTargetPawns = true,
                validator = (TargetInfo x) => IsSuitableForAttacking(pawn, x, this)
            };
        }

        public static bool IsSuitableForAttacking(Pawn caster, TargetInfo x, AbilityTierDef abilityTier)
        {
            return caster != x.Thing && x.Thing is Pawn victim && !victim.Dead
                && victim.Position.DistanceTo(caster.Position) <= abilityTier.effectRadius
                && GenSight.LineOfSight(caster.Position, x.Cell, caster.Map);
        }
    }
}