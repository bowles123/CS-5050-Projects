using System;
using System.Collections.Generic;
using System.IO;
using ConsoleApplication1;
using System.Diagnostics;

namespace MEDAssignment
{
    class MinimalEdit
    {
        // Global variable for the two strings being edited
        static string A, B;
        static char APrev = 'a';
        static double time = 0.0;
        static int[,] cache;

        static void Main(string[] args)
        {
            // Test all words with dynamic programming technique.
            dynamic();
            Console.WriteLine("It took {0: 0.00##}ms to find the minimal edit distance for each word pair.",
                time);
        }

        // Reads commonly misspelled words from the word file.
        static void readFromFile(ref List<string> wordList)
        {
            // Set up array of words and read all lines of the file.
            string[] words;
            String[] lines = File.ReadAllLines(
                "words.txt");

            // Separate each word from the complete text of the file.
            for (int i = 0; i < lines.Length; i++)
            {
                words = lines[i].Split('|');

                // If there is more than one word to compare it to, 
                // then add the first word for every word it needs to be compared to.
                for (int j = 1; j < words.Length; j++)
                {
                    wordList.Add(words[0]);
                    wordList.Add(words[j]);
                }
            }
        }

        // This function writes edit distances out to the edits text file.
        static void write(ref List<EditDistance> list)
        {
            // Set up the stream for writing to the file.
            System.IO.StreamWriter outFile =
            new System.IO.StreamWriter(
                @"edits.txt");
            string line, one, two, three;

            // Set up first line of text and write it out.
            one = "Edit Distance";
            two = "Number of Word Pairs";
            three = "Longest Word Pair";
            line = String.Format("{0, -15} {1, -25} {2, 20}", one, two, three);
            Console.WriteLine(line);
            outFile.WriteLine(line);

            // Set up second line of text and write it out.
            line = "-----------------------------------------------------------------------------------";
            Console.WriteLine(line);
            outFile.WriteLine(line);

            // For each unique edit distance, output that distance and the number of word pairs to the file.
            for (int i = 0; i < list.Count; i++)
            {
                line = String.Format("{0, -15} {1, -25} {2, -20}", 
                    list[i].getDistance(), list[i].getNumWordPairs(), list[i].largestPair());
                Console.WriteLine(line);

                outFile.WriteLine(line);
            }
            outFile.Close();
        }

        // Finds the edit distance of each word pair in the file using a dynamic programming approach.
        static void dynamic()
        {
            // Set up variables.
            List<EditDistance> edits = new List<EditDistance>();
            EditDistance edit;
            int distance;
            List<string> words = new List<string>();
            bool found = false;
            Stopwatch watch;

            // Read the words from the file.
            readFromFile(ref words);

            while(words.Count > 0)
            {
                // Remove word pair from the list of words.
                A = words[0];
                words.Remove(A);
                B = words[0];
                words.Remove(B);

                // Calculate the edit distance of the removed words.
                watch = Stopwatch.StartNew();
                distance = DPMED(A.Length, B.Length);
                watch.Stop();

                time += watch.Elapsed.TotalMilliseconds;

                for (int j = 0; j < edits.Count; j++)
                {
                    // If edit distance already exists, update it.
                    if (distance == edits[j].getDistance())
                    {
                        edits[j].addWordPair(A, B);
                        edits[j].incrementNumWordPairs();
                        found = true;
                        break;
                    }
                    else
                    {
                        found = false;
                    }
                }

                // Create new edit distance object if it wasn't found in the list of edits.
                if(!found)
                {
                    edit = new EditDistance(distance, 1, A, B);
                    edits.Add(edit);
                }
            }

            // Write edit distances out to the edits file.
            write(ref edits);
        }

        // This function writes the edit steps out to the console.
        static void writeEditSteps(int i, int j)
        {
            string steps;
            String[] steps_;

            // Find the steps taken to get the correct edit distance.
            steps = traceback(i, j);
            steps_ = steps.Split('\n');
            steps = "";

            // Step through the string array to reverse the the strings, by words.
            for (int a = steps_.Length - 2; a >= 0; a--)
            {
                steps += String.Format("{0}\n", steps_[a]);
            }

            // If it's gone through all the letters of the current alphabet letter, 
            // then read a key in from the user.
            if (char.ToLower(APrev) != char.ToLower(A[0]))
            {
                Console.WriteLine("Press any key to move  on to the next letter in the alphabet...");
                Console.ReadKey();
            }

            // Output the word, edit steps, and edit distance.
            Console.WriteLine("{0} -> {1}", A, B);
            Console.Write("Edit Steps: \n{0}", steps);
            Console.WriteLine("Edit Distance: {0}", cache[i, j]);
            Console.WriteLine();
            APrev = A[0];
        }

        static string traceback(int i, int j)
        {
            // Variables.
            int subD, subI, subS;

            // If there are no edits return the empty string.
            if (i == 0 || j == 0)
            {
                return "";
            }

            // If the characters are the same, then there's no need to make a move. 
            if (A[i - 1] == B[j - 1])
            {
                return "" + traceback(i - 1, j - 1);
            }

            // Set up values for delete, insert, and substitute.
            subD = cache[i - 1, j] + 1;
            subI = cache[i, j - 1] + 1;
            subS = cache[i - 1, j - 1];

            // Traceback recursively and return a string with the edit steps taken.
            if (cache[i , j] == subD)
            {
                return String.Format("  delete {0}\n{1}", A[i - 1], traceback(i - 1, j));
            }
            if (cache[i, j] == subI)
            {
                return String.Format("  insert {0}\n{1}", B[j - 1], traceback(i, j - 1));
            }
            return String.Format("  replace {0} with {1}\n{2}", A[i - 1], B[j - 1], traceback(i - 1, j - 1));
        }

        // Finds the minimum of three given values.
        static int min(int one, int two, int three)
        {
            if (one <= two && one <= three)
                return one;
            else
                if (two <= one && two <= three)
                return two;
            return three;
        }

        // Finds the minimum edit distance recursively.
        static int recursiveMED(int i, int j)
        {
            int extra;

            // Simple cases.
            if (i == 0 && j == 0)
                return 0;
            if (i == 0)
                return j;
            if (j == 0)
                return i;

            // Find the extra to add to the recursive call.
            if (A[i  - 1] == B[j - 1])
                extra = 0;
            else
                extra = 1;

            return min(recursiveMED(i - 1, j) + 1, recursiveMED(i, j - 1) + 1, recursiveMED(i -1, j - 1) + extra);
        }

        // Finds the minimum edit distance using a dynamic programming approach.
        static int DPMED(int i, int j)
        {
            // Setup cache and extra variables.
            cache = new int[i + 1, j + 1];
            int extra;

            for (int a = 0; a <= i; a++)
            {
                for (int b = 0; b <= j; b++)
                {
                    // Simple case initialization.
                    if (a == 0)
                        cache[a, b] = b;
                    else
                        if (b == 0)
                        cache[a, b] = a;
                    else
                    {
                        // Find the extra to add to the previous cache spot.
                        if (A[a - 1] == B[b - 1])
                            extra = 0;
                        else
                            extra = 1;

                        cache[a, b] = min(cache[a - 1, b] + 1, cache[a, b - 1] + 1, cache[a - 1, b - 1] + extra);
                    }
                    
                }
            }

            writeEditSteps(i, j);
            return cache[i, j];
        }
    }
}


