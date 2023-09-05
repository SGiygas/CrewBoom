﻿using BepInEx.Logging;
using BrcCustomCharactersLib;
using HarmonyLib;
using Reptile;
using UnityEngine;

namespace BrcCustomCharacters.Patches
{
    [HarmonyPatch(typeof(Reptile.CharacterLoader), nameof(Reptile.CharacterLoader.GetCharacterFbx))]
    public class GetFbxPatch
    {
        public static void Postfix(Characters character, ref GameObject __result)
        {
            //if (BrcCustomCharactersAPI.Database.GetNextOverride(out System.Guid id))
            //{
            //    AssetDatabase.GetCharacterReplacement(id, out CharacterDefinition overrideCharacter);
            //    __result = overrideCharacter.gameObject;
            //    return;
            //}

            if (AssetDatabase.GetCharacterReplacement(character, out CharacterDefinition characterObject))
            {
                __result = characterObject.gameObject;
            }
        }
    }

    [HarmonyPatch(typeof(Reptile.CharacterLoader), nameof(Reptile.CharacterLoader.GetCharacterMaterial))]
    public class GetMaterialPatch
    {
        public static void Postfix(Characters character, int outfitIndex, ref Material __result)
        {
            //if (BrcCustomCharactersAPI.Database.GetNextOverride(out System.Guid id))
            //{
            //    AssetDatabase.GetCharacterReplacement(id, out CharacterDefinition overrideCharacter);
            //    __result = overrideCharacter.Outfits[outfitIndex];
            //    return;
            //}

            if (AssetDatabase.GetCharacterReplacement(character, out CharacterDefinition characterObject))
            {
                Material material = characterObject.Outfits[outfitIndex];
                if (characterObject.UseReptileShader)
                {
                    material.shader = __result.shader;
                }
                __result = material;
            }
        }
    }

    [HarmonyPatch(typeof(Reptile.CharacterConstructor), nameof(Reptile.CharacterConstructor.GetCharacterMaterials))]
    public class GetMaterialsPatch
    {
        public static void Postfix(Material[,] __result)
        {
            for (int i = 0; i < __result.Length; i++)
            {
                Characters character = (Characters)i;
                if (AssetDatabase.GetCharacterReplacement(character, out CharacterDefinition characterObject))
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Material material = characterObject.Outfits[j];
                        if (characterObject.UseReptileShader)
                        {
                            material.shader = __result[i, j].shader;
                        }
                        __result[i, j] = material;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Reptile.CharacterVisual), "SetInlineSkatesPropsMode")]
    public class InlineSkatesTransformPatch
    {
        static void Postfix(CharacterVisual.MoveStylePropMode mode,
                            Transform ___footL,
                            Transform ___footR,
                            PlayerMoveStyleProps ___moveStyleProps,
                            CharacterVisual __instance)
        {
            if (mode != CharacterVisual.MoveStylePropMode.ACTIVE)
            {
                return;
            }

            Player player = __instance.GetComponentInParent<Player>(true);
            if (player == null)
            {
                return;
            }

            Characters character = (Characters)player.GetField("character").GetValue(player);
            if (AssetDatabase.HasCharacter(character))
            {
                Transform offsetL = ___footL.Find(CharUtil.SKATE_OFFSET_L);
                Transform offsetR = ___footR.Find(CharUtil.SKATE_OFFSET_R);

                if (offsetL != null && offsetR != null)
                {
                    ___moveStyleProps.skateL.transform.SetLocalPositionAndRotation(offsetL.localPosition, offsetL.localRotation);
                    ___moveStyleProps.skateL.transform.localScale = offsetL.localScale;
                    ___moveStyleProps.skateR.transform.SetLocalPositionAndRotation(offsetR.localPosition, offsetR.localRotation);
                    ___moveStyleProps.skateR.transform.localScale = offsetR.localScale;
                }
            }
        }
    }
}
