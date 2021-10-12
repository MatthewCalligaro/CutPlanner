using System;
using System.Collections.Generic;
using System.Linq;

namespace CutPlanner.Solutions
{
    /// <summary>
    /// A non-optimal greedy solution which places parts longest to shortest, always using the longest stock length.
    /// </summary>
    class GreedySolution : SolutionBase
    {
        /// <summary>
        /// The length of the longest stock.
        /// </summary>
        protected readonly double longestStockLength;

        /// <summary>
        /// The parts of the specification ordered from longest to shortest.
        /// </summary>
        private readonly Part[] parts;

        public GreedySolution(Spec spec) : base(spec)
        {
            this.longestStockLength = spec.StockLengths.Max();

            // Sort parts from longest to shortest
            this.parts = (Part[])spec.Parts.Clone();
            Array.Sort(parts);
        }

        public override double MinStock()
        {
            // The greedy solution is fast enough that there is no need to calculate MinStock separately
            return this.Solve().TotalStock;
        }

        public override FullPlan Solve()
        {
            List<CutPlan> cutPlans = new List<CutPlan>();
            List<double> partLengths = this.parts.Select((part) => part.Length).ToList();
            List<int> partQuantities = this.parts.Select((part) => part.Quantity).ToList();

            while (partLengths.Count > 0)
            {
                // Always take from the longest stock
                CutPlan curCutPlan = new CutPlan(this.longestStockLength);

                // Note: we add an extra BladeWidth since the first part does use a cut
                double curStockRemainingLength = this.longestStockLength - 2 * this.spec.EndTrimLength + this.spec.BladeWidth;

                // Fill the stock with as many parts as we can, working from largest to smallest
                for (int i = 0; i < partLengths.Count;)
                {
                    double partLengthWithCut = partLengths[i] + this.spec.BladeWidth;
                    if (partLengthWithCut <= curStockRemainingLength)
                    {
                        // We can fit one more of the current part, so add it to the cut plan
                        curCutPlan = curCutPlan.AddCut(partLengths[i]);
                        curStockRemainingLength -= partLengthWithCut;
                        partQuantities[i]--;

                        if (partQuantities[i] <= 0)
                        {
                            // We have planned for all instances of the current part
                            partLengths.RemoveAt(i);
                            partQuantities.RemoveAt(i);
                        }
                    }
                    else
                    {
                        // We cannot fit any more of this part, so move on to the next one
                        i++;
                    }
                }

                cutPlans.Add(curCutPlan);
            }

            return new FullPlan(this.spec, cutPlans.ToArray());
        }
    }
}
