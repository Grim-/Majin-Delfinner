using RimWorld;
using SaiyanMod;
using System;
using System.Collections.Generic;
using TaranMagicFramework;
using UnityEngine;
using Verse;
using Ability = TaranMagicFramework.Ability;
using HotSwappableAttribute = SaiyanMod.HotSwappableAttribute;

namespace Majin
{
    [HotSwappable]
    public class AbilityTier_MajinKIBeam : AbilityTierKIDef
    {
        public List<MajinLevelProjectileData> ProjectileData;


        public override TargetingParameters TargetingParameters(Pawn pawn)
        {
            return new TargetingParameters
            {
                canTargetPawns = true,
                validator = (TargetInfo x) => pawn != x.Thing && (x.Thing is Pawn || x.Thing is Building)
                && x.Cell.DistanceTo(pawn.Position) <= effectRadius && GenSight.LineOfSight(pawn.Position, x.Cell, pawn.Map)
            };
        }

        public override void Start(Ability ability)
        {
            base.Start(ability);
            ability.SetCooldown(ability.AbilityTier.cooldownTicks);
            ability.pawn.rotationTracker.FaceTarget(ability.curTarget.Thing);
            ability.pawn.stances.SetStance(new Stance_Wait(80, ability.curTarget.Thing, null, null));
            MakeKIBeamOverlay(ability, ability.pawn, ability.curTarget.Thing);
        }

        public virtual void MakeKIBeamOverlay(Ability ability, Pawn caster, Thing target)
        {
            var obj = ThingMaker.MakeThing(SR_DefOf.SR_LegendaryKIBeamChargingOverlay) as KIBeam_Overlay;
            obj.sourceAbility = ability;
            obj.Scale = 2f;
            obj.pawn = caster;
            obj.angle = (caster.DrawPos - target.DrawPos).AngleFlat() + 45;
            obj.projectileToFire = GetProjectile(ability.level);

            obj.target = target;
            var offset = Vector3.left * 0.5f;
            obj.exactPosition = caster.DrawPos;
            obj.Attach(caster, offset);
            GenSpawn.Spawn(obj, obj.exactPosition.ToIntVec3(), caster.Map);
        }


        protected virtual ThingDef GetProjectile(int level)
        {
            if (ProjectileData == null || ProjectileData.Count <= 0 || level > ProjectileData.Count)
            {
                return MajinDefOf.MajinBeamProjectile;
            }

            return ProjectileData[level].ProjectileDef != null ? ProjectileData[level].ProjectileDef : MajinDefOf.MajinBeamProjectile;
        }


    }

    [Serializable]
    public class MajinLevelProjectileData
    {
        public int Level;
        public ThingDef ProjectileDef;
    }
}
