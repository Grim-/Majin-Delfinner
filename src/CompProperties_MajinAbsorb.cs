using RimWorld;
using SaiyanMod;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaranMagicFramework;
using Verse;

namespace Majin
{

    public class CompProperties_MajinAbsorb : CompProperties_AbilityEffect
    {
        public CompProperties_MajinAbsorb()
        {
            compClass = typeof(CompAbilityEffect_MajinAbsorb);
        }
    }

    public class CompAbilityEffect_MajinAbsorb : CompAbilityEffect
    {

        public new CompProperties_MajinAbsorb Props => (CompProperties_MajinAbsorb)props;
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            if (parent.pawn.kindDef != MajinDefOf.SR_MajinPlayerBase)
            {
                Messages.Message("Only Majin Kinds can use this ability.", MessageTypeDefOf.RejectInput);
                return;
            }

            if (target.Pawn == null )
            {
                Messages.Message("Cannot absorb. Target is null.", MessageTypeDefOf.RejectInput);
                return;
            }

            if (!target.Pawn.Downed || !target.Pawn.RaceProps.Humanlike)
            {
                Messages.Message("Cannot absorb. Target must be a downed, humanlike creature.", MessageTypeDefOf.RejectInput);
                return;
            }


            float SeverityIncrease = 1;


            if (TryGetKiAbilityClass(target.Pawn, out AbilityClassKI abilityClassKI))
            {
                SeverityIncrease += 1;
            }


            Hediff_MajinAbsorbption majinAbsorbption = (Hediff_MajinAbsorbption)parent.pawn.health.GetOrAddHediff(MajinDefOf.SR_AbsorptionHediff);

            if (majinAbsorbption != null)
            {
                majinAbsorbption.IncreaseSeverity(SeverityIncrease);
                majinAbsorbption.RecordAbsorbption(target.Pawn.LabelShort);
            }
        }

        public bool TryGetKiAbilityClass(Pawn Pawn, out AbilityClassKI abilityClass)
        {
            abilityClass = null;

            var compAbility = Pawn.GetComp<CompAbilities>();

            if (compAbility == null) return false;

            if (compAbility.TryGetKIAbilityClass(out AbilityClassKI kiClass))
            {
                abilityClass = kiClass;
                return true;
            }

            return false;
        }
    }

    public class MajinAbsorb : KIAbility
    {
        public override bool CanHitTarget(LocalTargetInfo target)
        {
            return base.CanHitTarget(target) && target.Pawn != null && target.Pawn.RaceProps.Humanlike && target.Pawn.Downed;
        }

        public override void Cast(Verb verb, LocalTargetInfo target)
        {
            base.Cast(verb, target);

            if (this.pawn.kindDef != MajinDefOf.SR_MajinPlayerBase)
            {
                Messages.Message("Only Majin Kinds can use this ability.", MessageTypeDefOf.RejectInput);
                return;
            }

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

            float SeverityIncrease = 1;


            if (TryGetKiAbilityClass(target.Pawn, out AbilityClassKI abilityClassKI))
            {
                SeverityIncrease += 1;
            }


            Hediff_MajinAbsorbption majinAbsorbption = (Hediff_MajinAbsorbption)this.pawn.health.GetOrAddHediff(MajinDefOf.SR_AbsorptionHediff);

            if (majinAbsorbption != null)
            {
                majinAbsorbption.IncreaseSeverity(SeverityIncrease);
                majinAbsorbption.RecordAbsorbption(target.Pawn.LabelShort);
            }

        }



        public bool TryGetKiAbilityClass(Pawn Pawn, out AbilityClassKI abilityClass)
        {
            abilityClass = null;

            var compAbility = Pawn.GetComp<CompAbilities>();

            if (compAbility == null) return false;

            if (compAbility.TryGetKIAbilityClass(out AbilityClassKI kiClass))
            {
                abilityClass = kiClass;
                return true;
            }

            return false;
        }
    }
}
