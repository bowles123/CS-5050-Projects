using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    // This class represents a words edit distance.
    class EditDistance
    {
        // Data members.
        private int distance;
        private List<string> wordPairs;
        private int numWordPairs;

        // Default constructor.
        public EditDistance()
        {
            distance = -1;
            wordPairs = new List<string>();
            numWordPairs = 0;
        }

        // Explicit value constructor.
        public EditDistance(int dist, int pairs, string one, string two)
        {
            if (dist >= 0)
                distance = dist;
            else
                distance = 0;

            if (pairs > 0)
                numWordPairs = pairs;
            else
                numWordPairs = 0;

            // Create list of word pairs and add initial word pairs to it.
            wordPairs = new List<string>();
            wordPairs.Add(one);
            wordPairs.Add(two);
        }

        // Adds a word pair to the list of word pairs.
        public void addWordPair(string word1, string word2)
        {
            wordPairs.Add(word1);
            wordPairs.Add(word2);
        }

        // Returns the list of word pairs.
        public List<string> getwordPairs()
        {
            return wordPairs;
        }

        // Increment the number of word pairs.
        public void incrementNumWordPairs()
        {
            numWordPairs++;
        }

        // Return the number of word pairs.
        public int getNumWordPairs()
        {
            return numWordPairs;
        }

        // Set the value of the edit distance.
        public void setDistance(int dist)
        {
            if (dist >= 0)
                distance = dist;
            else
                distance = 0;
        }

        // Return the value of the edit distance.
        public int getDistance()
        {
            return distance;
        }

        // Prints out the list of word pairs.
        public void printWordPairs()
        {
            for (int i = 0; i < wordPairs.Count - 1; i+=2)
            {
                Console.WriteLine("{0} {1}", wordPairs[i], wordPairs[i + 1]);
            }
        }

        // Prints out the largest word pair.
        public string largestPair()
        {
            // Current longest length and current longest string.
            int currentLength, length = 0;
            string longest = "";

            // Traverse through the list of word pairs to find the longest one.
            for (int i = 0; i < wordPairs.Count - 1; i+=2)
            {
                currentLength = wordPairs[i].Length + wordPairs[i + 1].Length;

                if (currentLength > length)
                {
                    length = currentLength;
                    longest = String.Format(
                        "{0}, {1}", wordPairs[i], wordPairs[i + 1]);
                }
            }
            return longest;
        }
    }
}
