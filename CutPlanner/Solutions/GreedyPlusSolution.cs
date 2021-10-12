using System;
using System.Linq;

namespace CutPlanner.Solutions
{
    /// <summary>
    /// An improvement over the greedy algorithm which takes some steps to reduce wasted material.
    /// This algorithm is still provably non-optimal.
    /// </summary>
    class GreedyPlusSolution : GreedySolution
    {
        /// <summary>
        /// The amount each stock is shorter than the longest stock, sorted from shortest to second-longest.
        /// </summary>
        private readonly double[] stockLengthDifs;

        public GreedyPlusSolution(Spec spec) : base(spec)
        {
            // Sort stocks from shortest to longest
            double[] stockLengths = (double[])spec.StockLengths.Clone();
            Array.Sort(stockLengths);

            // Calculate the difference from the longest stock for each stock (except the longest)
            this.stockLengthDifs = stockLengths[0..^1].Select((stockLength) => this.longestStockLength - stockLength).ToArray();
        }

        public override FullPlan Solve()
        {
            CutPlan[] cutPlans = base.Solve().CutPlans;

            // For each cut plan, swap out for the smallest stock possible (without changing the cuts)
            for (int i = 0; i < cutPlans.Length; i++)
            {
                double remaining = cutPlans[i].Remaining(this.spec.EndTrimLength, this.spec.BladeWidth);
                foreach (double dif in this.stockLengthDifs)
                {
                    if (dif < remaining)
                    {
                        cutPlans[i] = cutPlans[i].ChangeStock(this.longestStockLength - dif);
                        break;
                    }
                }
            }

            return new FullPlan(this.spec, cutPlans);
        }
    }
}
