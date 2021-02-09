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
    [BepInPlugin("me.drumsy.dsp_craftmany", "Craft Many Plug-In", "0.0.1.0")]
    [BepInProcess("DSPGAME.exe")]
    class CraftMany : BaseUnityPlugin
    {
        private Harmony harmony = new Harmony("me.drumsy.dsp_craftmany");

        private static int maxQuantity = 200;

        void Awake()
        {
            harmony.PatchAll();

            Debug.Log("Craft Many mod has been loaded!");
        }

        void OnDestroy()
        {
            harmony.UnpatchSelf();

            Debug.Log("Craft Many mod has been unloaded!");
        }

        [HarmonyPatch(typeof(UIReplicatorWindow), "OnOkButtonClick")]
        public static class UIReplicatorWindow__OnOkButtonClick
        {
            static bool Prefix(UIReplicatorWindow __instance, RecipeProto ___selectedRecipe, MechaForge ___mechaForge)
            {
                if (___selectedRecipe == null)
                    return false;

                if (!___selectedRecipe.Handcraft)
                {
                    UIRealtimeTip.Popup("该配方".Translate() + ___selectedRecipe.madeFromString + "生产".Translate(), true, 0);
                }
                else
                {
                    int id = ___selectedRecipe.ID;
                    if (!GameMain.history.RecipeUnlocked(id))
                    {
                        UIRealtimeTip.Popup("配方未解锁".Translate(), true, 0);
                    }
                    else
                    {
                        int num = ___mechaForge.PredictTaskCount(___selectedRecipe.ID, 500);

                        var count = 1;

                        if (Input.GetKey(KeyCode.LeftControl)) count *= 10;
                        if (Input.GetKey(KeyCode.LeftShift)) count *= 100;
                        if (Input.GetKey(KeyCode.LeftAlt)) count *= num;

                        if (count > num)
                            count = num;
                        if (count == 0)
                            UIRealtimeTip.Popup("材料不足".Translate(), true, 0);
                        else if (___mechaForge.AddTask(id, count) == null)
                            UIRealtimeTip.Popup("材料不足".Translate(), true, 0);
                        else
                            GameMain.history.RegFeatureKey(1000104);

                    }
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(UIReplicatorWindow), "_OnOpen")]
        public static class UIReplicatorWindow___OnCreate
        {
            static void Postfix(UIReplicatorWindow __instance, UIButton ___plusButton, UIButton ___minusButton, Text ___multiValueText)
            {
                ___plusButton.gameObject.SetActive(false);
                ___minusButton.gameObject.SetActive(false);
                ___multiValueText.gameObject.transform.parent.gameObject.SetActive(false);
            }
        }

        [HarmonyPatch(typeof(UIReplicatorWindow), "_OnClose")]
        public static class UIReplicatorWindow___OnDestroy
        {
            static void Postfix(UIReplicatorWindow __instance, UIButton ___plusButton, UIButton ___minusButton, Text ___multiValueText)
            {
                ___plusButton.gameObject.SetActive(true);
                ___minusButton.gameObject.SetActive(true);
                ___multiValueText.gameObject.transform.parent.gameObject.SetActive(true);
            }
        }
    }
}
