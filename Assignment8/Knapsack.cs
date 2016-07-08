using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Assignment8
{
    class Knapsack
    {
        // Static class variables.
        static double[] sizes;
        static List<double> allErrors, allRuntimes;
        static long[] intSizes;
        static int numDigits;
        static Dictionary<int, long>[] cache;

        static void Main(string[] args)
        {
            allRuntimes = new List<double>();
            allErrors = new List<double>();

            // Run study 30 times and write the solutions and times out to a file.
            for (int i = 1; i < 31; i++)
            {
                numDigits = 12;
                runStudy();
            }
            writeToFile("Data");
        }

        ///<summary>
        /// Runs study for the tradeoff of time and accuracy with the real-value knapsack problem.
        ///</summary>
        static void runStudy()
        {
            // Set up the number of objects and the size of the knapsack.
            int objects = 2000;
            double[] errors = new double[12], runtimes = new double[12];
            double correctSol, x, knapsack = 1000.0000, multBy = Math.Pow(10, numDigits);
            long sol, intKnapsack = Convert.ToInt64(knapsack * multBy);
            cache = new Dictionary<int, long>[(int)knapsack + 1];
            Stopwatch watch;

            // Initialize the array of sizes
            // find the minimum remaining space after adding all possible objects.
            sizes = new double[objects];
            initializeSizes();
            convertSizesToInt();
            for (int i = 0; i < cache.Length; i++)
                cache[i] = new Dictionary<int, long>();

            // Time how long it took with precision being 12.
            watch = Stopwatch.StartNew();
            sol = realValueKnapsack(objects, intKnapsack);
            watch.Stop();
            correctSol = calculateCorrectSolution(sol);

            for (numDigits = 3; numDigits < 12; numDigits++)
            {
                intKnapsack = Convert.ToInt64(knapsack * (Math.Pow(10, numDigits)));
                convertSizesToInt();
                cache = new Dictionary<int, long>[(int)knapsack + 1];
                for (int i = 0; i < cache.Length; i++)
                    cache[i] = new Dictionary<int, long>();

                // Time how long it took.
                watch = Stopwatch.StartNew();
                sol = realValueKnapsack(objects, intKnapsack);
                watch.Stop();
                x = calculateCorrectSolution(sol);
                runtimes[numDigits] = (watch.Elapsed.TotalMilliseconds * 1000000); // Nano-seconds.
                errors[numDigits] = (correctSol - x) * (correctSol - x);
            }

            for (int i = 3; i < runtimes.Length; i++)
            {
                allRuntimes.Add(runtimes[i]);
                allErrors.Add(errors[i]);
            }
        }

        /// <summary>
        /// Assigns random values, between 0 and 1, to the sizes array.
        /// </summary>
        static void initializeSizes()
        {
            // Variables.
            Random rand = new Random();
            double num;

            // Assign random value to each element in the array.
            for (int i = 0; i < sizes.Length; i++)
            {
                num = (rand.NextDouble() * 1.0);
                sizes[i] = num;
            }
        }

        /// <summary>
        /// Calculates the "correct" solution for the space remaining
        /// </summary>
        static double calculateCorrectSolution(long solution)
        {
            double realSolution;
            realSolution = solution / (Math.Pow(10, numDigits));
            return realSolution;
        }

        /// <summary>
        /// Converts the real sizes, double, to an integer equivalent.
        /// </summary>
        static void convertSizesToInt()
        {
            double multBy = Math.Pow(10, numDigits);
            intSizes = new long[sizes.Length];

            for (int i = 0; i < sizes.Length; i++)
                intSizes[i] = Convert.ToInt64(sizes[i] * multBy);
        }

        /// <summary>
        /// Writes the runtimes and remaining space solutions to a csv file.
        /// </summary>
        static void writeToFile(string filename)
        {
            System.IO.StreamWriter outFile = new System.IO.StreamWriter(filename + ".csv");

            for (int i = 3; i < allRuntimes.Count; i++)
                outFile.WriteLine(String.Format("{0},{1}", allRuntimes[i], allErrors[i]));
            outFile.Close();
        }

        /// <summary>
        /// Calculates the minimum space remaining in a real value knapsack after all the objects
        /// that fit have been put in it.
        /// </summary>
        static long realValueKnapsack(int n, long k)
        {
            // Variable.
            long space;
            int realK = Convert.ToInt32(k / (Math.Pow(10, numDigits)));

            // Simple cases.
            if (k < 0) return 100000000;
            if (n == 0) return Convert.ToInt64(k);
            if (k == 0.0) return 0;
            if (cache[realK].TryGetValue(n, out space)) return space;

            // Store solution in cache and return that solution.
            cache[realK][n] =  min(realValueKnapsack(n - 1, k - intSizes[n - 1]), 
                realValueKnapsack(n - 1, k));
            return cache[realK][n];
        }

        /// <summary>
        /// Calculates the minimum of two "long" numbers.
        /// </summary>
        static long min(long one, long two)
        {
            if (one < two) return one;
            return two;
        }
    }
}
