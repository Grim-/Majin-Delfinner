using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Majin
{
    public class Hediff_MajinAbsorbption : HediffWithComps
    {
        private int AbsorbCounter = 0;
        private List<string> AbsorbedNames = new List<string>();

        public override string Description
        {
            get
            {
                StringBuilder description = new StringBuilder(base.Description);
                description.AppendLine("\n\nAbsorbed beings:");

                if (AbsorbedNames == null || AbsorbedNames.Count == 0)
                {
                    description.AppendLine("None... yet.");
                    return description.ToString();
                }

                if (AbsorbedNames.Count <= 20)
                {
                    description.AppendLine(string.Join("\n", AbsorbedNames));
                }
                else
                {
                    description.AppendLine(string.Join("\n", AbsorbedNames.Take(20)));
                    description.AppendLine("...and countless others lost to the void.");
                }

                return description.ToString();
            }
        }

        public void IncreaseSeverity(float Amount)
        {
            this.Severity += Amount;
        }

        public void RecordAbsorbption(string PawnName)
        {
            this.AbsorbCounter++;
            AbsorbedNames.Add(PawnName);
        }

        public void ReduceSeverity(float Amount)
        {
            this.Severity -= Amount;
        }


        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref AbsorbCounter, "absorbCounter", 0);
            Scribe_Collections.Look(ref AbsorbedNames, "absorbedNames");
        }

    }
}
