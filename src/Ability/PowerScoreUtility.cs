using SaiyanMod;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Majin
{
    public static class PowerScoreUtility
    {
        private const float BASE_LEVEL_MULTIPLIER = 100f;
        private const float XP_MULTIPLIER = 0.01f;
        private const float ABILITY_BASE_SCORE = 50f;
        private const float ABILITY_TIER_MULTIPLIER = 1.5f;

        /// <summary>
        /// Calculates the total power score for a pawn based on their KI abilities and experience
        /// </summary>
        public static float CalculatePowerScore(Pawn pawn)
        {
            if (pawn == null) return 0f;

            float totalScore = 0f;

            if (pawn.TryGetKiAbilityClass(out AbilityClassKI kiClass))
            {

                totalScore += CalculateLevelScore(kiClass.level);
                totalScore += CalculateXPScore(kiClass.xpPoints);
                totalScore += CalculateAbilitiesScore(kiClass.LearnedAbilities);
            }

            return totalScore;
        }

        /// <summary>
        /// Calculates score contribution from the pawn's KI level
        /// </summary>
        public static float CalculateLevelScore(int level)
        {
            return level * BASE_LEVEL_MULTIPLIER;
        }

        /// <summary>
        /// Calculates score contribution from accumulated XP
        /// </summary>
        public static float CalculateXPScore(float xp)
        {
            return xp * XP_MULTIPLIER;
        }

        /// <summary>
        /// Calculates total score contribution from all learned abilities
        /// </summary>
        public static float CalculateAbilitiesScore(IEnumerable<TaranMagicFramework.Ability> abilities)
        {
            if (abilities == null) return 0f;

            return abilities.Sum(ability => CalculateSingleAbilityScore(ability));
        }

        /// <summary>
        /// Calculates score for a single ability including its tiers
        /// </summary>
        public static float CalculateSingleAbilityScore(TaranMagicFramework.Ability ability)
        {
            if (ability?.def == null) return 0f;

            float baseScore = ABILITY_BASE_SCORE;

            int abilityLevel = ability.LevelHumanReadable;
            float tierScore = baseScore * abilityLevel;

            return baseScore + tierScore;
        }

        /// <summary>
        /// Gets a text description of a pawn's power score components
        /// </summary>
        public static string GetPowerScoreBreakdown(Pawn pawn)
        {
            if (pawn == null) return "Invalid pawn";

            if (!pawn.TryGetKiAbilityClass(out AbilityClassKI kiClass))
                return "No KI abilities";

            float levelScore = CalculateLevelScore(kiClass.level);
            float xpScore = CalculateXPScore(kiClass.xpPoints);
            float abilitiesScore = CalculateAbilitiesScore(kiClass.LearnedAbilities);
            float totalScore = levelScore + xpScore + abilitiesScore;

            return $"Power Score Breakdown for {pawn.LabelShort}:\n" +
                   $"Level ({kiClass.level}): {levelScore:F0}\n" +
                   $"Experience: {xpScore:F0}\n" +
                   $"Abilities: {abilitiesScore:F0}\n" +
                   $"Total Score: {totalScore:F0}";
        }

        /// <summary>
        /// Gets relative power level description based on score
        /// </summary>
        public static string GetPowerLevelDescription(float score)
        {
            if (score < 500) return "Weak";
            if (score < 1000) return "Average";
            if (score < 2500) return "Strong";
            if (score < 5000) return "Very Strong";
            if (score < 10000) return "Extremely Powerful";
            return "Legendary";
        }

        /// <summary>
        /// Compares two pawns and returns a relative strength description
        /// </summary>
        public static string ComparePowerLevels(Pawn pawn1, Pawn pawn2)
        {
            if (pawn1 == null || pawn2 == null) return "Invalid comparison";

            float score1 = CalculatePowerScore(pawn1);
            float score2 = CalculatePowerScore(pawn2);

            float ratio = score1 / score2;

            if (ratio > 2f)
                return $"{pawn1.LabelShort} is vastly stronger than {pawn2.LabelShort}";
            if (ratio > 1.5f)
                return $"{pawn1.LabelShort} is notably stronger than {pawn2.LabelShort}";
            if (ratio > 1.2f)
                return $"{pawn1.LabelShort} is slightly stronger than {pawn2.LabelShort}";
            if (ratio > 0.8f)
                return $"{pawn1.LabelShort} and {pawn2.LabelShort} are roughly equal";
            if (ratio > 0.5f)
                return $"{pawn1.LabelShort} is slightly weaker than {pawn2.LabelShort}";
            if (ratio > 0.25f)
                return $"{pawn1.LabelShort} is notably weaker than {pawn2.LabelShort}";
            return $"{pawn1.LabelShort} is vastly weaker than {pawn2.LabelShort}";
        }
    }
}
