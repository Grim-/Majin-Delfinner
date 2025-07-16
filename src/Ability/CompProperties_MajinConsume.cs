using RimWorld;
using Verse;

namespace Majin
{
    public class CompProperties_MajinConsume : CompProperties_UseEffect
    {
        public bool RestoreParts = true;
        public bool HealInjuries = true;

        public CompProperties_MajinConsume()
        {
            compClass = typeof(MajinConsumeEffect);
        }
    }

    public class MajinConsumeEffect : CompUseEffect
    {
        CompProperties_MajinConsume Props => (CompProperties_MajinConsume)props;

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            if (!p.genes.HasActiveGene(MajinDefOf.SR_MajinGene))
            {
                return AcceptanceReport.WasRejected;
            }

            return base.CanBeUsedBy(p);
        }

        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);

            CompStoredPawn compStoredPawn = this.parent.GetComp<CompStoredPawn>();

            if (compStoredPawn != null && compStoredPawn.HasStoredPawn())
            {
                Gene_Majin majinAbsorbption = usedBy.genes.GetFirstGeneOfType<Gene_Majin>();
                if (majinAbsorbption != null)
                {
                    majinAbsorbption.RecordAbsorbption(compStoredPawn.Pawn);
                    compStoredPawn.DestroyStoredPawn();
                }
            }
        }
    }
}
