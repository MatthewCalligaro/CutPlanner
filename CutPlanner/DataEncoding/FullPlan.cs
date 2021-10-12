using System;
using System.Linq;

namespace CutPlanner
{
    /// <summary>
    /// All stock and cuts needed to satisfy a design specification.
    /// </summary>
    class FullPlan
    {
        /// <summary>
        /// A collection of cuts on stock materials.
        /// </summary>
        public readonly CutPlan[] CutPlans;

        /// <summary>
        /// The specification which the plan satisfies.
        /// </summary>
        public readonly Spec Spec;

        /// <summary>
        /// The total stock length used by the plan.
        /// </summary>
        public double TotalStock
        {
            get
            {
                return CutPlans.Aggregate(0.0, (sum, cutPlan) => sum + cutPlan.StockLength);
            }
        }

        /// <summary>
        /// The total length of all parts in the design specification.
        /// </summary>
        public double TotalPartLength
        {
            get
            {
                return Spec.Parts.Aggregate(0.0, (sum, part) => sum + part.Length * part.Quantity);
            }
        }

        /// <summary>
        /// The total amount of stock material which is not used as parts.
        /// </summary>
        public double TotalWaste
        {
            get
            {
                return this.TotalStock - this.TotalPartLength;
            }
        }

        /// <summary>
        /// The fraction of the stock material which is wasted by the plan.
        /// </summary>
        public double WasteFraction
        {
            get
            {
                return this.TotalWaste / TotalStock;
            }
        }

        public FullPlan(Spec spec, CutPlan[] cutPlans)
        {
            this.Spec = spec;
            this.CutPlans = cutPlans;
        }

        public override string ToString()
        {
            string output = $"Total Stock: {this.TotalStock}, Total Part Length: {this.TotalPartLength}, Waste Percentage: {WasteFraction * 100:F2}%";
            foreach (CutPlan cutPlan in this.CutPlans)
            {
                output += "\n" + cutPlan.ToString(this.Spec.EndTrimLength, this.Spec.BladeWidth);
            }
            return output;
        }

        public override bool Equals(Object obj)
        {
            return obj is FullPlan other && this.TotalStock == other.TotalStock && this.Spec.Equals(other.Spec);
        }
    }
}
