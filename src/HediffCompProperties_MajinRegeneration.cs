using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Majin
{
    public class HediffCompProperties_MajinRegeneration : HediffCompProperties
    {
        public int RegenLimbTicks = 2400;

        public HediffCompProperties_MajinRegeneration()
        {
            compClass = typeof(HediffComp_MajinRegeneration);
        }
    }

    public class HediffComp_MajinRegeneration : HediffComp
    {
        HediffCompProperties_MajinRegeneration Props => (HediffCompProperties_MajinRegeneration)props;
        private Hediff_MissingPart CurrentPartTarget = null;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (this.Pawn.IsHashIntervalTick(90))
            {
                if (CurrentPartTarget == null)
                {
                    List<Hediff_MissingPart> parts = GetMissingPartsPrioritized(this.Pawn);
                    if (parts == null || parts.Count == 0)
                    {
                        return;
                    }
                    CurrentPartTarget = parts[0];
                }

                if (CurrentPartTarget != null)
                {
                    float maxHealth = CurrentPartTarget.Part.def.GetMaxHealth(this.Pawn);
                    float healingPerSecond = maxHealth / (GenDate.TicksPerHour / 60f);

                    float currentSeverity = CurrentPartTarget.Severity;
                    if (currentSeverity <= 0)
                    {
                        CurrentPartTarget = null;
                        return;
                    }

                    HealthUtility.AdjustSeverity(this.Pawn, CurrentPartTarget.def, -healingPerSecond);
                }
            }
        }

        public static List<Hediff_MissingPart> GetMissingPartsPrioritized(Pawn pawn)
        {
            return pawn.health.hediffSet.hediffs
                .OfType<Hediff_MissingPart>()
                .OrderByDescending(x => x.Part.def.hitPoints)
                .ThenByDescending(x => x.Part.def.GetMaxHealth(pawn))
                .ToList();
        }
    }
}
