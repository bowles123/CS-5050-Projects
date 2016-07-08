using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PolynomialMultiplication
{
    class Program
    {
        static int size = 2;

        static void Main(string[] args)
        {
            // Variables.
            double[] one, two;
            string result = "", result1, result2;

            // Allocate memory for the two polynomials.
            one = new double[2] { 3.0, 4.0 };
            two = new double[2] { 2.0, -7.0 };

            // Compute the polynomial multiplication, using "High School Algorithm", and output the solution.
            result = toString(simplePolyMult(one, two));
            result1 = toString(one);
            result2 = toString(two);

            Console.WriteLine("Class Example Test:");
            Console.WriteLine("\t({0}) * ({1}), (\"High School Algorithm\") is: {2}", result1, result2, result);

            // Compute the polynomial multiplication, using "Divide & Conquer Algorithm", and output the solution.
            result = toString(divide_conquerPolyMult(one, two));
            Console.WriteLine("\t({0}) * ({1}), (\"Divide & Conquer\") is: {2}", result1, result2, result);
            result = "";

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Smaller random examples:");
            randomExamples();

            Console.WriteLine("Emperical Study:");
            empericalStudy();
        }

        // This function shows three random examples of a small length.
        static void randomExamples()
        {
            string result, result1, result2;
            double[] one = new double[size], two = new double[size];

            for (int i = 1; i < 10; i *= 2)
            {
                setPolys(ref one, ref two, i);

                // Compute the polynomial multiplication, using "High School Algorithm", and output the solution.
                result = toString(simplePolyMult(one, two));
                result1 = toString(one);
                result2 = toString(two);

                Console.WriteLine("\t({0}) * ({1}), (\"High School Algorithm\") is: {2}", result1, result2, result);

                // Compute the polynomial multiplication, using "Divide & Conquer Algorithm", and output the solution.
                result = toString(divide_conquerPolyMult(one, two));
                Console.WriteLine("\t({0}) * ({1}), (\"Divide & Conquer\") is: {2}", result1, result2, result);
                result = "";
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        // This function performs an emperical study on the two different algorithms.
        static void empericalStudy()
        {
            double[] one, two;
            Stopwatch highSchoolWatch, divide_conquerWatch;
            double hsaMilliseconds = 0.0, dcMilliseconds = 0.0;
            List<double> dcTimes = new List<double>(), hsaTimes = new List<double>();

            for (size = 32; size <= 1000000; size *= 2)
            {
                // Allocate memory for the two polynomials.
                one = new double[size];
                two = new double[size];

                // Set polynomials and set up watches.
                setPolynomials(ref one, ref two);
                highSchoolWatch = Stopwatch.StartNew();
                divide_conquerWatch = Stopwatch.StartNew();

                // Time how long it takes to run the polynomial multiplication using the "High School" Algortihm.
                highSchoolWatch.Restart();
                simplePolyMult(one, two);
                highSchoolWatch.Stop();
                hsaMilliseconds = highSchoolWatch.Elapsed.TotalMilliseconds;

                Console.WriteLine("\t\"High School Algorithm\" with polynomials of length {0} ran in {1: 0.00###}s.",
                size, hsaMilliseconds / 1000);
                hsaTimes.Add(hsaMilliseconds / 1000);

                // Time how long it takes to run the polynomial multiplication using the Divide & Conquer Algortihm.
                divide_conquerWatch.Restart();
                divide_conquerPolyMult(one, two);
                divide_conquerWatch.Stop();
                dcMilliseconds = divide_conquerWatch.Elapsed.TotalMilliseconds;

                Console.WriteLine("\tDivide & Conquer Algorithm with polynomials of length {0} ran in {1: 0.00###}s.",
                size, dcMilliseconds / 1000);
                Console.WriteLine();
                dcTimes.Add(dcMilliseconds / 1000);
            }

            writeToFile(hsaTimes, dcTimes);
        }

        // This function sets the polynomials up with random values between -1.0 & 1.0.
        static void setPolynomials(ref double[] P, ref double[] Q)
        {
            // Set up random generator and numbers.
            Random rand = new Random();
            double number1, number2;

            for (int i = 0; i < P.Length; i++)
            {
                // Generate two random numbers.
                number1 = (rand.NextDouble() * 2.0) - 1.0;
                number2 = (rand.NextDouble() * 2.0) - 1.0;

                // Assign random numbers to the ith element of the two arrays.
                P[i] = number1;
                Q[i] = number2;
            }
        }

        // This function sets up a test polynomial
        static void setPolys(ref double[] P, ref double[] Q, int sz)
        {
            // Variables.
            double num1, num2;
            Random random = new Random();

            // Allocate memory for the two polynomials and set the global size to the passed in value for sz.
            P = new double[sz];
            Q = new double[sz];
            size = sz;

            // Set the ith element of each polynomial to a randomly generated number between 0.0 & 10.0.
            for (int i = 0; i < sz; i++)
            {
                // Generate random numbers.
                num1 = (random.NextDouble() * 9.0) + 1;
                num2 = (random.NextDouble() * 9.0) + 1;

                // Assign the ith element of P & Q to the num1 & num2 respectively.
                P[i] = Math.Round(num1, 0);
                Q[i] = Math.Round(num2, 0);
            }
        }

        // Convert the array of the result into a string.
        static string toString(double[] result)
        {
            string stringResult = "";

            for (int i = 0; i < result.Length - 1; i++)
            {
                // If current element is zero, then do not add it to the string.
                if (result[i] == 0.0) continue;

                // Constant value.
                if (i == 0)
                    stringResult += result[i] + " + ";
                else
                {
                    // Power of 1.
                    if (i == 1)
                        stringResult += result[i] + "x" + " + ";
                    else
                        // Power greater than 1.
                        stringResult += result[i] + "x^" + i + " + ";
                }
            }

            // Concatenate last element of the array onto the string and return it.
            if (result.Length == 1)
                stringResult += result[0];
            else
                if (result.Length == 2)
                    stringResult += result[1] + "x";
                else
                    stringResult += result[result.Length - 1] + "x^" + (result.Length - 1);

            return stringResult;
        }

        // This function writes the times for the two algorithms out to a file.
        static void writeToFile(List<double> HSATimes, List<double> DCTimes)
        {
            // Set up stream and initial lines.
            System.IO.StreamWriter outFile = new System.IO.StreamWriter(@"times.txt");
            string line = "\"High School Algorithm\": ", line2 = "Divde & Conquer Algorithm:";
            outFile.WriteLine(line);

            // Write out all the times for the "High School Algorithm".
            for (int i = 0, j = 32; i < HSATimes.Count; i++, j *= 2)
            {
                line = String.Format("\t{0} {1}", j, HSATimes[i]);
                outFile.WriteLine(line);
            }

            // Write out all the times for the Divide & Conquer Algorithm.
            outFile.WriteLine(line2);
            for (int i = 0, j = 32; i < DCTimes.Count; i++, j *= 2)
            {
                line = String.Format("\t{0} {1}", j, DCTimes[i]);
                outFile.WriteLine(line);
            }

            outFile.Close();
        }

        // This function computes polynomial multiplication using the "high school algorithm" approach.
        static double[] simplePolyMult(double[] P, double[] Q)
        {
            // Result array.
            double[] PQ = new double[(2 * size) - 1];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    // Calculate the ith element of the result array.
                    PQ[i + j] += P[i] * Q[j];

            return PQ;
        }

        // This function computes polynomial multiplication using the divide & conquer approach.
        static double[] divide_conquerPolyMult(double[] P, double[] Q)
        {
            // Variables.
            double[] PL, PH, QL, QH, sol1, sol2, sol3, midSol, one, two;
            double[] PQ = new double[(2 * P.Length) - 1];
            int j = 0, mid = P.Length / 2;
            
            // Simplest solution.
            if (P.Length == 1)
            {
                PQ[0] = P[0] * Q[0];
                return PQ;
            }

            // Allocate the low and high arrays for P & Q.
            PL = new double[mid];
            PH = new double[mid];
            QL = new double[mid];
            QH = new double[mid];

            // Calculate the low arrays.
            for (int i = 0; i < mid; i++)
            {
                PL[i] = P[i];
                QL[i] = Q[i];
            }

            // Calculate the high arrays.
            for (int i = mid; i < P.Length; i++, j++)
            {
                PH[j] = P[i];
                QH[j] = Q[i];
            }

            // Calculate the three portions of the result.
            sol1 = divide_conquerPolyMult(PL, QL);
            sol2 = divide_conquerPolyMult(PH, QH);

            one = sum(PL, PH);
            two = sum(QL, QH);

            // Compute sub solution three.
            sol3 = divide_conquerPolyMult(one, two);

            // Stop watch for helper function call, then restart it.
            midSol = computeMidSol(sol1, sol2, sol3);

            // Combine the three portions of the result together to get the result and return it.
            // Stop watch for helper function call, then restart it.
            combineSolutions(sol1, midSol, sol2, ref PQ);
            return PQ; 
        }

        // This function adds two arrays together returning an array.
        static double[] sum(double[] one, double[] two)
        {
            // Result array.
            double[] result = new double[one.Length];

            for (int i = 0; i < one.Length; i++)
                // Calculate ith element of the result.
                result[i] = one[i] + two[i];
            return result;
        }

        // This function calculates the middle portion of the solution polynomial.
        static double[] computeMidSol(double[] one, double[] two, double[] three)
        {
            // Result array.
            double[] result = new double[one.Length];

            for (int i = 0; i < one.Length; i++)
                // Calculate the ith element of the result.
                result[i] = three[i] - one[i] - two[i];
            return result;
        }

        // This function combines the three portions of the solution into the complete solution.
        static void combineSolutions(double[] beg, double[] mid, double[] end, ref double[] solution)
        {
            // Variables.
            int j = 0, k = 0, l = 0;

            // Add first half of the first sub solution to the current solution.
            for (int i = 0; i <= beg.Length / 2 ; i++, j++)
                solution[j] = beg[i];

            // Add last half of the first sub soltuion to the first half of the second sub solution
            // Add that to the current solution.
            for (int i = j; i < beg.Length; i++, j++, k++)
                solution[j] = beg[i] + mid[k];

            // Add the middle element of the second solution to the current solution.
            solution[j] = mid[k];
            j++;
            k++;

            // Add first half of the third sub solution to the second half the the second sub soltuion.
            // Add that to the current solution.
            for (int i = 0; k < mid.Length; i++, j++, k++, l++)
                solution[j] = end[i] + mid[k];

            // Add the last half of the third subsolution to the current solution.
            for (int i = l; i < end.Length; i++, j++)
                solution[j] = end[i];
        }
    }
}
