using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fiveletters
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = "C:\\Users\\HFGF\\Desktop\\Programmering hf2\\fiveletters\\fiveletters\\beta.txt";
            List<string> words = new List<string>();

            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length != 5) continue;
                    if (line.Distinct().Count() != 5) continue;
                    words.Add(line);
                    Console.WriteLine(line); // til at debug ikke nødvendigt dog
                }
            }

            
            DateTime startTime = DateTime.Now;

            int result = FindValidCombinations(words);

            Console.WriteLine(result);

            DateTime endTime = DateTime.Now;

            // Beregn og print den forløbne tid i sekunder og minutter
            TimeSpan diff = (endTime - startTime).Duration();


            Console.WriteLine($"Tid taget: {diff} i alt");



        }

        static int FindValidCombinations(List<string> words)
        {
            var validCombinations = new List<List<string>>();
            int count = 0;
            int n = words.Count;
            int combResult = 0;

            // Iterer gennem alle mulige kombinationer af 5 ord
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if ((words[i] + words[j]).Distinct().Count() != 10) continue;
                    for (int k = j + 1; k < n; k++)
                    {
                        if ((words[i] + words[j] + words[k]).Distinct().Count() != 15) continue;
                        for (int l = k + 1; l < n; l++)
                        {
                            if ((words[i] + words[j] + words[k] + words[l]).Distinct().Count() != 20) continue;
                            for (int m = l + 1; m < n; m++)
                            {
                                //string comb = (words[i] + words[j] + words[k] + words[l] + words[m]);

                                combResult = (words[i] + words[j] + words[k] + words[l] + words[m]).Distinct().Count();

                                if (combResult == 25)
                                {
                                    count++;
                                }
                                //Console.WriteLine(comb);

                            }
                        }
                    }
                }
            }

            return combResult;
        }

        static bool IsUniqueLetterCombination(List<string> words)
        {
            var allLetters = string.Join("", words);
            return allLetters.Distinct().Count() == allLetters.Length;
        }
    }
}
