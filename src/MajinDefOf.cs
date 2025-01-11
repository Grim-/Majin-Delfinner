using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaranMagicFramework;
using Verse;
using AbilityDef = TaranMagicFramework.AbilityDef;

namespace Majin
{
    [DefOf]
    public class MajinDefOf
    {
        public static PawnKindDef SR_MajinRace;
        public static HediffDef SR_AbsorptionHediff;

        public static GeneDef SR_MajinGene;

        public static ThingDef MajinCandy;
        public static ThingDef MajinHealBeamProjectile;
        public static ThingDef MajinBeamProjectile;

        public static AbilityDef SR_CandyBeam;
        public static AbilityDef SR_Fission;
    }
}
