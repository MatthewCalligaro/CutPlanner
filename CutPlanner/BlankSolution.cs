using System;

namespace CutPlanner
{
    class BlankSolution : SolutionBase
    {
        // TODO (optional): Add any data members here 

        public BlankSolution(Spec spec) : base(spec)
        {
            // TODO (optional): Add any pre-computation here
        }

        public override double MinStock()
        {
            // TODO: Calculate the minimum length of stock material needed to satisfy the specification
            // Hint: you can access the specification as "this.spec" (it is a protected member of SolutionBase)
            throw new NotImplementedException();
        }

        public override FullPlan Solve()
        {
            // TODO: Calculate a series of cut plans to satisfy the specification
            // Hint: Take a close look at the FullPlan and CutPlan classes
            throw new NotImplementedException();
        }
    }
}
