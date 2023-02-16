namespace SharpDomainValue;

internal class Program
{
    private static void Main()
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
            return;
        }

        var input = "thisismylittletest";
        Console.WriteLine(Correct(input, symSpell));
    }

    private static string Correct(string input, SymSpell symSpell)
    {
        //check if input term or similar terms within edit-distance are in dictionary, return results sorted by ascending edit distance, then by descending word frequency     
        return symSpell.WordSegmentation(input).correctedString;
        
    }
}