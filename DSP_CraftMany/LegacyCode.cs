using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BepInEx;
using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;

namespace DSP_CraftMany
{
    class LegacyCode
    {
        private static int maxQuantity = 200;

        [HarmonyPatch(typeof(UIReplicatorWindow), "OnPlusButtonClick")]
        public static class UIReplicatorWindow__OnPlusButtonClick
        {
            static bool Prefix(UIReplicatorWindow __instance, ref RecipeProto ___selectedRecipe, ref Text ___multiValueText)
            {
                if (___selectedRecipe == null)
                    return false;

                var stepModifier = 1;

                if (Input.GetKey(KeyCode.LeftControl)) stepModifier = 10;
                if (Input.GetKey(KeyCode.LeftShift)) stepModifier = 100;

                if (!__instance.multipliers.ContainsKey(___selectedRecipe.ID))
                    __instance.multipliers[___selectedRecipe.ID] = 1;

                int num = __instance.multipliers[___selectedRecipe.ID] + 1 * stepModifier;
                if (num > maxQuantity) num = maxQuantity;

                __instance.multipliers[___selectedRecipe.ID] = num;
                ___multiValueText.text = num.ToString() + "x";

                return false;
            }
        }

        [HarmonyPatch(typeof(UIReplicatorWindow), "OnMinusButtonClick")]
        public static class UIReplicatorWindow__OnMinusButtonClick
        {
            static bool Prefix(UIReplicatorWindow __instance, ref RecipeProto ___selectedRecipe, ref Text ___multiValueText)
            {
                if (___selectedRecipe == null)
                    return false;

                var stepModifier = 1;

                if (Input.GetKey(KeyCode.LeftControl)) stepModifier = 10;
                if (Input.GetKey(KeyCode.LeftShift)) stepModifier = 100;

                if (!__instance.multipliers.ContainsKey(___selectedRecipe.ID))
                    __instance.multipliers[___selectedRecipe.ID] = 1;

                int num = __instance.multipliers[___selectedRecipe.ID] - 1 * stepModifier;
                if (num < 1) num = 1;

                __instance.multipliers[___selectedRecipe.ID] = num;
                ___multiValueText.text = num.ToString() + "x";

                return false;
            }
        }
    }
}
