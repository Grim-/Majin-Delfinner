﻿using RimWorld;
using UnityEngine;
using Verse;

namespace Majin
{
    public class ITab_MajinAbsorption : ITab
    {
        private Vector2 scrollPosition = Vector2.zero;
        private const float ROW_HEIGHT = 40f;
        private const float ICON_SIZE = 30f;
        private const float LABEL_WIDTH = 100f;
        private const float BUTTON_WIDTH = 220f;
        private const float COLUMN_SPACING = 5f;

        public override bool IsVisible => base.IsVisible && this.SelPawn != null && this.SelPawn.genes != null &&
            this.SelPawn.genes.GetFirstGeneOfType<Gene_Majin>() != null;

        public ITab_MajinAbsorption()
        {
            this.labelKey = "TabMajinAbsorption";
            this.tutorTag = "MajinAbsorption";
            this.size = new Vector2(450f, 450f);
        }

        protected override void FillTab()
        {
            Rect rect = new Rect(0f, 0f, this.size.x, this.size.y).ContractedBy(10f);
            Rect viewRect = new Rect(0f, 0f, rect.width - 16f, 1000f);
            Pawn pawn = (Pawn)this.SelPawn;
            Gene_Majin majinAbsorption = pawn.genes.GetFirstGeneOfType<Gene_Majin>();
            if (majinAbsorption == null) 
                return;

            Widgets.BeginScrollView(rect, ref scrollPosition, viewRect);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(viewRect);

            if (pawn != null && majinAbsorption != null)
            {
                if (majinAbsorption.AbsorbedPawns != null)
                {
                    listingStandard.Label($"Total Absorbed Beings: {majinAbsorption.AbsorbedPawns.Count}");
                    listingStandard.GapLine();

                    if (majinAbsorption.AbsorbedPawns.Count > 0)
                    {
                        if (listingStandard.ButtonText("Release All"))
                        {
                            majinAbsorption.ReleaseAllPawns();
                        }
                        listingStandard.GapLine();
                    }

                    foreach (var absorbedPawn in majinAbsorption.AbsorbedPawns.ToArray())
                    {
                        if (absorbedPawn == null)
                        {
                            continue;
                        }

                        DrawRow(pawn, majinAbsorption, absorbedPawn, listingStandard);
                    }
                }
                else
                {
                    listingStandard.Label("No absorbed beings");
                }
            }
            else
            {
                listingStandard.Label("No Majin Absorption data available");
            }

            listingStandard.End();
            Widgets.EndScrollView();
        }

        private void DrawRow(Pawn pawn, Gene_Majin majinAbsorption, Pawn absorbedPawn, Listing_Standard listingStandard)
        {
            Rect rowRect = listingStandard.GetRect(ROW_HEIGHT);
            var layout = new RowLayoutManager(rowRect);

            Rect iconRect = layout.NextRect(ICON_SIZE, COLUMN_SPACING);
            Rect labelRect = layout.NextRect(LABEL_WIDTH, COLUMN_SPACING);
            Rect releaseButtonRect = layout.NextRect(BUTTON_WIDTH);

            if (absorbedPawn.def.uiIcon != null)
            {
                Widgets.DrawTextureFitted(iconRect, absorbedPawn.def.uiIcon, 1f);
                Widgets.HyperlinkWithIcon(iconRect, new Dialog_InfoCard.Hyperlink(absorbedPawn.def));
            }

            Widgets.Label(labelRect, absorbedPawn.Label ?? absorbedPawn.def.label);

            if (Widgets.ButtonText(releaseButtonRect, "Release"))
            {
                majinAbsorption.ReleaseAbsorbedPawn(absorbedPawn);
            }
        }

    }
}
