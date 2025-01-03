using RimWorld;
using SaiyanMod;
using Verse;

namespace Majin
{
    public class MajinProjectile : KIBeam_Projectile
    {
        public override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing, blockedByShield);

            if (hitThing is Pawn hitPawn)
            {
                if (hitPawn.Faction != Faction.OfPlayer)
                {
                    IntVec3 spawnPosition = hitPawn.Position;
                    Map spawnMap = hitPawn.Map;

                    Thing candy = ThingMaker.MakeThing(MajinDefOf.MajinCandy);

                    if (candy.TryGetComp<CompStoredPawn>(out CompStoredPawn compStoredPawn))
                    {
                        compStoredPawn.StorePawn(hitPawn);
                    }

                    GenSpawn.Spawn(candy, spawnPosition, spawnMap);
                }
            }
        }
    }


}
