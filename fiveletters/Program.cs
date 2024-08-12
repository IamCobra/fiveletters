using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics; 

namespace fiveletters
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = "C:\\Users\\HFGF\\Desktop\\Programmering hf2\\fiveletters\\fiveletters\\alpha.txt";
            List<int> wordMasks = new List<int>();
            HashSet<string> uniqueWords = new HashSet<string>(); // hashset tillader ikke duplikerede elementer. bruges til at sikre kun unikke ord bliver tilføjet

            // Filtrering og maskering
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length == 5 && line.Distinct().Count() == 5)
                    {
                        //  Normalisere bogstavet ved at sortere karakterne alfabetisk
                        string normalizedWord = new string(line.OrderBy(c => c).ToArray());

                        //  tilføjer det normaliserede ord til selve settet hvis den ikke allerede er der
                        if (uniqueWords.Add(normalizedWord)) // fjerner anagrammer
                        {
                            wordMasks.Add(GetWordMask(line));
                        }
                    }
                }
            }

            Console.WriteLine("Amount of words: " + wordMasks.Count);


            Stopwatch stopwatch = Stopwatch.StartNew();

            int totalCombinations = FindValidCombinations(wordMasks);

            stopwatch.Stop();

            Console.WriteLine($"Total combinations processed: {totalCombinations}");
            Console.WriteLine($"Tid taget: {stopwatch.Elapsed.TotalSeconds} sekunder");
        }

        static int FindValidCombinations(List<int> wordMasks)
        {
            int n = wordMasks.Count();
            int totalCombinations = 0;

            Parallel.For(0, n, i =>
            {
                RecursiveSearch(wordMasks, i, 1, wordMasks[i], ref totalCombinations);
            });

            return totalCombinations;
        }

        // depth er hvor mange rekursiv kald der egentlig er lavet og hvor dybt den søger. 
        // currentMask En bitmaske, der repræsenterer den kombination af bogstaver, der er blevet samlet fra de valgte ord indtil videre

        static void RecursiveSearch(List<int> wordMasks, int index, int depth, int currentMask, ref int totalCombinations)
        {
            int n = wordMasks.Count(); // gemmer antal bit den finder i n

            if (depth == 5)
            {
                Interlocked.Increment(ref totalCombinations); // brug concurrentbag istedet?
                return;
            }

            for (int i = index + 1; i < n; i++)
            {
                if ((currentMask & wordMasks[i]) == 0) // tjekker efter om værdien er 0 og fortsætter kun når den har fundet et nyt unikt ord 
                {
                    RecursiveSearch(wordMasks, i, depth + 1, currentMask | wordMasks[i], ref totalCombinations);
                }
            }
        }

        static int GetWordMask(string word)
        {
            int mask = 0;
            foreach (char c in word)
            {
                mask |= (1 << (c - 'a'));
            }
            return mask;
        }
    }
}