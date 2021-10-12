using System.Collections.Generic;
using System.Linq;

namespace CutPlanner
{
    /// <summary>
    /// An immutable representation of the cuts made on a piece of stock.
    /// </summary>
    class CutPlan
    {
        /// <summary>
        /// The length of the stock material.
        /// </summary>
        public double StockLength { get; private set; }

        /// <summary>
        /// The lengths of each part we cut from the stock.
        /// </summary>
        private readonly List<double> cuts;

        public CutPlan(double stockLength)
        {
            this.StockLength = stockLength;
            this.cuts = new List<double>();
        }

        /// <summary>
        /// Copy constructor which makes a deep copy.
        /// </summary>
        /// <param name="other">The cut plan from which to create a copy.</param>
        public CutPlan(CutPlan other)
        {
            this.StockLength = other.StockLength;
            this.cuts = new List<double>(other.cuts);
        }

        /// <summary>
        /// Returns a new cut plan with the additional cut added without modifying this.
        /// </summary>
        /// <param name="length">The length of the new cut to make (not including the blade width).</param>
        /// <returns>A new cut plan with the additional cut added.</returns>
        public CutPlan AddCut(double length)
        {
            CutPlan output = new CutPlan(this);
            output.cuts.Add(length);
            return output;
        }

        /// <summary>
        /// Returns a new cut plan with the additional cuts added without modifying this.
        /// </summary>
        /// <param name="cutLengths">The lengths of each new cut to make (not including the blade width).</param>
        /// <returns>a new cut plan with the additional cuts added.</returns>
        public CutPlan AddCuts(IEnumerable<double> cutLengths)
        {
            CutPlan output = new CutPlan(this);
            output.cuts.AddRange(cutLengths);
            return output;
        }

        /// <summary>
        /// Returns a new cut plan with the same cuts but a new stock length without modifying this.
        /// </summary>
        /// <param name="newStockLength">The stock length of the new cut plan.</param>
        /// <returns>a new cut plan with the new stock length.</returns>
        public CutPlan ChangeStock(double newStockLength)
        {
            CutPlan output = new CutPlan(this);
            output.StockLength = newStockLength;
            return output;
        }

        /// <summary>
        /// The remaining stock available after the cuts have been made.
        /// </summary>
        /// <param name="endTrimLength">The length which must be trimmed from each end of the stock material.</param>
        /// <param name="bladeWidth">The length of material which is lost from each cut.</param>
        /// <returns>The remaining length of stock.</returns>
        public double Remaining(double endTrimLength, double bladeWidth)
        {
            return this.StockLength - 2 * endTrimLength - this.cuts.Sum() - bladeWidth * (this.cuts.Count - 1);
        }

        public override string ToString()
        {
            return this.cuts.Aggregate($"{this.StockLength}:", (str, cut) => str + $" { cut}");
        }

        public string ToString(double endTrimLength, double bladeWidth)
        {
            return this.ToString() + $" ({this.Remaining(endTrimLength, bladeWidth)} remaining)";
        }
    }
}
