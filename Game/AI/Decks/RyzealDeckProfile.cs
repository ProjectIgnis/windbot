using System;
using System.Collections.Generic;

namespace RyzealAI.Prototype
{
    // Single source of truth for AI_Ryzeal - 복사본.txt (40 main / 15 extra).
    // This is deck composition metadata only; the duel engine remains the authority
    // for current in-duel availability and legal actions.
    public static class RyzealDeckProfile
    {
        public const string Name = "AI_Ryzeal";
        public const int MainDeckSize = 40;
        public const int ExtraDeckSize = 15;

        public static readonly Dictionary<int, int> Main =
            new Dictionary<int, int>
            {
                { 27204311, 1 }, // Nibiru
                { 84433129, 1 }, // Star Ryzeal
                { 8633261, 3 },  // Ice Ryzeal
                { 67750322, 1 }, // Skull Meister
                { 35844557, 3 }, // Sword Ryzeal
                { 72238166, 1 }, // Node Ryzeal
                { 34022970, 3 }, // Ext Ryzeal
                { 67972302, 2 }, // Sakitama
                { 42141493, 3 }, // Mulcharmy Fuwalos
                { 84192580, 2 }, // Mulcharmy Purulia
                { 14558127, 2 }, // Ash Blossom
                { 59438930, 1 }, // Ghost Ogre
                { 73642296, 1 }, // Ghost Belle
                { 23434538, 1 }, // Maxx "C"
                { 7477101, 1 }, // Seventh Tachyon
                { 25311006, 1 }, // Triple Tactics Talent
                { 84211599, 1 }, // Pot of Prosperity
                { 85106525, 1 }, // Bonfire
                { 89753095, 3 }, // Lightning Crash
                { 60394026, 1 }, // Ryzeal Plugin
                { 65681983, 1 }, // Crossout Designator
                { 6798031, 1 },  // Ryzeal Cross
                { 10045474, 3 }, // Infinite Impermanence
                { 31548215, 1 }, // Gravedigger's Trap Hole
                { 33787730, 1 }, // Ryzeal Plasma Hole
            };

        public static readonly Dictionary<int, int> Extra =
            new Dictionary<int, int>
            {
                { 93039339, 1 }, // Super Starslayer TY-PHON
                { 9940036, 1 },  // Mereologic Aggregator
                { 34909328, 3 }, // Ryzeal Detonator
                { 2061963, 1 },  // Number 104: Masquerade
                { 77675029, 1 }, // Exosister Karmael
                { 7511613, 2 },  // Ryzeal Duo Drive
                { 90091224, 1 }, // Dark Rebellion Xyz Dragon of the Four Heavenly Dragons
                { 45852939, 1 }, // Eclipse Twins
                { 1269512, 1 },  // Full Armored Utopic Ray Lancer
                { 6983839, 1 },  // Tornado Dragon
                { 46772449, 1 }, // Evilswarm Exciton Knight
                { 6511113, 1 },  // Traptrix Rafflesia
            };

        public static bool IsInMain(int cardId) { return Main.ContainsKey(cardId); }
        public static bool IsInExtra(int cardId) { return Extra.ContainsKey(cardId); }
    }
}
