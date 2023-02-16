//Source: https://github.com/wolfgarbe/SymSpell/issues/15
//create a word frequency dictionary

using System.Text;

void CreateWordFrequencyDictionary()
{
    var termlist = new Dictionary<string, long>();

    //spelling dictionary
    //http://app.aspell.net/create?defaults=en_GB-large
    //http://wordlist.aspell.net/ 
    var hs = new HashSet<string>();
    using (var sr = new StreamReader(@"scowl2.txt"))
    {
        //process a single line at a time only for memory efficiency
        while (sr.ReadLine() is { } line)
        {
            if (line.Length < 1) continue;
            if (char.IsUpper(line.Last())) continue; //do not allow abbreviations
            if (line.Length <= 2 && char.IsUpper(line.First())) continue;
            hs.Add(line.ToLower());
        }
    }


    //### process google ngrams
    //http://storage.googleapis.com/books/ngrams/books/datasetsv2.html

    for (var i = 0; i < 10; i++)
    {
        using var sr = new StreamReader(@"googlebooks-eng-1M-1gram-20090715-" + i + ".csv");
        //process a single line at a time only for memory efficiency
        while (sr.ReadLine() is { } line)
        {
            var lineParts = line.Split('\t');
            if (lineParts.Length < 3) continue;
            var key = lineParts[0].ToLower();

            //allow only terms from the google n-grams which are also in the SCOWL lis
            if (!hs.Contains(key)) continue;

            //allow only terms which start with a letter
            if (!char.IsLetter(key.First())) continue;


            //only a & i are genuine single letter english words
            if (key.Length == 1 && key != "a" && key != "i") continue;

            //addition filters
            if (key.EndsWith(".")) continue;
            if (key.Length == 2 && (key.StartsWith("'") || key.EndsWith("'"))) continue;

            switch (key)
            {
                case "ha":
                case "te":
                case "sp":
                case "th":
                case "ca":
                case "yu":
                case "ms":
                case "ins":
                case "ith":
                case "spp":
                case "hou":
                case "ewith":
                case "fori":
                    continue;
            }


            //set word counts
            if (!long.TryParse(lineParts[2], out var count)) continue;
            //add to dictionary
            if (termlist.ContainsKey(key))
                termlist[key] += count;
            else
                termlist[key] = count;
            //Console.WriteLine(key+" "+count.ToString("N0"));
        }
    }

    //add some additional terms
    foreach (var key in new string[15]
             {
                 "can't", "won't", "don't", "couldn't", "shouldn't", "wouldn't", "needn't", "mustn't", "she'll",
                 "we'll", "he'll", "they'll", "i'll", "i'm", "wasn't"
             }) termlist[key] = 300000;

    //sort by frequency
    var termlist2 = termlist.ToList();
    termlist2.Sort((x, y) => y.Value.CompareTo(x.Value));

    //limit size
    if (termlist2.Count > 500000) termlist2.RemoveRange(500000, termlist2.Count - 500000);

    //write new dict to file
    using (var file = new StreamWriter(@"frequency_dictionary_en.txt", false, Encoding.UTF8))
    {
        foreach (var t in termlist2)
            file.WriteLine(t.Key + " " + t.Value);
    }

    Console.WriteLine("ready: " + termlist.Count.ToString("N0") + " terms");
}