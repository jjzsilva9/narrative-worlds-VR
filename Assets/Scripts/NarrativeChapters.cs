using System.Collections.Generic;

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
        "When Mr. Bilbo Baggins of Bag End announced that he would shortly be celebrating " +
        "his eleventy-first birthday with a party of special magnificence, there was much " +
        "talk and excitement in Hobbiton.",

        "Bilbo was very rich and very peculiar, and had been the wonder of the Shire for " +
        "sixty years, ever since his remarkable disappearance and unexpected return.",

        "The riches he had brought back from his travels had now become a local legend, " +
        "and it was popularly believed, whatever the old folk might say, that the Hill at " +
        "Bag End was full of tunnels stuffed with treasure. And if that was not enough " +
        "for fame, there was also his prolonged vigour to marvel at.",

        "Time wore on, but it seemed to have little effect on Mr. Baggins. At ninety he " +
        "was much the same as at fifty. At ninety-nine they began to call him " +
        "well-preserved; but unchanged would have been nearer the mark.",

        "There were some that shook their heads and thought this was too much of a good " +
        "thing; it seemed unfair that anyone should possess (apparently) perpetual youth " +
        "as well as (reputedly) inexhaustible wealth.",

        "'It will have to be paid for,' they said. 'It isn't natural, and trouble will come of it!'",

        "But so far trouble had not come; and as Mr. Baggins was generous with his money, " +
        "most people were willing to forgive him his oddities and his good fortune.",

        "He remained on visiting terms with his relatives (except, of course, the " +
        "Sackville-Bagginses), and he had many devoted admirers among the hobbits of " +
        "poor and unimportant families.",

        "But he had no close friends, until some of his younger cousins began to grow up.\n\n" +
        "The eldest of these, and Bilbo's favourite, was young Frodo Baggins."
    };

    // ─────────────────────────────────────────────────────────────────────
    // GOLLUM'S CAVE
    // Source: "Riddles in the Dark" — The Hobbit, Ch. V
    // ─────────────────────────────────────────────────────────────────────
    public static readonly string[] GollumCave = new[]
    {
        "Gollum's Cave\n\"Riddles in the Dark\"\nThe Hobbit — Chapter V\n\n" +
        "Bilbo Baggins had lost his companions. He lay in the dark — cold stone " +
        "beneath him, cold black above — with no idea which way was out.",

        "When he reached out his hand, his fingers closed on something small and " +
        "round and metal, half-buried in the cave floor.",

        "He pocketed it without a second thought. He would think of it again, " +
        "many times, for the rest of his long life.",

        "There came a sound from the underground lake — a faint, wet paddling. " +
        "Then: two pale, round eyes gleaming in the dark.",

        "Gollum had lived beneath the Misty Mountains longer than he could " +
        "remember, catching blind fish and creeping through tunnels, always " +
        "talking to his Precious. He was very interested in Bilbo. Not in a " +
        "friendly way.",

        "They played a game of riddles, as was the custom, with Bilbo's life " +
        "as the stakes. Back and forth the riddles went — from simple things " +
        "of teeth and wind to darker, deeper puzzles that neither fully understood.",

        "In the end, flustered and desperate, Bilbo asked:\n\n" +
        "\"What have I got in my pocket?\"\n\n" +
        "It was not a proper riddle. But it saved his life — and set in motion " +
        "everything that was to come."
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
        { "GollumCave", GollumCave },
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
