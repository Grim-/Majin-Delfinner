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
            return base.CanHitTarget(target) && target.Cell != null && GenSight.LineOfSight(target.Cell, target.Cell, this.pawn.Map);
        }

        //public override void Start(bool consumeEnergy = true)
        //{
        //    base.Start(consumeEnergy);

        //    if (this.curTarget.Pawn == null)
        //    {
        //        Messages.Message("Cannot absorb. Target is null.", MessageTypeDefOf.RejectInput);
        //        return;
        //    }

        //    IntVec3 postion = this.curTarget.Pawn.Position;
        //    Map spawnMap = this.pawn.Map;


        //    ThingDef chosenDef = Tier.availableCandyDefs == null || Tier.availableCandyDefs.Count <= 0 ? MajinDefOf.MajinCandy : Tier.availableCandyDefs.RandomElement();


        //    Thing candy = ThingMaker.MakeThing(chosenDef);

        //    if (candy.TryGetComp<CompStoredPawn>(out CompStoredPawn compStoredPawn))
        //    {
        //        compStoredPawn.StorePawn(this.curTarget.Pawn);
        //    }

        //    GenSpawn.Spawn(candy, postion, spawnMap);
        //}
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
