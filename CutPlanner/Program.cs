using System;
using System.Diagnostics;
using CutPlanner.Solutions;

namespace CutPlanner
{
    /// <summary>
    /// The command-line program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Entry point for the command-line program
        /// </summary>
        static void Main()
        {
            Spec spec = Spec.ReadmeExample;
            SolutionBase solution = new GreedyPlusSolution(spec);
            bool compare = true;

            if (compare)
            {
                SolutionBase benchmark = new FullSolution(spec);
                Compare(solution.Solve, benchmark.Solve);
            }
            else
            {
                Solve(solution);
                Console.WriteLine("================================================================================");
                MinStock(solution);
            }
        }

        /// <summary>
        /// Finds and prints the cut plan calculated for the spec by the provided solution.
        /// </summary>
        /// <param name="solution">The solution object which will calculate the cut plan.</param>
        static void Solve(SolutionBase solution)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            FullPlan plan = solution.Solve();
            stopwatch.Stop();
            Console.WriteLine(plan);
            Console.WriteLine($"Time: {Format(stopwatch.Elapsed)}");
        }

        /// <summary>
        /// Finds and prints the minimum stock length calculated for the spec by the provided solution.
        /// </summary>
        /// <param name="solution">The solution object which will calculate the minimum stock length.</param>
        static void MinStock(SolutionBase solution)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            double minStock = solution.MinStock();
            stopwatch.Stop();
            Console.WriteLine($"MinStock: {minStock}");
            Console.WriteLine($"Time: {Format(stopwatch.Elapsed)}");
        }

        /// <summary>
        /// Compares the results and performance of two solutions.
        /// </summary>
        /// <typeparam name="T">The return type of the functions being compared.</typeparam>
        /// <param name="dut">The function to call on using the solution being evaluated.</param>
        /// <param name="benchmark">The equivalent function to call using the benchmark solution.</param>
        /// <param name="trials">The number of trials to run, not counting warm-up.</param>
        /// <param name="requireEquality">If true, the comparison will cancel if dut and benchmark return different results.</param>
        static void Compare<T>(Func<T> dut, Func<T> benchmark, int trials = 10, bool requireEquality = false)
        {
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan averageBenchmark = TimeSpan.Zero;
            TimeSpan averageDut = TimeSpan.Zero;

            // Do a single untimed run of each as a "warm-up"
            Console.WriteLine("Warming up...");
            T benchmarkResult = benchmark.Invoke();
            T dutResult = dut.Invoke();

            // Verify that dut and benchmark gave equivalent results
            bool exitEarly = false;
            if (!dutResult.Equals(benchmarkResult))
            {
                if (requireEquality)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR: dut and benchmark did not return equivalent results.");
                    exitEarly = true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("WARNING: dut and benchmark did not return equivalent results.");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("SUCCESS: dut and benchmark returned equivalent results.");
            }
                
            Console.WriteLine("\n================================== Dut result ==================================");
            Console.WriteLine(dutResult);

            Console.WriteLine("\n=============================== Benchmark result ===============================");
            Console.WriteLine(benchmarkResult);
            Console.WriteLine("================================================================================\n");
            Console.ResetColor();

            if (exitEarly)
            {
                return;
            }

            for (int i = 0; i < trials; ++i)
            {
                Console.WriteLine($"Trial {i + 1} of {trials}");

                // Run benchmark
                stopwatch.Reset();
                stopwatch.Start();
                benchmark.Invoke();
                stopwatch.Stop();
                TimeSpan benchmarkTime = stopwatch.Elapsed;
                averageBenchmark += benchmarkTime;
                Console.WriteLine($"Benchmark: {Format(benchmarkTime)}");

                // Run dut
                stopwatch.Reset();
                stopwatch.Start();
                dut.Invoke();
                stopwatch.Stop();
                TimeSpan dutTime = stopwatch.Elapsed;
                averageDut += dutTime;
                Console.WriteLine($"Dut: {Format(dutTime)}");

                Console.WriteLine($"Savings: {Format(benchmarkTime - dutTime)}\n");
            }

            averageBenchmark /= trials;
            averageDut /= trials;
            TimeSpan averageSavings = averageBenchmark - averageDut;
            Console.WriteLine(
                "================================================================================\n" +
                $"Benchmark average: {Format(averageBenchmark)}\n" +
                $"Dut average: {Format(averageDut)}\n" +
                $"Average savings: {Format(averageSavings)} ({averageSavings * 100 / averageBenchmark:F2}%)\n" +
                "================================================================================\n");
        }

        /// <summary>
        /// Formats a TimeSpan into a human-readable duration, rounding to approximately 3 to 4 significant figures.
        /// </summary>
        /// <param name="duration">The elapsed time to format.</param>
        /// <returns>The duration expressed in a human-readable format.</returns>
        public static string Format(TimeSpan duration)
        {
            if (duration < TimeSpan.FromMilliseconds(0.1))
            {
                return $"{duration.Ticks} ticks";
            }
            else if (duration < TimeSpan.FromSeconds(1))
            {
                return $"{duration.TotalMilliseconds:F3} ms";
            }
            else if (duration < TimeSpan.FromMinutes(1))
            {
                return $"{duration.TotalSeconds:F3} seconds";
            }
            else if (duration < TimeSpan.FromHours(1))
            {
                return $"{duration.Minutes} min {duration.Seconds:F1} seconds";
            }
            return $"{Math.Floor(duration.TotalHours)} hours {duration.Minutes:F1} min";
        }
    }
}
