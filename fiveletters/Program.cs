using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections;

namespace fiveletters
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = "C:\\Users\\HFGF\\Downloads\\alpha.txt";
            List<int> wordMasks = new List<int>();
            HashSet<string> uniqueWords = new HashSet<string>();

            // Filtrering og maskering
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length == 5 && line.Distinct().Count() == 5)
                    {
                        // Normalize the word by sorting the characters
                        string normalizedWord = new string(line.OrderBy(c => c).ToArray());

                        // Add the normalized word to the set if it's not already present
                        if (uniqueWords.Add(normalizedWord))
                        {
                            wordMasks.Add(GetWordMask(line));
                        }
                    }
                }
            }

            DateTime startTime = DateTime.Now;

            int result = FindValidCombinations(wordMasks, out int totalCombinations);

            Console.WriteLine($"Total combinations: {totalCombinations}");

            DateTime endTime = DateTime.Now;

            TimeSpan diff = (endTime - startTime).Duration();

            Console.WriteLine($"Tid taget: {diff.TotalSeconds} sekunder ({diff.TotalMinutes} minutter)");
        }

        static int FindValidCombinations(List<int> wordMasks, out int totalCombinations)
        {
            int n = wordMasks.Count;
            totalCombinations = 0;
            var localCounts = new System.Collections.Concurrent.ConcurrentBag<int>();
            var localTotalCombinations = new System.Collections.Concurrent.ConcurrentBag<int>();

            System.Threading.Tasks.Parallel.For(0, n, i =>
            {
                RecursiveSearch(wordMasks, i, 1, wordMasks[i], localCounts, localTotalCombinations);
            });

            int count = localCounts.Sum();
            totalCombinations = localTotalCombinations.Sum();

            return count;
        }

        static void RecursiveSearch(List<int> wordMasks, int index, int depth, int currentMask, System.Collections.Concurrent.ConcurrentBag<int> localCounts, System.Collections.Concurrent.ConcurrentBag<int> localTotalCombinations)
        {
            int n = wordMasks.Count;

            if (depth == 5) // We have reached the desired depth (5 words)
            {
                if (currentMask == (1 << 25) - 1)
                {
                    localCounts.Add(1);
                }
                localTotalCombinations.Add(1);
                return;
            }

            for (int i = index + 1; i < n; i++)
            {
                if ((currentMask & wordMasks[i]) == 0)
                {
                    RecursiveSearch(wordMasks, i, depth + 1, currentMask | wordMasks[i], localCounts, localTotalCombinations);
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
