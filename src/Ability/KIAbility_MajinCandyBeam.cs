using RimWorld;
using SaiyanMod;
using System.Collections.Generic;
using System.Linq;
using TaranMagicFramework;
using Verse;
using static UnityEngine.GraphicsBuffer;

namespace Majin
{
    public class KIAbility_MajinCandyBeam : KIAbility
    {
        private AbilityTier_MajinCandyBeam Tier => (AbilityTier_MajinCandyBeam)AbilityTier;

        public override bool CanHitTarget(LocalTargetInfo target)
        {
            return base.CanHitTarget(target) && target.Cell.IsValid && GenSight.LineOfSight(target.Cell, target.Cell, this.pawn.Map);
        }
    }



    public class AbilityTier_MajinCandyBeam : AbilityTier_MajinKIBeam
    {
        public List<ThingDef> availableCandyDefs;

        protected override ThingDef GetProjectile(int level)
        {
            return MajinDefOf.MajinHealBeamProjectile;
        }
    }
}
