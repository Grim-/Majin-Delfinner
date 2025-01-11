using SaiyanMod;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using AbilityDef = TaranMagicFramework.AbilityDef;

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

        public override Vector2 InitialSize => WinSize;

        private Action<AbilityDef> onAbilitySelected;
        private Action onCanceled;
        private Pawn targetPawn;
        private List<AbilityDef> availableAbilities;

        public Window_AbilitySelection(Pawn target, Action<AbilityDef> onAbilitySelected, Action onCanceled = null)
        {
            this.targetPawn = target;
            this.onAbilitySelected = onAbilitySelected;
            this.onCanceled = onCanceled;
            this.doCloseX = true;
            this.doCloseButton = true;
            this.absorbInputAroundWindow = true;
            this.forcePause = true;

            this.availableAbilities = new List<AbilityDef>();
            if (target.TryGetKiAbilityClass(out AbilityClassKI kiClass))
            {
                foreach (var ability in kiClass.LearnedAbilities)
                {
                    this.availableAbilities.Add(ability.def);
                }
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            DrawTitle(inRect);

            float totalHeight = CalculateTotalHeight();


            Rect outRect = new Rect(0f, 40f, inRect.width, inRect.height - 40f);
            Rect viewRect = new Rect(0f, 0f, outRect.width - 16f, totalHeight);

            if (availableAbilities.Count == 0)
            {
                DrawNoAbilitiesMessage(outRect);
                return;
            }

            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
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
            return 40f + (Mathf.Ceil(availableAbilities.Count / (float)ABILITIES_PER_ROW) * (ABILITY_BUTTON_SIZE + BUTTON_PADDING));
        }

        private void DrawAbilityGrid(Rect viewRect)
        {
            float curX = 0f;
            float curY = 0f;

            foreach (AbilityDef ability in availableAbilities)
            {
                if (curX + ABILITY_BUTTON_SIZE > viewRect.width)
                {
                    curX = 0f;
                    curY += ABILITY_BUTTON_SIZE + BUTTON_PADDING;
                }

                Rect buttonRect = new Rect(curX, curY, ABILITY_BUTTON_SIZE, ABILITY_BUTTON_SIZE);
                DrawAbilityButton(buttonRect, ability);
                curX += ABILITY_BUTTON_SIZE + BUTTON_PADDING;
            }
        }

        private void DrawAbilityButton(Rect buttonRect, TaranMagicFramework.AbilityDef ability)
        {
            Widgets.DrawBox(buttonRect, 1, SolidColorMaterials.NewSolidColorTexture(Color.grey));

            Rect innerRect = buttonRect.ContractedBy(INNER_PADDING);
            float labelHeight = LABEL_HEIGHT;

            Rect iconRect = new Rect(
                innerRect.x,
                innerRect.y,
                innerRect.width,
                innerRect.height - labelHeight
            );

            if (!string.IsNullOrEmpty(ability.uiSkillIcon))
            {
                Widgets.DrawTextureFitted(iconRect, ContentFinder<Texture2D>.Get(ability.uiSkillIcon), 0.8f);
            }

            Rect footerRect = new Rect(
                buttonRect.x,
                buttonRect.y + buttonRect.height - labelHeight,
                buttonRect.width,
                labelHeight
            );
            GUI.DrawTexture(footerRect, SolidColorMaterials.NewSolidColorTexture(FOOTER_COLOR));

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(footerRect, ability.label ?? ability.defName);

            if (Mouse.IsOver(buttonRect))
            {
                Widgets.DrawHighlight(buttonRect);
                TooltipHandler.TipRegion(buttonRect, ability.description);

                if (Widgets.ButtonInvisible(buttonRect))
                {
                    onAbilitySelected?.Invoke(ability);
                    Close();
                }
            }

            Text.Anchor = TextAnchor.UpperLeft;
        }

        public override void PostClose()
        {
            base.PostClose();
            if (!Find.WindowStack.Windows.Contains(this))
            {
                onCanceled?.Invoke();
            }
        }
    }
}
