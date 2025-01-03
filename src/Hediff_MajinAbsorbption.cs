using RimWorld;
using SaiyanMod;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using static UnityEngine.GraphicsBuffer;

namespace Majin
{
    public class Hediff_MajinAbsorbption : HediffWithComps
    {
        private List<Pawn> AbsorbedPawns = new List<Pawn>();

        public override string Description
        {
            get
            {
                StringBuilder description = new StringBuilder(base.Description);
                description.AppendLine("\n\nAbsorbed beings:");

                if (AbsorbedPawns == null || AbsorbedPawns.Count == 0)
                {
                    description.AppendLine("None... yet.");
                    return description.ToString();
                }

                if (AbsorbedPawns.Count <= 20)
                {
                    description.AppendLine(string.Join("\n", AbsorbedPawns));
                }
                else
                {
                    description.AppendLine(string.Join("\n", AbsorbedPawns.Take(20)));
                    description.AppendLine("...and countless others lost to the void.");
                }

                return description.ToString();
            }
        }

        public void IncreaseSeverity(float Amount)
        {
            this.Severity += Amount;
        }

        public void RecordAbsorbption(Pawn PawnToAbsorb)
        {
            AbsorbedPawns.Add(PawnToAbsorb);     

            float severity = 1;
            if (PawnToAbsorb.TryGetKiAbilityClass(out AbilityClassKI abilityClassKI))
            {
                severity += 1;
            }
            IncreaseSeverity(severity);

            PawnToAbsorb.SafeMoveToWorld();
        }


        public void ReleaseAllPawns()
        {
            foreach (var item in AbsorbedPawns.ToList())
            {
                ReleaseAbsorbedPawn(item);
            }
        }

        public void ReleaseAbsorbedPawn(Pawn Pawn)
        {
            if (AbsorbedPawns.Contains(Pawn))
            {
                Pawn.SafeReturnToMap(this.pawn.Map, this.pawn.Position);
                AbsorbedPawns.Remove(Pawn);
            }
        }

        public void ReduceSeverity(float Amount)
        {
            this.Severity -= Amount;
        }


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref AbsorbedPawns, "absorbedPawns");
        }

    }


}
