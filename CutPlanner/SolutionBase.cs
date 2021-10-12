namespace CutPlanner
{
    /// <summary>
    /// Something which can calculate a plan to make the parts of a specification.
    /// </summary>
    /// <remarks>Every solution should inherit from this class.</remarks>
    abstract class SolutionBase
    {
        /// <summary>
        /// The design specification to be solved.
        /// </summary>
        protected readonly Spec spec;

        /// <summary>
        /// Calculates a full cut plan for the provided specification.
        /// </summary>
        /// <returns>A plan specifying the stock and cuts needed to create the parts in the specification.</returns>
        public abstract FullPlan Solve();

        /// <summary>
        /// Calculates the minimum stock material necessary to satisfy the specification.
        /// </summary>
        /// <returns></returns>
        public abstract double MinStock();

        public SolutionBase(Spec spec)
        {
            this.spec = spec;
        }
    }
}
