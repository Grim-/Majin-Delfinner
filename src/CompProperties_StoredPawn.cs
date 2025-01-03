﻿using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace Majin
{
    public class CompProperties_StoredPawn : CompProperties
    {
        public CompProperties_StoredPawn()
        {
            compClass = typeof(CompStoredPawn);
        }
    }

    public class CompStoredPawn : ThingComp
    {
        private Pawn StoredPawn;
        public Pawn Pawn => StoredPawn;

        public void StorePawn(Pawn pawn)
        {
            StoredPawn = pawn;
            if (StoredPawn != null)
            {
                StoredPawn.SafeMoveToWorld();
            }       
        }

        public bool HasStoredPawn()
        {
            return StoredPawn != null;
        }

        public Pawn ReleasePawn(IntVec3 releasePosition)
        {
            var pawn = StoredPawn;

            if (pawn != null)
            {
                pawn.SafeReturnToMap(this.parent.Map, this.parent.Position);
            }

            StoredPawn = null;
            return pawn;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if (respawningAfterLoad)
            {
                StorePawn(StoredPawn);
            }
        }

        public override string CompInspectStringExtra()
        {
            if (StoredPawn == null)
            {
                return "No pawn stored";
            }

            TaggedString pawnInfo = "Stored pawn: " + StoredPawn.NameFullColored;
            return pawnInfo;
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selectionPawn)
        {
            foreach (FloatMenuOption option in base.CompFloatMenuOptions(selectionPawn))
            {
                yield return option;
            }

            Pawn storedPawn = StoredPawn;
            yield return new FloatMenuOption($"View stored pawn info", () =>
            {
                Find.WindowStack.Add(new Dialog_InfoCard(storedPawn));
            });
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Deep.Look(ref StoredPawn, "storedPawn");
        }
    }
}
