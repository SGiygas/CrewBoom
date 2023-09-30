﻿using BepInEx.Configuration;
using BepInEx.Logging;
using BrcCustomCharactersLib;
using Reptile;
using System;

namespace CrewBoom
{
    public static class CharacterConfig
    {
        private const string CONFIG_DESCRIPTION = "Enter a GUID of a character bundle to always load for {0} (Blank = Auto-detect, \"OFF\" = Default character for you)";

        private static ConfigEntry<string>[] _characterIdOverrides;

        public static void Initialize(ConfigFile config)
        {
            var values = Enum.GetValues(typeof(Characters));
            _characterIdOverrides = new ConfigEntry<string>[values.Length - 2];
            foreach (Characters character in values)
            {
                if (character == Characters.NONE || character == Characters.MAX)
                {
                    continue;
                }

                BrcCharacter characterName = (BrcCharacter)character;
                _characterIdOverrides[(int)character] = config.Bind<string>("Replacement IDs", characterName.ToString(), null, string.Format(CONFIG_DESCRIPTION, characterName)); ;
            }
        }

        public static bool GetCharacterOverride(Characters character, out Guid id, out bool isDisabled)
        {
            id = Guid.Empty;
            isDisabled = false;

            string guidString = _characterIdOverrides[(int)character].Value;
            if (guidString == string.Empty)
            {
                return false;
            }
            if (guidString == "OFF")
            {
                isDisabled = true;
                return false;
            }

            if (Guid.TryParse(guidString, out id))
            {
                return true;
            }

            return false;
        }
    }
}