using System;
using System.Collections.Generic;
using System.Linq;

namespace CutPlanner.Solutions
{
    /// <summary>
    /// A full optimal implementation of SolutionBase with some algorithmic performance optimizations.
    /// </summary>
    class FullSolution : SolutionBase
    {
        /// <summary>
        /// Stock lengths ordered from shortest to longest.
        /// </summary>
        protected readonly double[] stockLengths;

        /// <summary>
        /// Stock lengths accounting for end trim ordered from shortest to longest.
        /// </summary>
        protected readonly double[] stockLengthsWithTrim;

        /// <summary>
        /// The length of each individual part, ordered from longest to shortest.
        /// </summary>
        private readonly double[] allPartLengths;

        public FullSolution(Spec spec) : base(spec)
        {
            // Sort stocks from shortest to longest and add end trim
            this.stockLengths = (double[])spec.StockLengths.Clone();
            Array.Sort(this.stockLengths);
            this.stockLengthsWithTrim = stockLengths.Select((stock) => stock - 2 * spec.EndTrimLength).ToArray();

            // Sort parts from longest to shortest
            Part[] parts = (Part[])spec.Parts.Clone();
            Array.Sort(parts);

            // Calculate every part length we must cut (from longest to shortest)
            List<double> allPartLengths = new List<double>(parts.Length);
            for (int i = 0; i < parts.Length; i++)
            {
                for (int j = 0; j < parts[i].Quantity; j++)
                {
                    allPartLengths.Add(parts[i].Length);
                }
            }
            this.allPartLengths = allPartLengths.ToArray();
        }

        public override FullPlan Solve()
        {
            return new FullPlan(this.spec, this.SolveHelper(0, new double[0], new CutPlan[0]));
        }

        public override double MinStock()
        {
            return this.MinStockHelper(0, new double[0]);
        }

        private CutPlan[] SolveHelper(int curPart, double[] partials, CutPlan[] cutPlans)
        {
            double length = this.allPartLengths[curPart];
            double lengthWithCut = length + this.spec.BladeWidth;
            curPart++;

            // Base case: this is the last part
            if (curPart == this.allPartLengths.Length)
            {
                // Try to take the part from any of the partials
                for (int i = 0; i < partials.Length; i++)
                {
                    if (partials[i] >= lengthWithCut)
                    {
                        // We need to make a copy so that we don't modify cutPlans
                        // A shallow copy is okay because CutPlan is immutable
                        CutPlan[] copy = (CutPlan[])cutPlans.Clone();
                        copy[i] = copy[i].AddCut(length);
                        return copy;
                    }
                }

                // Otherwise, take the part from the smallest stock which accommodates it
                CutPlan[] finalPlans = new CutPlan[cutPlans.Length + 1];
                Array.Copy(cutPlans, finalPlans, cutPlans.Length);
                for (int i = 0; i < this.stockLengths.Length; i++)
                {
                    if (this.stockLengthsWithTrim[i] >= length)
                    {
                        finalPlans[^1] = new CutPlan(this.stockLengths[0]).AddCut(length);
                        return finalPlans;
                    }
                }
            }

            CutPlan[] bestPlans = null;
            double bestStock = double.PositiveInfinity;

            // Try to take the current part from all partials
            for (int i = 0; i < partials.Length; i++)
            {
                if (partials[i] >= lengthWithCut)
                {
                    CutPlan curPlan = cutPlans[i];
                    cutPlans[i] = curPlan.AddCut(length);
                    partials[i] -= lengthWithCut;
                    CutPlan[] result = SolveHelper(curPart, partials, cutPlans);

                    if (result.Length == partials.Length)
                    {
                        // If this result did not add any new stock, we cannot do any better so return here
                        return result;
                    }

                    double resultStock = result.Aggregate(0.0, (sum, cutPlan) => sum + cutPlan.StockLength);
                    if (resultStock < bestStock)
                    {
                        bestStock = resultStock;
                        bestPlans = result;
                    }

                    // Undo changes to cutPlans and partials
                    cutPlans[i] = curPlan;
                    partials[i] += lengthWithCut;
                }
            }

            double curStock = cutPlans.Aggregate(0.0, (sum, cutPlan) => sum + cutPlan.StockLength);
            CutPlan[] newPlans = new CutPlan[cutPlans.Length + 1];
            Array.Copy(cutPlans, newPlans, cutPlans.Length);
            double[] newPartials = new double[partials.Length + 1];
            Array.Copy(partials, newPartials, partials.Length);

            // Try to take the current part from all stocks
            for (int i = 0; i < this.stockLengths.Length; i++)
            {
                if (bestStock <= curStock + this.stockLengths[i])
                {
                    // The best solution so far uses less stock than any further solution
                    // in this for loop could use, so stop searching
                    break;
                }
                else if (this.stockLengthsWithTrim[i] >= length)
                {
                    newPlans[^1] = new CutPlan(this.stockLengths[i]).AddCut(length);
                    newPartials[^1] = this.stockLengthsWithTrim[i] - length;

                    CutPlan[] result = SolveHelper(curPart, newPartials, newPlans);
                    double resultStock = result.Aggregate(0.0, (sum, cutPlan) => sum + cutPlan.StockLength);
                    if (resultStock < bestStock)
                    {
                        bestStock = resultStock;
                        bestPlans = result;
                    }
                }
            }

            return bestPlans;
        }

        private double MinStockHelper(int curPart, double[] partials)
        {
            double length = this.allPartLengths[curPart];
            double lengthWithCut = length + this.spec.BladeWidth;
            curPart++;

            // Base case: this is the last part
            if (curPart == this.allPartLengths.Length)
            {
                // Try to take the part from any of the partials
                for (int i = 0; i < partials.Length; i++)
                {
                    if (partials[i] >= lengthWithCut)
                    {
                        return 0;
                    }
                }

                // Otherwise, take the part from the smallest stock that accommodates it
                for (int i = 0; i < this.stockLengths.Length; i++)
                {
                    if (this.stockLengthsWithTrim[i] >= length)
                    {
                        return this.stockLengths[i];
                    }
                }
            }

            double bestResult = double.PositiveInfinity;

            // Try to take the current part from all partials
            for (int i = 0; i < partials.Length; i++)
            {
                if (partials[i] >= lengthWithCut)
                {
                    partials[i] -= lengthWithCut;

                    double result = MinStockHelper(curPart, partials);

                    if (result == 0)
                    {
                        // This is the best possible result, so no need to search further
                        return result;
                    }

                    bestResult = Math.Min(result, bestResult);

                    // Undo the change made to partials
                    partials[i] += lengthWithCut;
                }
            }

            // Try to take the current part from all stocks
            double[] newPartials = new double[partials.Length + 1];
            Array.Copy(partials, newPartials, partials.Length);
            for (int i = 0; i < this.stockLengths.Length; i++)
            {
                if (bestResult <= this.stockLengths[i])
                {
                    // The best solution so far used less stock than each remaining stock length
                    // so no need to continue searching
                    break;
                }
                else if (this.stockLengthsWithTrim[i] >= length)
                {
                    newPartials[^1] = this.stockLengthsWithTrim[i] - length;
                    double result = this.stockLengths[i] + MinStockHelper(curPart, newPartials);
                    bestResult = Math.Min(result, bestResult);
                }
            }

            return bestResult;
        }
    }
}
