using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace fiveletters
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = "C:\\Users\\HFGF\\Downloads\\alpha.txt";
            List<int> wordMasks = new List<int>();

            // Filtrering og maskering
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length == 5 && line.Distinct().Count() == 5)
                    {
                        wordMasks.Add(GetWordMask(line));
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
            int count = 0;
            int n = wordMasks.Count;
            totalCombinations = 0;

            // Brug af ConcurrentBag til at samle resultater
            var localCounts = new ConcurrentBag<int>();
            var localTotalCombinations = new ConcurrentBag<int>();

            Parallel.For(0, n, () => (localCount: 0, localTotalCombinations: 0), (i, loopState, local) =>
            {
                for (int j = i + 1; j < n; j++)
                {
                    if ((wordMasks[i] & wordMasks[j]) != 0) continue;
                    int ijMask = wordMasks[i] | wordMasks[j];
                    for (int k = j + 1; k < n; k++)
                    {
                        if ((ijMask & wordMasks[k]) != 0) continue;
                        int ijkMask = ijMask | wordMasks[k];
                        for (int l = k + 1; l < n; l++)
                        {
                            if ((ijkMask & wordMasks[l]) != 0) continue;
                            int ijklMask = ijkMask | wordMasks[l];
                            for (int m = l + 1; m < n; m++)
                            {
                                if ((ijklMask & wordMasks[m]) != 0) continue;

                                local.localTotalCombinations++;

                                int combinedMask = ijklMask | wordMasks[m];
                                if (combinedMask == (1 << 25) - 1)
                                {
                                    local.localCount++;
                                }
                            }
                        }
                    }
                }
                return local;
            },
            local =>
            {
                localCounts.Add(local.localCount);
                localTotalCombinations.Add(local.localTotalCombinations);
            });

            count = localCounts.Sum();
            totalCombinations = localTotalCombinations.Sum();

            return count;
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
