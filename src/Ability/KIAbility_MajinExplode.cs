using SaiyanMod;
using System.Collections.Generic;
using Verse;

namespace Majin
{
    public class KIAbility_MajinExplode : KIAbility
    {
        private AbilityTier_MajinExplode Tier => (AbilityTier_MajinExplode)AbilityTier;

        public override bool CanHitTarget(LocalTargetInfo target)
        {
            return true;
        }

        public override void Cast(Verb verb, LocalTargetInfo target)
        {
            base.Cast(verb, target);

            Map map = pawn.Map;
            IntVec3 position = target.Cell;

            if (Tier.ExplosionEffect != null)
            {
                Effecter effecter = Tier.ExplosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(position, map), new TargetInfo(position, map));
                effecter.Cleanup();
            }


            IEnumerable<IntVec3> cellsToAffect = GenRadial.RadialCellsAround(position, Tier.Radius, true);

            HashSet<Thing> damagedThings = new HashSet<Thing>();

            foreach (IntVec3 cell in cellsToAffect)
            {
                if (!cell.InBounds(map)) continue;

                List<Thing> thingList = cell.GetThingList(map);
                foreach (Thing thing in thingList)
                {
                    if (damagedThings.Contains(thing)) continue;

                    if (thing is Pawn pawn)
                    {
                        // Apply damage
                        if (Tier.DamageType != null)
                        {
                            float finalDamage = Tier.Damage;
                            float distanceFromCenter = position.DistanceTo(cell);

                            finalDamage *= 1f - (distanceFromCenter / Tier.Radius);

                            DamageInfo dinfo = new DamageInfo(
                                Tier.DamageType,
                                finalDamage,
                                instigator: this.pawn,
                                hitPart: null,
                                weapon: null
                            );

                            pawn.TakeDamage(dinfo);
                        }

                    }

                    damagedThings.Add(thing);
                }
            }

            if (Tier.HediffToApply != null)
            {
                Hediff hediff = HediffMaker.MakeHediff(Tier.HediffToApply, pawn);
                pawn.health.AddHediff(hediff);
            }
        }
    }

    public class AbilityTier_MajinExplode : AbilityTierKIDef
    {
        public int Radius = 10;
        public float Damage = 100f;
        public DamageDef DamageType;
        public HediffDef HediffToApply;
        public EffecterDef ExplosionEffect;
    }
}
