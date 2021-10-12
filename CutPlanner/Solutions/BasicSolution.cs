using System;
using System.Collections.Generic;

namespace CutPlanner.Solutions
{
    /// <summary>
    /// The simplest optimal implementation of MinStock without any performance optimizations.
    /// </summary>
    /// <remarks>This solution does not implement Solve.</remarks>
    class BasicSolution : SolutionBase
    {
        public BasicSolution(Spec spec) : base(spec) { }

        public override FullPlan Solve()
        {
            // Basic solution intentionally does not implement Solve
            throw new NotImplementedException();
        }

        public override double MinStock()
        {
            // Begin with no cuts/parts having been made
            return MinStock(0, 0, new List<double>());
        }

        /// <summary>
        /// A recursive helper function for MinStock which calculates the additional stock needed to make the remaining parts.
        /// </summary>
        /// <param name="curPartIndex">The index in the specification of the current part we are trying to make.</param>
        /// <param name="curPartQuantity">The number of copies of this part we have already made.</param>
        /// <param name="partials">The remaining length in every piece of stock that we have already used.</param>
        /// <returns>The additional length of stock needed to complete the specification given the cuts which have already been made.</returns>
        private double MinStock(int curPartIndex, int curPartQuantity, List<double> partials)
        {
            // Base case: we have accounted for all of the parts
            if (curPartIndex >= this.spec.Parts.Length)
            {
                return 0;
            }

            // Determine the length of the part to place this iteration
            Part curPart = this.spec.Parts[curPartIndex];
            double length = curPart.Length;
            curPartQuantity++;
            if (curPartQuantity >= curPart.Quantity)
            {
                curPartQuantity = 0;
                curPartIndex++;
            }

            double bestResult = double.PositiveInfinity;

            // Try to take the current part from all partials
            for (int i = 0; i < partials.Count; i++)
            {
                if (partials[i] >= length + this.spec.BladeWidth)
                {
                    List<double> newPartials = new List<double>(partials);
                    newPartials[i] -= length + this.spec.BladeWidth;

                    double result = MinStock(curPartIndex, curPartQuantity, newPartials);
                    bestResult = Math.Min(result, bestResult);
                }
            }

            // Try to take the current part from all stocks
            foreach (double stock in this.spec.StockLengths)
            {
                double stockLengthWithTrim = stock + 2 * this.spec.EndTrimLength;
                if (stockLengthWithTrim >= length)
                {
                    List<double> newPartials = new List<double>(partials);
                    newPartials.Add(stockLengthWithTrim);

                    double result = stock + MinStock(curPartIndex, curPartQuantity, newPartials);
                    bestResult = Math.Min(result, bestResult);
                }
            }

            return bestResult;
        }
    }
}
