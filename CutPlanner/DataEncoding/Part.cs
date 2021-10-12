using System;

namespace CutPlanner
{
    /// <summary>
    /// A unique part in a design specification.
    /// </summary>
    class Part : IComparable<Part>
    {
        /// <summary>
        /// The length of the part.
        /// </summary>
        public readonly double Length;

        /// <summary>
        /// The number of copies of this part appearing in the specification.
        /// </summary>
        public readonly int Quantity;

        public Part(double length, int quantity)
        {
            this.Length = length;
            this.Quantity = quantity;
        }

        public int CompareTo(Part other)
        {
            return -Length.CompareTo(other.Length);
        }

        public override string ToString()
        {
            return this.Length.ToString();
        }
    }
}
