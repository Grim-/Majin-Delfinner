using RimWorld;
using SaiyanMod;
using System.Collections.Generic;
using TaranMagicFramework;
using Verse;
using static UnityEngine.GraphicsBuffer;

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

            if (this.curTarget.Pawn == null)
            {
                Messages.Message("Cannot absorb. Target is null.", MessageTypeDefOf.RejectInput);
                return;
            }

            if (!this.curTarget.Pawn.Downed || !this.curTarget.Pawn.RaceProps.Humanlike)
            {
                Messages.Message("Cannot absorb. Target must be a downed, humanlike creature.", MessageTypeDefOf.RejectInput);
                return;
            }

            if (this.pawn.TryAbsorb(this.curTarget.Pawn))
            {
                //notification
                Messages.Message($"Asorbed {this.curTarget.Pawn.Label}", MessageTypeDefOf.PositiveEvent);
            }
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

    public class KIAbility_MajinCandyBeam : KIAbility
    {
        private AbilityTier_MajinCandyBeam Tier => (AbilityTier_MajinCandyBeam)AbilityTier;


        public override bool CanHitTarget(LocalTargetInfo target)
        {
            return base.CanHitTarget(target) && target.Pawn != null && target.Pawn.RaceProps.Humanlike && target.Pawn.Downed;
        }

        public override void Start(bool consumeEnergy = true)
        {
            base.Start(consumeEnergy);

            if (this.curTarget.Pawn == null)
            {
                Messages.Message("Cannot absorb. Target is null.", MessageTypeDefOf.RejectInput);
                return;
            }

            IntVec3 postion = this.curTarget.Pawn.Position;
            Map spawnMap = this.pawn.Map;


            ThingDef chosenDef = Tier.availableCandyDefs == null || Tier.availableCandyDefs.Count <= 0 ? MajinDefOf.MajinCandy : Tier.availableCandyDefs.RandomElement();


            Thing candy = ThingMaker.MakeThing(chosenDef);

            if (candy.TryGetComp<CompStoredPawn>(out CompStoredPawn compStoredPawn))
            {
                compStoredPawn.StorePawn(this.curTarget.Pawn);
            }

            GenSpawn.Spawn(candy, postion, spawnMap);
        }
    }

    public class AbilityTier_MajinCandyBeam : AbilityTierKIDef
    {
        public List<ThingDef> availableCandyDefs;

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
