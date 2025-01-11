using RimWorld;
using SaiyanMod;
using System.Collections.Generic;
using Verse;

namespace Majin
{
    public class KIAbility_MajinFission : KIAbility
    {
        private AbilityTier_MajinFission Tier => (AbilityTier_MajinFission)AbilityTier;



        public override bool CanHitTarget(LocalTargetInfo target)
        {
            return base.CanHitTarget(target) && target.Pawn == this.pawn;
        }

        public override void Start(bool consumeEnergy = true)
        {
            base.Start(consumeEnergy);
            Pawn newBuu = SaiyanMod.PawnCloneUtility.CreateCloneOf(this.pawn, new PawnCloneConfiguration()
            {
                LockApparel = false,
                ExcludedHediffs = new List<HediffDef>()
                {
                    MajinDefOf.SR_AbsorptionHediff
                }
            });


            if (this.pawn.gender == Gender.Female)
            {
                ParentRelationUtility.SetMother(newBuu, this.pawn);
            }
            else
            {
                ParentRelationUtility.SetFather(newBuu, this.pawn);
            }



            if (newBuu.TryTransferAbsorption(this.pawn, 50f))
            {

            }


            if (newBuu.TryGetKiAbilityClass(out AbilityClassKI abilityClassKI))
            {
                if (abilityClassKI.Learned(MajinDefOf.SR_Fission))
                {
                    TaranMagicFramework.Ability ability = abilityClassKI.GetLearnedAbility(MajinDefOf.SR_Fission);
                    TaranMagicFramework.AbilityTierDef tier = abilityClassKI.GetAbilityTier(MajinDefOf.SR_Fission);
                    if (ability != null && tier != null)
                    {
                        ability.SetCooldown(GenTicks.SecondsToTicks(36000));
                    }
                }
            }

            GenSpawn.Spawn(newBuu, this.pawn.Position, this.pawn.Map);  
        }
    }


    public class AbilityTier_MajinFission : AbilityTierKIDef
    {
        public override TargetingParameters TargetingParameters(Pawn pawn)
        {
            return new TargetingParameters
            {
                canTargetSelf = true,
                validator = (TargetInfo x) => x.Thing != null && x.Thing is Pawn otherpawn && otherpawn == pawn
            };
        }
    }
}
