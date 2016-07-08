using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace PolynomialMultiplication
{
    class Program
    {
        static int size = 4;

        static void Main(string[] args)
        {
            // Variables.
            double[] one, two;
            string result = "", result1, result2;
            double time = 0.0;

            // Allocate memory for the two polynomials.
            one = new double[4] { 0.0, 1.0, 2.0, 3.0 };
            two = new double[4] { 10.0, 11.0, 12.0, 13.0 };

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

            // Compute the polynomial multiplication, using "Fast Fourier Transform", and output the solution.
            result = toString(FFT_PolyMult(one, two, ref time));
            Console.WriteLine("\t({0}) * ({1}), (\"Fast Fourier Transform\") is: {2}", result1, result2, result);
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
            double time = 0.0;

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

                result = toString(FFT_PolyMult(one, two, ref time));
                Console.WriteLine("\t({0}) * ({1}), (\"Fast Fourier Transform\") is: {2}", result1, result2, result);
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
            double hsaMilliseconds = 0.0, dcMilliseconds = 0.0, fftMilliseconds = 0.0;
            List<double> dcTimes = new List<double>(), hsaTimes = new List<double>();
            List<double> fftTimes = new List<double>();

            for (size = 32; size < 5000000; size *= 2)
            {
                // Allocate memory for the two polynomials.
                one = new double[size];
                two = new double[size];

                // Set polynomials and set up watches.
                setPolynomials(ref one, ref two);
                highSchoolWatch = Stopwatch.StartNew();
                divide_conquerWatch = Stopwatch.StartNew();
                

                if (hsaMilliseconds < 600000)
                {
                    // Time how long it takes to run the polynomial multiplication using the "High School" Algortihm.
                    highSchoolWatch.Restart();
                    simplePolyMult(one, two);
                    highSchoolWatch.Stop();
                    hsaMilliseconds = highSchoolWatch.Elapsed.TotalMilliseconds;

                    Console.WriteLine("\t\"High School Algorithm\" with polynomials of length {0} ran in {1: 0.00###}s.",
                        size, hsaMilliseconds / 1000);
                    hsaTimes.Add(hsaMilliseconds / 1000);
                }

                if (dcMilliseconds < 600000)
                {
                    // Time how long it takes to run the polynomial multiplication using the Divide & Conquer Algortihm.
                    divide_conquerWatch.Restart();
                    divide_conquerPolyMult(one, two);
                    divide_conquerWatch.Stop();
                    dcMilliseconds = divide_conquerWatch.Elapsed.TotalMilliseconds;

                    Console.WriteLine("\tDivide & Conquer Algorithm with polynomials of length {0} ran in {1: 0.00###}s.",
                        size, dcMilliseconds / 1000);
                    dcTimes.Add(dcMilliseconds / 1000);
                }

                // Time how long it takes to run the polynomial multiplication using the Divide & Conquer Algortihm.
                // Output the time it took to run the algorithm and add the time to the fftTimes list.
                FFT_PolyMult(one, two, ref fftMilliseconds);
                Console.WriteLine("\tFast Fourier Transform with polynomials of length {0} ran in {1: 0.00###}s.",
                    size, fftMilliseconds / 1000);
                Console.WriteLine();
                fftTimes.Add(fftMilliseconds / 1000);
                writeToFile(hsaTimes, dcTimes, fftTimes);
            }
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
        static void writeToFile(List<double> HSATimes, List<double> DCTimes, List<double> FFT_Times)
        {
            // Set up stream and initial lines.
            System.IO.StreamWriter outFile = new System.IO.StreamWriter(@"realTimes.txt");
            string line = "\"High School Algorithm\": ", line2 = "Divde & Conquer Algorithm:";
            string line3 = "Fast Fourier Tranform Algorithm:";
            outFile.WriteLine(line);

            // Write out all the times for the "High School Algorithm".
            for (int i = 0, j = 32; i < HSATimes.Count; i++, j *= 2)
            {
                line = String.Format("\t{0}, {1}", j, HSATimes[i]);
                outFile.WriteLine(line);
            }

            // Write out all the times for the Divide & Conquer Algorithm.
            outFile.WriteLine(line2);
            for (int i = 0, j = 32; i < DCTimes.Count; i++, j *= 2)
            {
                line = String.Format("\t{0}, {1}", j, DCTimes[i]);
                outFile.WriteLine(line);
            }

            // Write out all the times for the Fast Fourier Transform Algorithm.
            outFile.WriteLine(line3);
            for (int i = 0, j = 32; i < FFT_Times.Count; i++, j *= 2)
            {
                line = String.Format("\t{0}, {1}", j, FFT_Times[i]);
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

        // This function multiplies two functions together using the FFT approach.
        static double[] FFT_PolyMult(double[] P, double[] Q, ref double time)
        {
            // Set up stopwatch for timing
            Stopwatch fftWatch;

            // Set up temporary arrays used by FFT.
            Complex[] p = new Complex[size * 2];
            Complex[] q = new Complex[size * 2];
            Complex[] Omega = new Complex[size * 2];
            Complex[] PQ = new Complex[size * 2];
            Complex[] PQ2 = new Complex[size * 2];
            double[] pq = new double[(size * 2) - 1];
            setOmega(ref Omega, Omega.Length);

            // Convert P and Q to arrays of complex numbers.
            for (int i = 0; i < size * 2; i++)
            {
                if (i < size)
                {
                    p[i] = new Complex(P[i], 0);
                    q[i] = new Complex(Q[i], 0);
                }
                else
                {
                    p[i] = new Complex(0, 0);
                    q[i] = new Complex(0, 0);
                }
            }

            // Begin timing the algorithm.
            fftWatch = Stopwatch.StartNew();

            // Evaluate P and Q at n values using FFT.
            Complex[] tempP = FFT(p, Omega, size * 2);
            Complex[] tempQ = FFT(q, Omega, size * 2); 

            // Calculate PQ from tempP and tempQ and set up the Omega array for interpolation.
            for (int i = 0; i < PQ.Length; i++)
            {
                PQ[i] = multiply(tempP[i], tempQ[i]);
            }

            // Interpolate PQ to get the coefficients back and convert back to real numbers before returning.
            fftWatch.Stop();
            computeConjugate(ref Omega);
            fftWatch.Start();
            PQ2 = FFT(PQ, Omega, PQ.Length);
            for (int i = 0; i < pq.Length; i++)
                pq[i] = PQ2[i].Real / (size * 2);

            fftWatch.Stop();
            time = fftWatch.Elapsed.TotalMilliseconds;
            return pq;
        }

        // This function sets up the omega array used by FFT
        static void setOmega(ref Complex[] Omega, int size_)
        {
            double twoPI_over_size = (Math.PI * 2) / size_;

            for (int j = 0; j < size_; j++)
                Omega[j] = new Complex(Math.Cos(twoPI_over_size * j), Math.Sin(twoPI_over_size * j));
        }

        // This function computes the conjugates of the omega array
        static void computeConjugate(ref Complex[] omega)
        {
            double real, imaginary;

            for (int i = 0; i < omega.Length; i++)
            {
                real = omega[i].Real;
                imaginary = -omega[i].Imaginary;
                omega[i] = new Complex(real, imaginary);
            }
        }

        // This function is the main brains of the FFT algorithm.
        static Complex[] FFT(Complex[] P, Complex[] Omega, int n)
        {
            // Set up complex arrays for recursive calls and computing the final solution.
            Complex[] pOdd = new Complex[n / 2];
            Complex[] pEven = new Complex[n / 2];
            Complex[] newOmega = new Complex[n / 2];
            Complex[] evenSol, oddSol, solution = new Complex[n];

            // if the size of the polynomial is one evaluate it at omega and return the solution.
            if (n == 1)
            {
                solution[0] = P[0];
                return solution;
            }

            // Seperate the P array and Omega array into even and odd.
            for (int i = 0, j = 0; i < n; i += 2, j++)
            {
                pEven[j] = P[i];
                newOmega[j] = Omega[i];
            }

            for (int i = 1, j = 0; i < n; i += 2, j++)
            {
                pOdd[j] = P[i];
            }

            // Compute even and odd sub solutions.
            evenSol = FFT(pEven, newOmega, n / 2);
            oddSol = FFT(pOdd, newOmega, n / 2);

            // Compute solution from the even and odd sub solutions.
            for (int j = 0; j < n / 2; j++)
            {
                solution[j] = add(evenSol[j], multiply(Omega[j], oddSol[j]));
                solution[j + n / 2] = subtract(evenSol[j], multiply(Omega[j], oddSol[j]));
            }
            return solution;
        }

        // This function multiplies two complex numbers together.
        static Complex multiply(Complex one, Complex two)
        {
            double real = (one.Real * two.Real) - (one.Imaginary * two.Imaginary);
            double imaginary = (one.Real * two.Imaginary) + (one.Imaginary * two.Real);

            return new Complex(real, imaginary);
        }

        // This function adds two complex numbers together.
        static Complex add(Complex one, Complex two)
        {
            double real = one.Real + two.Real;
            double imaginary = one.Imaginary + two.Imaginary;

            return new Complex(real, imaginary);
        }

        // This function subtracts two complex numbers.
        static Complex subtract(Complex one, Complex two)
        {
            double real = one.Real - two.Real;
            double imaginary = one.Imaginary - two.Imaginary;

            return new Complex(real, imaginary);
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
