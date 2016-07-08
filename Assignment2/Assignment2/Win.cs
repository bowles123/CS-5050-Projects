using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    class Win
    {
        static void Main(string[] args)
        {
            // Initialize heaps sizes.
            int a = 0, b = 0, c = 0;

            // Run the 10,000 tests with all three algorithms.
            Console.Write("Running 1st test...");
            for (int i = 0; i < 10000; i++)
                randomGenerator(a, b, c, i);
            Console.WriteLine("Success!");

            // Run the 10,000 tests with only the memoizing and dynamic programming algorithms.
            Console.Write("Running 2nd test...");
            for (int i = 0; i < 10000; i++)
                randGenerator(a, b, c, i);
            Console.WriteLine("Success!");
        }

        // Generates smaller random numbers for test 1 and runs the test.
        static void randomGenerator(int a, int b, int c, int i)
        {
            // Initialize all variables.
            Random rand = new Random();
            a = rand.Next(1, 10);
            b = rand.Next(1, 5);
            c = rand.Next(1, 7);
            bool[,,] sol = new bool[a + 1, b + 1, c + 1];
            bool[,,] done = new bool[a + 1, b + 1, c + 1];
            bool recursive, memoized, dynamicProgramming;

            // Test each algorithm witht the same three random numbers.
            recursive = recursiveWin(a, b, c);
            memoized = memoizedWin(a, b, c, ref sol, ref done);
            dynamicProgramming = dpWin(a, b, c);

            // If the three algorithms don't return the same value then exit the program, there's a bug.
            if (recursive != dynamicProgramming || recursive != memoized || memoized != dynamicProgramming)
            {
                Console.WriteLine("Bug in code on iteration " + i + ".");
                Console.WriteLine("A = " + a + " B = " + b + " C = " + c + ".");
                Console.WriteLine("Recursive: " + recursive + ", Memoized: " + memoized + ", DP: " + dynamicProgramming + ".");
                System.Environment.Exit(1);
            }
        }

        // Generates larger random numbers for test 2 and runs the test.
        static void randGenerator(int a, int b, int c, int i)
        {
            // Initialize variables.
            Random rand = new Random();
            a = rand.Next(1, 25);
            b = rand.Next(1, 20);
            c = rand.Next(1, 25);
            bool[,,] sol = new bool[a + 1, b + 1, c + 1];
            bool[,,] done = new bool[a + 1, b + 1, c + 1];
            bool memoized, dynamicProgramming;

            // Test the memoizing and dynamic programming algorithms with same three random numbers.
            memoized = memoizedWin(a, b, c, ref sol, ref done);
            dynamicProgramming = dpWin(a, b, c);

            // If the two algorithms don't return the same value exit the program, there's a bug.
            if (memoized != dynamicProgramming)
            {
                Console.WriteLine("Bug in code on iteration " + i + ".");
                Console.WriteLine("A = " + a + " B = " + b + " C = " + c + ".");
                Console.WriteLine("Memoized: " + memoized + ", DP: " + dynamicProgramming + ".");
                System.Environment.Exit(1);
            }
        }

        // This function checks to see if there's a forced win based on the values in the three heaps.
        // Done recursively.
        static bool recursiveWin(int A, int B, int C)
        {
            // If there are no more stones remaining you have lost.
            if (A == 0 && B == 0 && C == 0)
                return false;

            // Call win recursively with each possible move your opponent coule make.
            // If your opponent doesn't have a forced win, then you do.
            for (int i = 1; i <= A; i++)
                if (!(recursiveWin(A - i, B, C)))
                    return true;

            for (int i = 1; i <= B; i++)
                if (!(recursiveWin(A, B - i, C)))
                    return true;

            for (int i = 1; i <= C; i++)
                if (!(recursiveWin(A, B, C - i)))
                    return true;
            return false;
        }

        // Same function as the recursiveWin, except that the algorithm uses memoizing with cache.
        static bool memoizedWin(int A, int B, int C, ref bool [,,] sol, ref bool [,,] done)
        {
            if (A == 0 && B == 0 && C == 0)
                return false;

            // If the solution is already in the cache return that solution.
            if (done[A, B, C])
                return sol[A, B, C];
            else
            {
                // Calculate whether there's a forced win with each possible move and return true if there isn't
                // Store the value of the solution in cache and update the done array.
                for (int i = 1; i <= A; i++)
                {
                    sol[A, B, C] = !(memoizedWin(A - i, B, C, ref sol, ref done));
                    done[A, B, C] = true;
                    if (sol[A, B, C])
                        return sol[A, B, C];
                }

                for (int i = 1; i <= B; i++)
                {
                    sol[A, B, C] = !(memoizedWin(A, B - i, C, ref sol, ref done));
                    done[A, B, C] = true;
                    if (sol[A, B, C])
                        return sol[A, B, C];
                }

                for (int i = 1; i <= C; i++)
                {
                    sol[A, B, C] = !(memoizedWin(A, B, C - i, ref sol, ref done));
                    done[A, B, C] = true;
                    if (sol[A, B, C])
                        return sol[A, B, C];
                }
                return false;
            }
        }

        // Same function as the recursiveWin, excpet the dynamic programming approach is taken.
        static bool dpWin(int A, int B, int C)
        {
            // Intialize state of function.
            bool done = false;
            bool[,,] sol = new bool[A + 1, B + 1, C + 1];

            // Initialize simplest case into cache.
            if (A == 0 && B == 0 && C == 0)
            {
                sol[0, 0, 0] = false;
                return false;
            }

            // Initialize the rest of the cache with all other solutions.
            // Return solution after all of cache is initialized.
            for (int i = 0; i <= A; i++)
                for (int j = 0; j <= B; j++)
                    for (int k = 0; k <= C; k++)
                    {
                        if (i == 0 && j == 0 && k == 0)
                            continue;

                        // Rewrite recursion replacing returns with assignment into cache and recursive calls with cache access.
                        for (int a = 1; a <= i && !done; a++)
                        {
                            if (!(sol[i - a, j, k]))
                            {
                                sol[i, j, k] = true;
                                done = true;
                            }
                        }

                        for (int b = 1; b <= j && !done; b++)
                        {
                            if (!(sol[i, j - b, k]))
                            {
                                sol[i, j, k] = true;
                                done = true;
                            }
                        }

                        for (int c = 1; c <= k && !done; c++)
                        {
                            if (!(sol[i, j, k - c]))
                            {
                                sol[i, j, k] = true;
                                done = true;
                            }
                        }
                        if (!done)
                            sol[i, j, k] = false;
                        done = false;
                    }
            return sol[A, B, C];
        }
    }
}
