using NUnit.Framework;
using System.Collections.Generic;
using System.Xml;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.LightTransport;
using UnityEngine.Rendering;
using UnityEngine.UIElements.Experimental;
using UnityEngine.XR;
using UnityEngine.XR.ARSubsystems;
using static Unity.Burst.Intrinsics.X86;
using static UnityEngine.InputManagerEntry;

/// <summary>
/// Central store of chapter text for each environment scene.
/// Pages are split so each string fills one page of the VRBook.
/// </summary>
public static class NarrativeChapters
{
    // ─────────────────────────────────────────────────────────────────────
    // THE SHIRE
    // Source: "A Long-expected Party" — The Fellowship of the Ring, Ch. I
    // ─────────────────────────────────────────────────────────────────────
    public static readonly string[] Shire = new[]
    {
        "The Shire\n\"A Long-expected Party\"\nThe Fellowship of the Ring — Chapter I\n\n" +
        "In a hole in the ground there lived a hobbit. Not a nasty, dirty, wet hole " +
        "filled with the smell of worms, nor yet a bare, sandy hole — it was a " +
        "hobbit-hole, and that means comfort.",

        "The hobbit was Bilbo Baggins of Bag End, and on this particular morning — " +
        "the morning of his one hundred and eleventh birthday — the whole of the " +
        "Shire was in a flurry of excitement.",

        "Preparations had been underway for weeks. A great party was to be held that " +
        "evening, with fireworks by Gandalf the Grey — a name that sent a thrill " +
        "through the younger hobbits who still remembered his rockets and cascading " +
        "stars of old.",

        "Bilbo had invited practically the entire Shire, and the field beside Bag End " +
        "was filled with marquees and tables laden with more food than any one hobbit " +
        "could sensibly account for.",

        "After supper, Bilbo rose to speak. He thanked his guests, made a few " +
        "well-placed jokes, and then — with a wink and a smile — vanished.",

        "He slipped on his golden ring and walked unseen through the applauding crowd, " +
        "leaving Bag End behind for the last time.\n\n" +
        "The ring passed to Frodo. And with it, without anyone knowing it yet, " +
        "passed the fate of all Middle-earth."
    };

    // ─────────────────────────────────────────────────────────────────────
    // GOLLUM'S CAVE
    // Source: "Riddles in the Dark" — The Hobbit, Ch. V
    // ─────────────────────────────────────────────────────────────────────
    public static readonly string[] GollumCave = new[]
    {
        // Page 1 (reference size)
        "Gollum's Cave\n\"Riddles in the Dark\"\nThe Hobbit — Chapter V\n\n" +
        "What has roots as nobody sees,\n" +
        "Is taller than trees,\n" +
        "Up, up it goes,\n" +
        "And yet never grows?\n\n" +
        "\"Easy!\" said Bilbo. \"Mountain, I suppose.\"\n\n",

        // Page 2
        "\"Does it guess easy? It must have a competition with us, my\n" +
        "preciouss! If precious asks, and it doesn't answer, we eats it, my preciousss.\"\n\n",

        // Page 3
        "If it asks us, and we doesn't answer, then we does what it wants, eh? We\n" +
        "shows it the way out, yes!\"\n\n",

        // Page 4
        "All right! said Bilbo, not daring to disagree, and nearly bursting his\n" +
        "brain to think of riddles that could save him from being eaten.\n\n",

        // Page 5
        "Thirty white horses on a red hill,\n" +
        "First they champ,\n" +
        "Then they stamp,\n" +
        "Then they stand still.\n\n",

        // Page 6
        "That was all he could think of to ask — the idea of eating was rather on\n" +
        "his mind.\n\n",

        // Page 7
        "It was rather an old one, too, and Gollum knew the answer as well\n" +
        "as you do.\n\n",

        // Page 8
        "\"Chestnuts, chestnuts,\" he hissed. \"Teeth! teeth! my preciousss; but we\n" +
        "has only six!\"\n\n",

        // Page 9
        "Voiceless it cries,\n" +
        "Wingless flutters,\n" +
        "Toothless bites,\n" +
        "Mouthless mutters.\n\n",

        // Page 10
        "\"Half a moment!\" cried Bilbo, who was still thinking uncomfortably\n" +
        "about eating.\n\n",

        // Page 11
        "Fortunately he had once heard something rather like this before, and\n" +
        "getting his wits back he thought of the answer.\n\n",

        // Page 12
        "\"Wind, of course,\" he said, and he was so pleased that he made up one\n" +
        "on the spot.\n\n",

        // Page 13
        "An eye in a blue face\n" +
        "Saw an eye in a green face.\n\n",

        // Page 14
        "\"That eye is like to this eye\"\n" +
        "Said the first eye,\n" +
        "\"But in low place,\n" +
        "Not in high place\".\n\n",

        // Page 15
        "\"Ss, ss, ss,\" said Gollum.\n\n",

        // Page 16
        "He had been underground a long long time, and was forgetting this sort\n" +
        "of thing.\n\n",

        // Page 17
        "But just as Bilbo was beginning to hope that the wretch would not be\n" +
        "able to answer, Gollum brought up memories of ages and ages and ages before,\n\n",

        // Page 18
        "when he lived with his grandmother in a hole in a bank by a river, \"Sss,\n" +
        "sss, my preciouss,\" he said.\n\n",

        // Page 19
        "\"Sun on the daisies it means, it does.\"\n\n",

        // Page 20
        "But these ordinary aboveground everyday sort of riddles were tiring for\n" +
        "him.\n\n",

        // Page 21
        "Also they reminded him of days when he had been less lonely and\n" +
        "sneaky and nasty, and that put him out of temper.\n\n",

        // Page 22
        "What is more they made him hungry; so this time he tried something a bit\n" +
        "more difficult and more unpleasant:\n\n",

        // Page 23
        "It cannot be seen, cannot be felt,\n" +
        "Cannot be heard, cannot be smelt.\n\n",

        // Page 24
        "It lies behind stars and under hills,\n" +
        "And empty holes it fills.\n\n",

        // Page 25
        "It comes first and follows after,\n" +
        "Ends life, kills laughter."
    };

    // ─────────────────────────────────────────────────────────────────────
    // MORDOR
    // Source: "The Land of Shadow" — The Return of the King, Ch. II
    // ─────────────────────────────────────────────────────────────────────
    public static readonly string[] Mordor = new[]
    {
        "Mordor\n\"The Land of Shadow\"\nThe Return of the King — Chapter II\n\n" +
        "Frodo and Sam stood at the edge of the world they knew. Below them lay " +
        "the Plateau of Gorgoroth: a dead and ashen land, pocked with craters, " +
        "spiked with black rock, choking under the fumes of Orodruin.",

        "There was no colour here. Everything was grey, or black, or the sickly " +
        "orange of distant fire. Nothing grew. Nothing moved but dust and ash.",

        "The Ring was heavier with every step. Frodo could feel it pulling at " +
        "him — not just at his neck, but at his will, at the edges of who he was.",

        "The Eye was always searching. Sometimes it felt as though it had " +
        "nearly found him.",

        "Sam walked at his master's side, watching him carefully, carrying what " +
        "he could carry: food, rope, water, and whatever hope he had left.",

        "The mountain was always ahead of them. Mount Doom smoked and rumbled " +
        "in the distance, casting its red light over the waste.",

        "They did not speak of what would happen when they reached it. " +
        "They only walked.\n\n" +
        "One step, then another. That was enough.\n\n" +
        "The fate of the world had come to this: two hobbits, alone, " +
        "at the edge of the fire."
    };

    // ─────────────────────────────────────────────────────────────────────
    // Lookup by scene name (must match Build Settings)
    // ─────────────────────────────────────────────────────────────────────
    private static readonly Dictionary<string, string[]> ByScene = new Dictionary<string, string[]>
    {
        { "Shire",      Shire },
        { "gollumscenetest", GollumCave },
        { "Mordor",     Mordor },
    };

    /// <summary>
    /// Returns the pages for the given scene name, or null if none defined.
    /// </summary>
    public static string[] GetPages(string sceneName)
    {
        return ByScene.TryGetValue(sceneName, out var pages) ? pages : null;
    }
}
