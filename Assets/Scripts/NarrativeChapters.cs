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
    // THE DEPARTURE OF BOROMIR
    // Source: "The Departure of Boromir" — The Return of the King, Ch. I
    // ─────────────────────────────────────────────────────────────────────
    public static readonly string[] Mordor = new[]
    {   
        //page 1
        "The Departure of Boromir\nThe Return of the King - Chapter I\n\n" +
        "Aragorn sped on up the hill. Every now and again he bent to the "+
        "ground. Hobbits go light, and their footprints are not easy even for "+
        "a Ranger to read, but not far from the top a spring crossed the path, "+
        "and in the wet earth he saw what he was seeking.",

        //page 2
        "‘I read the signs aright,’ he said to himself.\n\n ‘Frodo ran to the "+
        "hill-top. I wonder what he saw there? But he returned by the same "+
        "way, and went down the hill again.’ \n\n",

        //page 3
        "Aragorn hesitated. He desired to go to the high seat himself, "+
        "hoping to see there something that would guide him in his perplexities; but time was pressing. Suddenly he leaped forward, and "+
        "ran to the summit, across the great flag-stones, and up the steps. ",

        //page 4
        "Then sitting in the high seat he looked out. But the sun seemed "+
        "darkened, and the world dim and remote. He turned from the North "+
        "back again to North, and saw nothing save the distant hills, unless "+
        "it were that far away he could see again a great bird like an eagle "+
        "high in the air, descending slowly in wide circles down towards the "+
        "earth. ",
        
        //page 5
        "Even as he gazed his quick ears caught sounds in the woodlands "+
        "below, on the west side of the River. He stiffened. There were cries, "+
        "and among them, to his horror, he could distinguish the harsh voices "+
        "of Orcs. Then suddenly with a deep-throated call a great horn blew, "+
        "and the blasts of it smote the hills and echoed in the hollows, rising ",

        //page 6
        "in a mighty shout above the roaring of the falls. "+ 
        "‘The horn of Boromir!’ he cried. ‘He is in need!’ He sprang down "+
        "the steps and away, leaping down the path. ‘Alas! An ill fate is on "+
        "me this day, and all that I do goes amiss. Where is Sam?’\n\n ",
        
        //page 7
        "As he ran the cries came louder, but fainter now and desperately "+
        "the horn was blowing. Fierce and shrill rose the yells of the Orcs, "+
        "and suddenly the horn-calls ceased. Aragorn raced down the last ",

        //page 7
        "slope, but before he could reach the hill’s foot, the sounds died away; "+
        "and as he turned to the left and ran towards them they retreated, "+
        "until at last he could hear them no more. Drawing his bright sword "+
        "and crying Elendil! Elendil! he crashed through the trees. ",

        //page 8
        "A mile, maybe, from Parth Galen in a little glade not far from the "+
        "lake he found Boromir. He was sitting with his back to a great tree, "+
        "as if he was resting. But Aragorn saw that he was pierced with many "+
        "black-feathered arrows; his sword was still in his hand, but it was ",

        //page 9
        "broken near the hilt; his horn cloven in two was at his side. Many "+
        "Orcs lay slain, piled all about him and at his feet. "+
        "Aragorn knelt beside him. Boromir opened his eyes and strove to "+
        "speak. Atlastslow words came. ‘I tried to take the Ring from Frodo,’ he "+
        "said. ‘I am sorry. I have paid.’\n\n His glance strayed to his fallen enemies; ",

        //page 10
        "twenty at least lay there. ‘They have gone: the Halflings: the Orcs have "+
        "taken them. I think they are not dead. Orcs bound them.’\n\n He paused "+
        "and his eyes closed wearily. After a moment he spoke again. "+
        "‘Farewell, Aragorn! Go to Minas Tirith and save my people! I "+
        "have failed.’\n\n ",

        //page 11
        "‘No!’ said Aragorn, taking his hand and kissing his brow. "+
        "‘You have conquered. Few have gained such a victory. Be at peace! "+
        "Minas Tirith shall not fall!’ \n\n"+
        "Boromir smiled. "+
        "‘Which way did they go? Was Frodo there?’ said Aragorn.\n\n "+
        "But Boromir did not speak again. ",

        //page 12
        "‘Alas!’ said Aragorn. ‘Thus passes the heir of Denethor, Lord of "+
        "the Tower of Guard! This is a bitter end. Now the Company is all "+
        "in ruin. It is I that have failed. Vain was Gandalf’s trust in me. "+
        "What shall I do now? Boromir has laid it on me to go to Minas Tirith, "+
        "and my heart desires it; but where are the Ring and the Bearer? ",

        //page 13
        "How shall I find them and save the Quest from disaster?’ "+
        "He knelt for a while, bent with weeping, still clasping Boromir’s "+
        "hand. So it was that Legolas and Gimli found him. They came from "+
        "the western slopes of the hill, silently, creeping through the trees "+
        "as if they were hunting. Gimli had his axe in hand, and Legolas his ",

        //page 14
        "long knife: all his arrows were spent. When they came into the glade "+
        "they halted in amazement; and then they stood a moment with heads "+
        "bowed in grief, for it seemed to them plain what had happened. ",

        //page 15
        "‘Alas!’ said Legolas, coming to Aragorn’s side. ‘We have hunted "+
        "and slain many Orcs in the woods, but we should have been of more "+
        "use here. We came when we heard the horn – but too late, it seems. ",
        
        //page 16
        "I fear you have taken deadly hurt.’ "+
        "‘Boromir is dead,’ said Aragorn. ‘I am unscathed, for I was not "+
        "here with him. He fell defending the hobbits, while I was away upon "+
        "the hill.’ ",
        
        //page 17
        "‘The hobbits!’ cried Gimli. ‘Where are they then? Where is Frodo?’ \n"+
        "‘I do not know,’ answered Aragorn wearily. ‘Before he died "+
        "Boromir told me that the Orcs had bound them; he did not think "+
        "that they were dead. I sent him to follow Merry and Pippin; but I did "+
        "not ask him if Frodo or Sam were with him: not until it was too late. ",
        
        //page 18
        "All that I have done today has gone amiss. What is to be done now?’ "+
        "‘First we must tend the fallen,’ said Legolas. ‘We cannot leave "+
        "him lying like carrion among these foul Orcs.’ "+
        "‘But we must be swift,’ said Gimli. ‘He would not wish us to "+
        "linger. We must follow the Orcs, if there is hope that any of our "+
        "Company are living prisoners.’ ",
        
        //page 19
        "‘But we do not know whether the Ring-bearer is with them or "+
        "not,’ said Aragorn. ‘Are we to abandon him? Must we not seek him "+
        "first? An evil choice is now before us!’ "+
        "‘Then let us do first what we must do,’ said Legolas. ‘We have "+
        "not the time or the tools to bury our comrade fitly, or to raise a "+
        "mound over him. A cairn we might build.’ ",

        //page 20
        "‘The labour would be hard and long: there are no stones that we "+
        "could use nearer than the water-side,’ said Gimli. "+
        "‘Then let us lay him in a boat with his weapons, and the weapons "+
        "of his vanquished foes,’ said Aragorn.",
        
        //page 21
        "‘We will send him to the Falls "+
        "of Rauros and give him to Anduin. The River of Gondor will take "+
        "care at least that no evil creature dishonours his bones.’" ,    
        
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
