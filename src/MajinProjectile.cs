using RimWorld;
using SaiyanMod;
using Verse;

namespace Majin
{
    public class MajinCandyBeam_Projectile : KIBeam_Projectile
    {

        public override int BaseDamage => 5;
        public override int DamagePerLevel => 1;
        public override int explosionRadius => 1;

        public override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            if (hitThing is Pawn hitPawn && hitPawn.Faction.HostileTo(Faction.OfPlayer) && !hitPawn.Destroyed && hitPawn.Downed && hitPawn.RaceProps.Humanlike)
            {
                hitPawn.TurnPawnIntoCandy();
                return;
            }
            else
            {
                base.Impact(hitThing, blockedByShield);
            }
        }


        protected override void DealDamageToThing(Thing thing)
        {
            if (thing is Pawn hitPawn && !hitPawn.Destroyed && hitPawn.Downed && hitPawn.RaceProps.Humanlike)
            {
                if (hitPawn.Faction.HostileTo(this.launcher.Faction))
                {
                    hitPawn.TurnPawnIntoCandy();
                    return;
                }
            }
            else
            {
                base.DealDamageToThing(thing);
            }
        }
    }


}
