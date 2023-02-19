using System.Diagnostics;
using FastText.NetWrapper;


namespace SharpDomainValue;


public static class Program
{
    public static void Main()
    {
        //set parameters
        const int initialCapacity = 82765;
        const int maxEditDistance = 0;
        var symSpell = new SymSpell(initialCapacity, maxEditDistance);

        var path = AppDomain.CurrentDomain.BaseDirectory +
                   "frequency_dictionary_en_82_765.txt"; //path referencing the SymSpell core project
        //string path = "../../frequency_dictionary_en_82_765.txt";  //path when using symspell nuget package (frequency_dictionary_en_82_765.txt is included in nuget package)
        if (!symSpell.LoadDictionary(path, 0, 1))
        {
            Console.Error.WriteLine("\rFile not found: " + Path.GetFullPath(path));
        }

        var input = "dasistmeinkleinertest";
        Console.WriteLine(Correct(input, symSpell));

        Console.WriteLine(GetLanguage(input));

    }

    private static string Correct(string input, SymSpell symSpell)
    {
        //check if input term or similar terms within edit-distance are in dictionary, return results sorted by ascending edit distance, then by descending word frequency     
        return symSpell.WordSegmentation(input).correctedString;
    }

    private static string GetLanguage(string input)
    {
        // Stopwatch stopWatch = new Stopwatch();
        // stopWatch.Start();
        var fastText = new FastTextWrapper();
        fastText.LoadModel("data/lid.176.ftz");
        var detectedlang = fastText.PredictSingle(input);
        //remove first 9 characters
        return detectedlang.Label.Substring(9);
        // stopWatch.Stop();
        // TimeSpan ts = stopWatch.Elapsed;
        // Console.WriteLine(ts.TotalMilliseconds);
    }
}