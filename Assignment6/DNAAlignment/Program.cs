using System;
using System.Collections.Generic;
using System.IO;

namespace DNAAlignment
{
    // Alignment structure for the divide and conquer algorithm.
    public struct Alignment
    {
        // Two strings
        public string Z;
        public string W;

        // Explicit-value constructor.
        public Alignment(string first, string second)
        {
            Z = first;
            W = second;
        }
    }

    class Program
    {
        // Cache for the solutions and the two sequences being compared.
        static string A, B;
        static string Z = "", W = "";
        static int[,] cache;

        // Dictionary to get the score from one letter to another.
        static Dictionary<char, Dictionary<char, int>> scores = new Dictionary<char, Dictionary<char, int>>
        {
            { 'A', new Dictionary<char, int> { { 'T', -1 }, { 'C', -1 }, { 'G', -2 }, { 'A', 5 }, { '-', -3 } } },
            { 'G', new Dictionary<char, int> { { 'T', -2 }, { 'C', -3 }, { 'G', 5 }, { 'A', -2 }, {'-', -2 } } },
            { 'C', new Dictionary<char, int> { { 'T', -2 }, { 'C', 5 }, { 'G', -3 }, { 'A', -1 }, { '-', -4 } } },
            { 'T', new Dictionary<char, int> { { 'T', 5 }, { 'C', -2 }, { 'G', -2 }, { 'A', -1 }, { '-', -1 } } },
            { '-', new Dictionary<char, int> { { 'A', -3 }, { 'G', -2 }, { 'C', -4 }, { 'T', -1 }, { '-', 0 } } }
        };

        // This function compares DNA sequences of different humans and apes.
        static void Main(string[] args)
        {
            A = "A";
            B = "CTAGAG";

            Console.WriteLine("Best Alignment (Dynamic): {0}", dynamicAlignment(A.Length, B.Length));
            Console.Write("Edits:\n{0}", writeEditSteps(A.Length, B.Length, "testEdits.txt"));
            Console.WriteLine("Best Alignment (Low-Memory Dynamic): {0}",  lowMemoryAlignment(A, B)[A.Length]);

            // Compare human DNA with Neanderthal DNA, other humans' DNA, and ape DNA.
            compareHuman_Neanderthal();
            compareChinese_Diverse();
            compareEthopian_Diverse();
            compareFrench_Diverse();
            compareIngman_Diverse();
            compareItalian_Diverse();
            compareJapanese_Diverse();
            compareRest_Diverse();
            compareHuman_Ape();
        }

        // This function returns the maximum of three integers.
        static int max(int one, int two, int three)
        {
            if (one >= two && one >= three)
                return one;
            else
                if (two >= one && two >= three)
                return two;
            else
                return three;
            
        }


        // This function is a dynamic programming algorithm for finding the best DNA alignment.
        static int dynamicAlignment(int n, int m)
        {
            // Set up cache, match, and dictionary.
            cache = new int[n + 1 , m + 1];
            int match, gapMatch, delMatch;
            Dictionary<char, int> dict, gapDict;

            // Add simple cases to cache.
            cache[0, 0] = 0;
            scores.TryGetValue('-', out gapDict);

            // Add simple cases for string B = 0 and inserting a gap.
            for (int i = 1; i <= n; i++)
            {
                scores.TryGetValue(A[i - 1], out dict);
                dict.TryGetValue('-', out match);
                cache[i, 0] = cache[i - 1, 0] + match;
            }

            // Add simpmle cases for string A = 0 and inserting B[j - 1].
            for (int j = 1; j <= m; j++)
            {
                scores.TryGetValue('-', out dict);
                dict.TryGetValue(B[j - 1], out match);
                cache[0, j] = cache [0, j - 1] + match;
            }

            // Fill in the rest of cache.
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    // Get dictionary for the given value in A and get the score to go from A[i - 1] to B[j - 1].
                    scores.TryGetValue(A[i - 1], out dict);
                    dict.TryGetValue(B[j - 1], out match);

                    // Get dictionary for the gap and get the score to go from gap to B[j - 1].
                    dict.TryGetValue('-', out gapMatch);
                    gapDict.TryGetValue(B[j - 1], out delMatch);

                    // Fill in space in cache.
                    cache[i, j] = max(cache[i - 1, j] + gapMatch, 
                        cache[i, j - 1] + delMatch, cache[i - 1, j - 1] + match);
                }
            }
            return cache[n, m];
        }

        static int[] lowMemoryAlignment(String n, String m) // Takes two strings instead?
        {
            // Set up cache, match, and dictionary.
            int [,] cache = new int[n.Length + 1, 2];
            int[] score = new int[n.Length + 1];
            int match, gapMatch, delMatch;
            Dictionary<char, int> dict, gapDict;

            // Add simple cases to cache.
            cache[0, 0] = 0;
            scores.TryGetValue('-', out gapDict);

            // Fill in initial 0th column of the 2-column array
            for (int i = 1; i <= n.Length; i++)
            {
                scores.TryGetValue(n[i - 1], out dict);
                dict.TryGetValue('-', out match);
                cache[i, 0] = cache[i - 1, 0] + match;
            }

            // Fill in the rest of cache.
            for (int j = 1; j <= m.Length; j++)
            {
                // Fill in simple case for row 0, column 1.
                scores.TryGetValue('-', out dict);
                dict.TryGetValue(m[j - 1], out match);
                cache[0, 1] = cache[0, 0] + match;

                for (int i = 1; i <= n.Length; i++)
                {
                    // Get dictionary for the given value in A and get the score to go from A[i - 1] to B[j - 1].
                    scores.TryGetValue(n[i - 1], out dict);
                    dict.TryGetValue(m[j - 1], out match);

                    // Get dictionary for the gap and get the score to go from gap to B[j - 1].
                    dict.TryGetValue('-', out gapMatch);
                    gapDict.TryGetValue(m[j - 1], out delMatch);

                    // Fill in space in cache.
                        cache[i, 1] = max(cache[i - 1, 1] + gapMatch,
                            cache[i, 0] + delMatch, cache[i - 1, 0] + match);
                }

                // If you're not on the last iteration.
                if (j < m.Length)
                {
                    // Disregard the 0th column in order to compute the next column and store it.
                    for (int a = 0; a <= n.Length; a++)
                    {
                        cache[a, 0] = cache[a, 1];
                        cache[a, 1] = 0;
                    }
                }
            }

            // Set up the array to return back to the calling function.
            for (int i = 0; i <= n.Length; i++)
            {
                score[i] = cache[i, 1];
            }

            return score;
        }

        // This function traces back through the cache to find the edits made.
        static string dynamicTraceback(int i, int j)
        {
            // Set up three possibilities.
            int subD, subG, subR;

            // Simplest cases.
            if ((i == 0 || j == 0))
            {
                if (i > 0)
                    return String.Format("  {0}->_\n{1}", A[i - 1], dynamicTraceback(i - 1, j)); // delete from A.
                if (j > 0)
                    return String.Format("  _->{0}\n{1}", B[j - 1], dynamicTraceback(i, j - 1)); // delete from B.
                return "";
            }

            // Match.
            if (A[i - 1] == B[j - 1])
            {
                return String.Format("  {0}->{1}\n{2}", 
                    A[i - 1], B[j - 1], dynamicTraceback(i - 1, j - 1));
            }

            // Get three different options from previously calculated portions in cache.
            subD = cache[i - 1, j];
            subG = cache[i, j - 1];
            subR = cache[i - 1, j - 1];

            // Find the maximum of the three different options to determine what edit was made.
            if (firstIsMax(subD, subG, subR))
            {
                // Delete from A, adding gap into B.
                return String.Format("  {0}->_\n{1}", A[i - 1], dynamicTraceback(i - 1, j));
            }
            if (firstIsMax(subG, subD, subR))
            {
                // Delete from B, adding gap into A.
                return String.Format("  _->{0}\n{1}", B[j - 1], dynamicTraceback(i, j - 1));
            }
            // Replace.
            return String.Format("  {0}->{1}\n{2}", 
                A[i - 1], B[j - 1], dynamicTraceback(i - 1, j - 1));
        }

        // This functions finds if the first parameter is the max of the three parameters.
        static bool firstIsMax(int one, int two, int three)
        {
            if (one >= two && one >= three) return true;
            return false;
        }

        // This function calls the traceback function.
        // It also reverses the order of the edits given from traceback because they're backwards.
        static string writeEditSteps(int i, int j, string study)
        {
            System.IO.StreamWriter outFile = new System.IO.StreamWriter(@study);
            string steps, header = String.Format("{0} -> {1}:\n", A, B);
            String[] steps_;

            // Find the steps taken to get the correct edit distance.
            steps = dynamicTraceback(i, j);
            steps_ = steps.Split('\n');
            steps = "";

            // Step through the string array to reverse the the strings, by words.
            for (int a = steps_.Length - 1; a >= 0; a--)
            {
                if (steps_[a] == "")
                    continue;
                steps += String.Format("{0}\n", steps_[a]);
            }
            outFile.WriteLine(header);
            outFile.WriteLine(steps);
            outFile.Close();
            return steps;
        }

        // This function is a divide and conquer algorithm for finding the best DNA alignment.
        static Alignment divide_conquerAlignment(string X, string Y)
        {
            int X_Mid, Y_Mid;
            int[] score_L, score_R;
            string X1 = "", X2 = "", Yreverse = "", Y1 = "", Y2 = "", trace;
            string[] split;
            Alignment alignment, one, two;

            // Simple solution.
            if (X.Length == 0)
            {
                // Gap for A.
                for (int i = 1; i <= Y.Length; i++)
                {
                    Z = Z + "-";
                    W = W + B[i - 1];
                }
                alignment = new Alignment(Z, W);
            }
            else
            {
                // Simple solution.
                if (Y.Length == 0)
                {
                    // Gap for B.
                    for (int i = 1; i <= X.Length; i++)
                    {
                        Z = Z + A[i - 1];
                        W = W + "-";
                    }
                    alignment = new Alignment(Z, W);
                }
                else
                {
                    if (X.Length == 1 || Y.Length == 1)
                    {
                        // Call the naive algorithm and it's traceback.
                        dynamicAlignment(X.Length, Y.Length);
                        trace = dynamicTraceback(X.Length, Y.Length);
                        split = trace.Split('\n');

                        // Pull out z and w from the traceback.
                        for(int i = 1; i < split.Length; i++)
                        {
                            Z += split[i][0];
                            W += split[i][3];
                        }

                        alignment = new Alignment(Z, W);
                    }
                    else
                    {
                        X_Mid = X.Length / 2;
                        
                        // Calculate first half of x.
                        for (int i = 0; i < X_Mid; i++)
                            X1 += A[i];

                        // Calculate second half of x.
                        for (int i = A.Length - 1; i >= X_Mid; i++)
                            X2 += A[i];

                        // Calculate the reverse of y.
                        for (int i = A.Length - 1; i >= 0; i--)
                            Yreverse += A[i];

                        // Get the left and the right arrays and add them together.
                        score_L = lowMemoryAlignment(X1, Y);
                        score_R = lowMemoryAlignment(X2, Yreverse);
                        Y_Mid = partition(score_L, score_R);

                        // Calculate first half of y.
                        for (int i = 0; i < Y_Mid; i++)
                            Y1 += B[i];

                        // Calculate second half of y.
                        for (int i = Y_Mid; i <= Y.Length; i++)
                            Y2 += B[i];

                        one = new Alignment();
                        two = new Alignment();

                        // Get the two halves.
                        one = divide_conquerAlignment(X1, Y1);
                        two = divide_conquerAlignment(X2, Y2);

                        // Combine the two z's and the two w's.
                        alignment = new Alignment(one.Z + two.Z, one.W + two.W);
                    }
                }
            }
            return alignment;
        }

        // This function adds two arrays and returns the position of the maximum element.
        static int partition(int[] left, int[] right)
        {
            int[] result = new int[left.Length];
            int maxPosition = 0;

            // Add the right and the left arrays together.
            for (int i = 0; i < left.Length; i++)
            {
                // Store the ith result.
                result[i] = left[i] + right[i];

                // Check if max has changed.
                if (result[i] > result[maxPosition])
                    maxPosition = i;
            }
            return maxPosition;
        }

        // This function runs the test comparing human and neanderthal DNA sequences.
        static void compareHuman_Neanderthal()
        {
            int[] alignment;

            readIn("homosapienMitochondria.txt", "neanderthalMitochondria.txt");
            alignment = lowMemoryAlignment(A, A);
            Console.WriteLine("The alignment score for prototypical human DNA compared to itself is: {0}",
                alignment[A.Length]);

            alignment = lowMemoryAlignment(B, B);
            Console.WriteLine("The alignment score for neanderthal DNA compared to itself is: {0}",
                alignment[A.Length]);

            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for prototypical human DNA" +
                " compared to neanderthal DNA is: {0}", alignment[A.Length]);
        }

        // This function compares Chinese DNA with the DNA of nine other human races.
        static void compareChinese_Diverse()
        {
            int[] alignment;

            // Compare Chinese DNA with Ethopian, French, and Ingman Australian DNA.
            readIn("Chinese.txt", "Ethopian.txt");
            alignment = lowMemoryAlignment(A, A);
            Console.WriteLine("The alignment score for Chinese DNA compared to itself is: {0}",
                alignment[A.Length]);

            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Chinese DNA compared to Ethopian DNA is: {0}",
                alignment[A.Length]);

            readIn("French.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Chinese DNA compared to French DNA is: {0}",
                alignment[A.Length]);

            readIn("Ingman_Australian.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Chinese DNA compared to Ingman Australian DNA is: {0}",
                alignment[A.Length]);

            // Compare Chinese DNA with Italian, Japanese, and Mishmar Caucasian DNA.
            readIn("Italian.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Chinese DNA compared to Italian DNA is: {0}",
                alignment[A.Length]);

            readIn("Japanese.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Chinese DNA compared to Japanese DNA is: {0}",
                alignment[A.Length]);

            readIn("Mishmar_Caucasian.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Chinese DNA compared to Mishmar Caucasian DNA is: {0}",
                alignment[A.Length]);

            // Compare Chinese DNA with Native American, Navajo, and Spanish DNA.
            readIn("Native_American.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Chinese DNA compared to Native American DNA is: {0}",
                alignment[A.Length]);

            readIn("Navajo.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Chinese DNA compared to Navajo DNA is: {0}",
                alignment[A.Length]);

            readIn("Spanish.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Chinese DNA compared to Spanish DNA is: {0}",
                alignment[A.Length]);
        }

        // This function compares Ethopian DNA with the DNA of eight other human races.
        static void compareEthopian_Diverse()
        {
            int[] alignment;

            // Compare Ethopian DNA with French, Ingman Australian, Italian, and Japanese DNA.
            readIn("Ethopian.txt", "French.txt");
            alignment = lowMemoryAlignment(A, A);
            Console.WriteLine("The alignment score for Ethopian DNA compared to itself is: {0}",
                alignment[A.Length]);

            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Ethopian DNA compared to French DNA is: {0}",
                alignment[A.Length]);

            readIn("Ingman_Australian.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Ethopian DNA compared to Ingman Australian DNA is: {0}",
                alignment[A.Length]);

            readIn("Italian.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Ethopian DNA compared to Italian DNA is: {0}",
                alignment[A.Length]);

            readIn("Japanese.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Ethopian DNA compared to Japanese DNA is: {0}",
                alignment[A.Length]);

            // Compare Ethopian DNA with Mishmar Caucasian, Native American, Navajo, and Spanish DNA.
            readIn("Mishmar_Caucasian.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Ethopian DNA compared to Mishmar Caucasian DNA is: {0}",
                alignment[A.Length]);

            readIn("Native_American.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Ethopian DNA compared to Native American DNA is: {0}",
                alignment[A.Length]);

            readIn("Navajo.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Ethopian DNA compared to Navajo DNA is: {0}",
                alignment[A.Length]);

            readIn("Spanish.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Ethopian DNA compared to Spanish DNA is: {0}",
                alignment[A.Length]);
        }

        // This function compares French DNA with the DNA of seven other human races.
        static void compareFrench_Diverse()
        {
            int[] alignment;

            // Compare French DNA with Ingman Australian, Italian, Japanese, and Mishmar Caucasian DNA.
            readIn("French.txt", "Ingman_Australian.txt");
            alignment = lowMemoryAlignment(A, A);
            Console.WriteLine("The alignment score for French DNA compared to itself is: {0}",
                alignment[A.Length]);

            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for French DNA compared to Ingman Australian DNA is: {0}",
                alignment[A.Length]);

            readIn("Italian.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for French DNA compared to Italian DNA is: {0}",
                alignment[A.Length]);

            readIn("Japanese.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for French DNA compared to Japanese DNA is: {0}",
                alignment[A.Length]);

            readIn("Mishmar_Caucasian.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for French DNA compared to Mishmar Caucasian DNA is: {0}",
                alignment[A.Length]);

            // Compare French DNA with Native American, Navajo, and Spanish DNA.
            readIn("Native_American.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for French DNA compared to Native American DNA is: {0}",
                alignment[A.Length]);

            readIn("Navajo.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for French DNA compared to Navajo DNA is: {0}",
                alignment[A.Length]);

            readIn("Spanish.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for French DNA compared to Spanish DNA is: {0}",
                alignment[A.Length]);
        }

        // This function compares Ingman Australian DNA with the DNA of six other human races.
        static void compareIngman_Diverse()
        {
            int[] alignment;

            // Compare Ingman Australian DNA with Italian, Japanese, and Mishmar Caucasian DNA.
            readIn("Ingman_Australian.txt", "Italian.txt");
            alignment = lowMemoryAlignment(A, A);
            Console.WriteLine("The alignment score for Ingman Australian DNA compared to itself is: {0}",
                alignment[A.Length]);

            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Ingman Australian DNA compared to Italian DNA is: {0}",
                alignment[A.Length]);

            readIn("Japanese.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Ingman Australian DNA compared to Japanese DNA is: {0}",
                alignment[A.Length]);

            readIn("Mishmar_Caucasian.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Ingman Australian DNA" +
                " compared to Mishmar Caucasian DNA is: {0}", alignment[A.Length]);

            // Compare Ingman Australian DNA with Native American, Navajo, and Spanish DNA.
            readIn("Native_American.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Ingman Australian DNA compared to Native American DNA is: {0}",
                alignment[A.Length]);

            readIn("Navajo.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Ingman Australian DNA compared to Navajo DNA is: {0}",
                alignment[A.Length]);

            readIn("Spanish.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Ingman Australian DNA compared to Spanish DNA is: {0}",
                alignment[A.Length]);
        }

        // This function compares Italian DNA with the DNA of five other human races.
        static void compareItalian_Diverse()
        {
            int[] alignment;

            // Compare Italian DNA with Japanese, Mishmar Caucasian, and Native American DNA.
            readIn("Italian.txt", "Japanese.txt");
            alignment = lowMemoryAlignment(A, A);
            Console.WriteLine("The alignment score for Italian DNA compared to itself is: {0}",
                alignment[A.Length]);

            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Italian DNA compared to Japanese DNA is: {0}",
                alignment[A.Length]);

            readIn("Mishmar_Caucasian.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Italian DNA compared to Mishmar Caucasian DNA is: {0}",
                alignment[A.Length]);

            readIn("Native_American.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Italian DNA compared to Native American DNA is: {0}",
                alignment[A.Length]);

            // Compare Italian DNA with Navajo DNA and Spanish DNA.
            readIn("Navajo.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Italian DNA compared to Navajo DNA is: {0}",
                alignment[A.Length]);

            readIn("Spanish.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Italian DNA compared to Spanish DNA is: {0}",
                alignment[A.Length]);
        }

        // This function compares Japanese DNA with the DNA of four other human races.
        static void compareJapanese_Diverse()
        {
            int[] alignment;

            // Compare Japanese DNA with Mishmar Caucasian, Native American, Navajo, and Spanish DNA.
            readIn("Japanese.txt", "Mishmar_Caucasian.txt");
            alignment = lowMemoryAlignment(A, A);
            Console.WriteLine("The alignment score for Japanese DNA compared to itself is: {0}",
                alignment[A.Length]);

            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Japanese DNA compared to Mishmar Caucasian DNA is: {0}",
                alignment[A.Length]);

            readIn("Native_American.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Japanese DNA compared to Native American DNA is: {0}",
                alignment[A.Length]);

            readIn("Navajo.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Japanese DNA compared to Navajo DNA is: {0}",
                alignment[A.Length]);

            readIn("Spanish.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Japanses DNA compared to Spanish DNA is: {0}",
                alignment[A.Length]);
        }

        // This function compares Mishmar Caucasian DNA with the DNA of three other human races,
        // Native American DNA with the DNA of two other human races,
        // and Navajo DNA with Spanish DNA.
        static void compareRest_Diverse()
        {
            int[] alignment;

            // Compare Mishmar Caucasian DNA to Native American, Navajo, and Spanish DNA.
            readIn("Mishmar_Caucasian.txt", "Native_American.txt");
            alignment = lowMemoryAlignment(A, A);
            Console.WriteLine("The alignment score for Mishmar Caucasian DNA compared to itself is: {0}",
                alignment[A.Length]);

            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Mishmar Caucasian DNA compared to Native American DNA is: {0}",
                alignment[A.Length]);

            readIn("Navajo.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Mishmar Caucasian DNA compared to Navajo DNA is: {0}",
                alignment[A.Length]);

            readIn("Spanish.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Mishmar Caucasian DNA compared to Spanish DNA is: {0}",
                alignment[A.Length]);

            // Compare Native American DNA with Navajo DNA and Spanish DNA.
            readIn("Native_American.txt", "Navajo.txt");
            alignment = lowMemoryAlignment(A, A);
            Console.WriteLine("The alignment score for Native American DNA compared to itself is: {0}",
                alignment[A.Length]);

            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Native American DNA compared to Navajo DNA is: {0}",
                alignment[A.Length]);

            readIn("Spanish.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Native American DNA compared to Spanish DNA is: {0}",
                alignment[A.Length]);

            // Compare Navajo DNA with Spanish DNA.
            readIn("Navajo.txt", "Spanish.txt");
            alignment = lowMemoryAlignment(A, A);
            Console.WriteLine("The alignment score for Navajo DNA compared to itself is: {0}",
                alignment[A.Length]);

            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for Navajo DNA compared to Spanish DNA is: {0}",
                alignment[A.Length]);

            alignment = lowMemoryAlignment(B, B);
            Console.WriteLine("The alignment score for Spanish DNA compared to itself is: {0}",
                alignment[A.Length]);
        }

        static void compareHuman_Ape()
        {
            int[] alignment;

            // Compare human DNA with baboon DNA.
            readIn("homosapienMitochondria.txt", "baboon.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for prototypical human DNA compared to baboon DNA is: {0}", 
                alignment[A.Length]);

            // Compare human DNA with bonobo DNA.
            readIn("bonobo.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for prototypical human DNA compared bonobo DNA is: {0}", 
                alignment[A.Length]);

            // Compare human DNA with one type of chimpanzee DNA.
            readIn("chimpanzee8.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for prototypical human DNA" +
               " compmared to one type of chimpanzee DNA is: {0}", alignment[A.Length]);

            // Compare human DNA with the other type of chimpanzee DNA.
            readIn("chimpanzee9.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for prototypical human DNA" +
                " compared to the other type of chimpanzee DNA is: {0}", alignment[A.Length]);

            // Compare human DNA with gorilla DNA.
            readIn("gorilla.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for prototypical human DNA compared to gorilla DNA is: {0}", 
                alignment[A.Length]);

            // Compare human DNA with western lowland gorilla DNA.
            readIn("westernLowlandGorilla.txt");
            alignment = lowMemoryAlignment(A, B);
            Console.WriteLine("The alignment score for prototypical human DNA" + 
                " compared western lowland gorilla DNA is: {0}", alignment[A.Length]);
        }

        // This function reads the DNA sequence in from the two files.
        static void readIn(string file1, string file2 = "")
        {
            if (file2 != "")
            {
                // Read from the two files containing the DNA sequences.
                String[] lines1 = File.ReadAllLines(file1);
                String[] lines2 = File.ReadAllLines(file2);

                A = "";
                B = "";

                // Set the first DNA sequence to the string 'A'.
                for (int i = 0; i < lines1.Length; i++)
                {
                    B += lines1[i];
                }

                // Set the second DNA sequence to the string 'B'.
                for (int i = 0; i < lines2.Length; i++)
                {
                    A += lines2[i];
                }
            }
            else
            {
                String[] lines1 = File.ReadAllLines(file1);

                B = "";

                for (int i = 0; i < lines1.Length; i++)
                {
                    B += lines1[i];
                }
            }
        }
    }
}
