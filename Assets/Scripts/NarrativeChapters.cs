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
