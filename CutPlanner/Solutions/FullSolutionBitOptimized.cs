using System;
using System.Collections.Generic;

namespace CutPlanner.Solutions
{
    /// <summary>
    /// A performance optimization of FullSolution which represents each CutPlan as a single ulong during computation.
    /// </summary>
    class FullSolutionBitOptimized : FullSolution
    {
        /// <summary>
        /// The length of each unique part in the specification.
        /// </summary>
        private readonly double[] partLengths;

        /// <summary>
        /// The length of each individual part represented as an index into partLengths, ordered from longest to shortest.
        /// </summary>
        private readonly ulong[] allPartIndices;

        public FullSolutionBitOptimized(Spec spec) : base(spec)
        {
            // Sort parts from longest to shortest
            Part[] parts = (Part[])spec.Parts.Clone();
            Array.Sort(parts);

            // Calculate every part length we must cut as an index into partLengths (from shortest to longest)
            this.partLengths = new double[parts.Length];
            List<ulong> allPartIndices = new List<ulong>(parts.Length);
            for (int i = 0; i < parts.Length; i++)
            {
                this.partLengths[i] = parts[i].Length;
                for (int j = 0; j < parts[i].Quantity; j++)
                {
                    allPartIndices.Add((ulong)i);
                }
            }
            this.allPartIndices = allPartIndices.ToArray();
        }

        public override FullPlan Solve()
        {
            if (this.stockLengths.Length > 16 || this.partLengths.Length > 16 || this.partLengths[^1] * 14 < this.stockLengths[^1])
            {
                // Any of these cases can potentially overflow our CutPlan encoding, so revert to the unoptimized solution
                return base.Solve();
            }

            ulong[] encodedPlan = this.SolveHelperOptimized(0, new ulong[0]);

            CutPlan[] plan = new CutPlan[encodedPlan.Length];
            for (int i = 0; i < encodedPlan.Length; i++)
            {
                plan[i] = this.ConvertCutPlan(encodedPlan[i]);
            }
            return new FullPlan(this.spec, plan);
        }

        private ulong[] SolveHelperOptimized(int curPart, ulong[] partials)
        {
            ulong cutIndex = this.allPartIndices[curPart];
            double length = this.partLengths[cutIndex];
            double lengthWithCut = length + this.spec.BladeWidth;
            curPart++;

            // Base case: this is the last part
            if (curPart == this.allPartIndices.Length)
            {
                // Try to take the part from any of the partials
                for (int i = 0; i < partials.Length; i++)
                {
                    if (Remaining(partials[i]) >= lengthWithCut)
                    {
                        // We need to make a copy so that we don't modify partials
                        // A shallow copy is okay because ulong is a value type
                        ulong[] copy = (ulong[])partials.Clone();
                        copy[i] = AddCut(copy[i], cutIndex);
                        return copy;
                    }
                }

                // Otherwise, take the part from the smallest stock (stock index 0)
                ulong[] finalPlan = new ulong[partials.Length + 1];
                Array.Copy(partials, finalPlan, partials.Length);
                for (ulong i = 0; i < (ulong)this.stockLengths.Length; i++)
                {
                    if (this.stockLengthsWithTrim[i] >= length)
                    {
                        finalPlan[^1] = (cutIndex << 8) | 0x10u | i;
                        return finalPlan;
                    }
                }
            }

            double curStock = TotalStock(partials);
            ulong[] bestPlan = null;
            double bestStock = double.PositiveInfinity;

            // Try to take the current part from all partials
            for (int i = 0; i < partials.Length; i++)
            {
                if (Remaining(partials[i]) >= lengthWithCut)
                {
                    ulong prevPartial = partials[i];
                    partials[i] = AddCut(partials[i], cutIndex);
                    ulong[] result = SolveHelperOptimized(curPart, partials);

                    if (result.Length == partials.Length)
                    {
                        // If this result did not add any new stock, we cannot do any better so return here
                        return result;
                    }

                    double resultStock = TotalStock(result);
                    if (resultStock < bestStock)
                    {
                        bestStock = resultStock;
                        bestPlan = result;
                    }

                    // Undo the change made to partials
                    partials[i] = prevPartial;
                }
            }

            ulong[] newPartials = new ulong[partials.Length + 1];
            Array.Copy(partials, newPartials, partials.Length);

            // Try to take the current part from all stocks
            for (ulong i = 0; i < (ulong)this.stockLengths.Length; i++)
            {
                if (bestStock <= curStock + this.stockLengths[i])
                {
                    // The best solution so far uses less stock than any further solution
                    // in this for loop could use, so stop searching
                    return bestPlan;
                }
                else if (this.stockLengthsWithTrim[i] >= length)
                {
                    newPartials[^1] = (cutIndex << 8) | 0x10u | i;

                    ulong[] result = SolveHelperOptimized(curPart, newPartials);
                    double resultStock = TotalStock(result);
                    if (resultStock < bestStock)
                    {
                        bestStock = resultStock;
                        bestPlan = result;
                    }
                }
            }

            return bestPlan;
        }

        /// <summary>
        /// Calculates the available stock remaining on a cut plan.
        /// </summary>
        /// <param name="cutPlan">The cut plan.</param>
        /// <returns>The length of free stock remaining.</returns>
        private double Remaining(ulong cutPlan)
        {
            double result = this.stockLengthsWithTrim[cutPlan & 0xFu]; ;
            for (int shiftAmount = (int)(((cutPlan >> 4) & 0xFu) + 1) * 4; shiftAmount >= 8; shiftAmount -= 4)
            {
                result -= this.partLengths[(cutPlan >> shiftAmount) & 0xFu] + this.spec.BladeWidth;
            }
            return result + this.spec.BladeWidth;
        }

        /// <summary>
        /// Adds a cut to a cut plan.
        /// </summary>
        /// <param name="cutPlan">The cut plan.</param>
        /// <param name="cutIndex">The index of the part to add.</param>
        /// <returns>The cut plan with the new cut added.</returns>
        private ulong AddCut(ulong cutPlan, ulong cutIndex)
        {
            ulong numCuts = ((cutPlan >> 4) & 0xFu) + 1;
            cutPlan &= 0xFFFFFFFFFFFFFF0Fu;
            cutPlan |= numCuts << 4;
            cutPlan |= cutIndex << ((int)(numCuts + 1) * 4);
            return cutPlan;
        }

        /// <summary>
        /// Converts a ulong representation of a cut plan to a CutPlan object.
        /// </summary>
        /// <param name="optimizedCutPlan">The ulong representation of the cut plan.</param>
        /// <returns>The CutPlan representation of the cut plan.</returns>
        private CutPlan ConvertCutPlan(ulong optimizedCutPlan)
        {
            CutPlan cutPlan = new CutPlan(this.stockLengths[optimizedCutPlan & 0xFu]);
            for (int shiftAmount = (int)(((optimizedCutPlan >> 4) & 0xFu) + 1) * 4; shiftAmount >= 8; shiftAmount -= 4)
            {
                cutPlan = cutPlan.AddCut(this.partLengths[(optimizedCutPlan >> shiftAmount) & 0xFu]);
            }
            return cutPlan;
        }

        /// <summary>
        /// Calculates the total length of stock used by a collection of cut plans.
        /// </summary>
        /// <param name="cutPlans">The array of cut plans.</param>
        /// <returns>The total length of stock used across the provided cut plans.</returns>
        private double TotalStock(ulong[] cutPlans)
        {
            // We use a for loop rather than Aggregate or a foreach loop for performance
            double result = 0;
            for (int i = 0; i < cutPlans.Length; i++)
            {
                result += this.stockLengths[cutPlans[i] & 0xFu];
            }
            return result;
        }
    }
}
