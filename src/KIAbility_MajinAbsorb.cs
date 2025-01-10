using RimWorld;
using SaiyanMod;
using TaranMagicFramework;
using Verse;
using static UnityEngine.GraphicsBuffer;

namespace Majin
{
    public class KIAbility_MajinAbsorb : KIAbility
    {
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
}
