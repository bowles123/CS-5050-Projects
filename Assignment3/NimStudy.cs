using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Assignment1
{
    class NimStudy
    {
        static void Main(string[] args)
        {
            // Create stopwatches to time the algorithms.
            Stopwatch recursiveWatch, memoizedWatch, dynamicWatch;


            // Run recursive algorthim with different sizes and time it.
            Console.WriteLine("Testing recursive algorithm...");
            for (int i = 2; i < 5; i++)
            {
                if (i == 2)
                {
                    recursiveWatch = Stopwatch.StartNew();

                    for (int j = 0; j <= 1000; j++)
                    {
                        if (j == 0)
                        {
                            recursiveWatch.Restart();
                        }
                        else
                        {
                            recursiveWatch.Start();
                            recursiveWin(i, i, i);
                            recursiveWatch.Stop();
                        }
                    }

                    Console.WriteLine("Recursive algorithm with 2 stones ran in: {0: 0.00##}s.",
                        (recursiveWatch.Elapsed.TotalMilliseconds / 1000));
                }
                else
                {
                    recursiveWatch = Stopwatch.StartNew();
                    recursiveWin(i, i, i);
                    recursiveWatch.Stop();
                    Console.WriteLine("Recursive algorithm with {0} stones ran in: {1: 0.00##}s.",
                        i, recursiveWatch.Elapsed.TotalMilliseconds);
                }
            }

            // Run memoized and dynamic progamming algorithm with different sizes and time them.
            Console.WriteLine("Testing memoized and dynamic programming algorthims...");
            for (int i = 2; i <= 256; i *= 2)
            {
                bool[,,] sol = new bool[i + 1, i + 1, i + 1];
                bool[,,] done = new bool[i + 1, i + 1, i + 1];

                if (i == 2)
                {
                    memoizedWatch = Stopwatch.StartNew();

                    for (int j = 0; j <= 1000; j++)
                    {
                        if (j == 0)
                        {
                            memoizedWatch.Restart();
                        }
                        else
                        {
                            memoizedWatch.Start();
                            memoizedWin(i, i, i, ref sol, ref done);
                            memoizedWatch.Stop();
                        }
                    }

                    Console.WriteLine("Memoized algorithm with 2 stones ran in: {0: 0.00##}ms.",
                        (memoizedWatch.Elapsed.TotalMilliseconds / 1000));

                    dynamicWatch = Stopwatch.StartNew();

                    for (int j = 0; j <= 5000; j++)
                    {
                        if (j == 0)
                        {
                            dynamicWatch.Restart();
                        }
                        else
                        {
                            dynamicWatch.Start();
                            dpWin(i, i, i);
                            dynamicWatch.Stop();
                        }
                    }

                    Console.WriteLine("Dynamic programming algorithm with 2 stones ran in: {0: 0.00##}ms.",
                        (dynamicWatch.Elapsed.TotalMilliseconds / 5000));
                }
                else
                {
                    memoizedWatch = Stopwatch.StartNew();
                    memoizedWin(i, i, i, ref sol, ref done);
                    memoizedWatch.Stop();
                    Console.WriteLine("Memoized prgramming algorithm with {0} stones ran in: {1: 0.00##}ms.",
                        i, memoizedWatch.Elapsed.TotalMilliseconds);

                    dynamicWatch = Stopwatch.StartNew();
                    dpWin(i, i, i);
                    dynamicWatch.Stop();
                    Console.WriteLine("Dynamic programming algorithm with {0} stones ran in: {1: 0.00##}ms.",
                        i, dynamicWatch.Elapsed.TotalMilliseconds);
                }
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
        static bool memoizedWin(int A, int B, int C, ref bool[,,] sol, ref bool[,,] done)
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
