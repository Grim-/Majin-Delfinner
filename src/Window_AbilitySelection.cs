using SaiyanMod;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using AbilityDef = TaranMagicFramework.AbilityDef;
using RimWorld;

namespace Majin
{
    public class Window_AbilitySelection : Window
    {
        private Vector2 scrollPosition = Vector2.zero;
        private static readonly Vector2 WinSize = new Vector2(800f, 600f);
        private const float ABILITY_BUTTON_SIZE = 120f;
        private const float BUTTON_PADDING = 10f;
        private const float LABEL_HEIGHT = 40f;
        private const float TITLE_HEIGHT = 35f;
        private const int ABILITIES_PER_ROW = 4;
        private const float INNER_PADDING = 5f;

        private Color TITLE_COLOR = new Color(0.3f, 0.3f, 0.3f, 0.3f);
        private Color FOOTER_COLOR = new Color(0.3f, 0.3f, 0.3f, 0.3f);

        private Pawn targetPawn;
        private bool selectionMade = false;

        private List<AbilityDef> availableFrameworkAbilities;
        private List<RimWorld.AbilityDef> availableRimWorldAbilities;

        private Action<AbilityDef> onFrameworkAbilitySelected;
        private Action<RimWorld.AbilityDef> onRimWorldAbilitySelected;
        private Action onCanceled;

        public override Vector2 InitialSize => WinSize;

        public Window_AbilitySelection(Pawn target, List<AbilityDef> frameworkAbilities, List<RimWorld.AbilityDef> rimworldAbilities, Action<AbilityDef> onFrameworkAbilitySelected, Action<RimWorld.AbilityDef> onRimWorldAbilitySelected, Action onCanceled = null)
        {
            this.targetPawn = target;
            this.availableFrameworkAbilities = frameworkAbilities ?? new List<AbilityDef>();
            this.availableRimWorldAbilities = rimworldAbilities ?? new List<RimWorld.AbilityDef>();
            this.onFrameworkAbilitySelected = onFrameworkAbilitySelected;
            this.onRimWorldAbilitySelected = onRimWorldAbilitySelected;
            this.onCanceled = onCanceled;

            this.doCloseX = true;
            this.doCloseButton = true;
            this.absorbInputAroundWindow = true;
            this.forcePause = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            DrawTitle(inRect);

            float contentY = TITLE_HEIGHT + 5f;
            Rect contentRect = new Rect(0f, contentY, inRect.width, inRect.height - contentY - 50f);

            if (availableFrameworkAbilities.Count == 0 && availableRimWorldAbilities.Count == 0)
            {
                DrawNoAbilitiesMessage(contentRect);
                return;
            }

            float totalContentHeight = CalculateTotalHeight();
            Rect viewRect = new Rect(0f, 0f, contentRect.width - 16f, totalContentHeight);

            Widgets.BeginScrollView(contentRect, ref scrollPosition, viewRect);
            DrawAbilityGrid(viewRect);
            Widgets.EndScrollView();
        }

        private void DrawNoAbilitiesMessage(Rect rect)
        {
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, $"{targetPawn.LabelShort} has no abilities to absorb.");
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DrawTitle(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect titleRect = new Rect(0f, 0f, inRect.width, TITLE_HEIGHT);
            GUI.DrawTexture(titleRect, SolidColorMaterials.NewSolidColorTexture(TITLE_COLOR));
            Widgets.Label(titleRect, $"Select Ability to Absorb from {targetPawn.LabelShort}");
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private float CalculateTotalHeight()
        {
            int totalAbilities = availableFrameworkAbilities.Count + availableRimWorldAbilities.Count;
            if (totalAbilities == 0) return 0f;

            int numRows = Mathf.CeilToInt(totalAbilities / (float)ABILITIES_PER_ROW);
            return numRows * (ABILITY_BUTTON_SIZE + BUTTON_PADDING);
        }

        private void DrawAbilityGrid(Rect viewRect)
        {
            float curX = 0f;
            float curY = 0f;

            void NextButtonPos()
            {
                curX += ABILITY_BUTTON_SIZE + BUTTON_PADDING;
                if (curX + ABILITY_BUTTON_SIZE > viewRect.width - INNER_PADDING)
                {
                    curX = 0f;
                    curY += ABILITY_BUTTON_SIZE + BUTTON_PADDING;
                }
            };

            foreach (var ability in availableFrameworkAbilities)
            {
                Rect buttonRect = new Rect(curX, curY, ABILITY_BUTTON_SIZE, ABILITY_BUTTON_SIZE);
                DrawAbilityButtonInternal(buttonRect, ability.icon, ability.LabelCap, ability.description, () =>
                {
                    selectionMade = true;
                    onFrameworkAbilitySelected?.Invoke(ability);
                    Close();
                });
                NextButtonPos();
            }

            foreach (var ability in availableRimWorldAbilities)
            {
                Rect buttonRect = new Rect(curX, curY, ABILITY_BUTTON_SIZE, ABILITY_BUTTON_SIZE);
                DrawAbilityButtonInternal(buttonRect, ability.uiIcon, ability.LabelCap, ability.description, () =>
                {
                    selectionMade = true;
                    onRimWorldAbilitySelected?.Invoke(ability);
                    Close();
                });
                NextButtonPos();
            }
        }

        private void DrawAbilityButtonInternal(Rect buttonRect, Texture2D icon, string label, string description, Action onSelect)
        {
            Widgets.DrawBox(buttonRect, 1, SolidColorMaterials.NewSolidColorTexture(Color.grey));
            Rect innerRect = buttonRect.ContractedBy(INNER_PADDING);

            Rect iconRect = new Rect(innerRect.x, innerRect.y, innerRect.width, innerRect.height - LABEL_HEIGHT);
            if (icon != null)
            {
                Widgets.DrawTextureFitted(iconRect, icon, 0.8f);
            }

            Rect footerRect = new Rect(buttonRect.x, buttonRect.yMax - LABEL_HEIGHT, buttonRect.width, LABEL_HEIGHT);
            GUI.DrawTexture(footerRect, SolidColorMaterials.NewSolidColorTexture(FOOTER_COLOR));
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(footerRect, label);
            Text.Anchor = TextAnchor.UpperLeft;

            if (Mouse.IsOver(buttonRect))
            {
                Widgets.DrawHighlight(buttonRect);
                TooltipHandler.TipRegion(buttonRect, description ?? "No description.");

                if (Widgets.ButtonInvisible(buttonRect))
                {
                    onSelect?.Invoke();
                }
            }
        }

        public override void PostClose()
        {
            base.PostClose();
            if (!selectionMade)
            {
                onCanceled?.Invoke();
            }
        }
    }
}