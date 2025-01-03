using RimWorld;
using SaiyanMod;
using TaranMagicFramework;
using Verse;

namespace Majin
{
    public class KIAbility_MajinAbsorb : KIAbility
    {
        public override bool CanHitTarget(LocalTargetInfo target)
        {
            return base.CanHitTarget(target) && target.Pawn != null && target.Pawn.RaceProps.Humanlike && target.Pawn.Downed;
        }

        public override void Cast(Verb verb, LocalTargetInfo target)
        {
            base.Cast(verb, target);

            //if (this.pawn.kindDef != MajinDefOf.SR_MajinPlayerBase)
            //{
            //    Messages.Message("Only Majin Kinds can use this ability.", MessageTypeDefOf.RejectInput);
            //    return;
            //}

            if (target.Pawn == null)
            {
                Messages.Message("Cannot absorb. Target is null.", MessageTypeDefOf.RejectInput);
                return;
            }

            if (!target.Pawn.Downed || !target.Pawn.RaceProps.Humanlike)
            {
                Messages.Message("Cannot absorb. Target must be a downed, humanlike creature.", MessageTypeDefOf.RejectInput);
                return;
            }

            if (this.pawn.TryAbsorb(target.Pawn))
            {
                //notification
            }
        }
    }
}
