# CutPlanner
A program for planning the cuts needed to make parts from stock material.

## Repository Contents
* `Program.cs`: The entry point for the command-line application.
* `SolutionBase.cs`: The base class from which all solutions inherit.
* `BlankSolution.cs`: An empty solution provided in case you wish to write your own solution.
* `Solution` directory: Contains a variety of solutions to the problem.
* `DataEncoding` directory: Contains classes which encode different parts of the problem and solution.

## Problem Description
Supose that we are creating a project which requires several parts cut from the same stock material (for example, several pieces of wood cut from 2x4's). At the store, we can purchase stock material at multiple lengths (such as 8ft, 10ft, and 12ft 2x4's). Our goal is to plan our cuts to minimize wasted stock material. An *optimal* solution is one which uses the minimum amount of stock possible.

We must also account for the following: 
* We must remove `EndTrimLength` from each end of each piece of stock, since the end of raw stock is often rough. 
* Each cut that we make on the stock (other than the end trim cuts) removes `BladeWidth` length of material.

### Example
Suppose that we need two copies of a part A (60 inches) and four copies of a part B (10 inches). The available stock lengths are 72 and 96 inches, `EndTrimLength` is 1/4 inch, and `BladeWidth` is 1/8 inch. One optimal solution is as follows:
* (1) 72 inch stock: (1) part A, (1) part B. The total material used is `60 + 10 + 1/4 * 2 + 1/8 = 70.625`, which is less than 72.
* (1) 96 inch stock:  (1) part A, (3) part Bs. The total material used is `60 + 10 * 3 + 1/4 * 2 + 1/8 * 3 = 90.875`, which is less than 96.

Our solution uses a total of 168 inches of stock.

### Writing a Solution
To solve this problem, you can write your solution in `BlankSolution.cs` in the root level of the project. `BlankSolution` inherits from the `SolutionBase` class. To test your souliton, use the methods provided in `Program.cs`, which is the entry point to the command-line application.

**If you do not want to see other solutions, do not look in the `Solutions` directory.**

## Future Work
Algorithmic
* Add aditional improvements to GreedyPlus
* Create a multithreading optimization of FullSolution

Proofs
* NP Hardness proof
* Greed approximation proof

Features
* Command-line arguments
* Read parts from a CSV
* Allow for angled parts
* Read parts from Onshape directly

Other
* Unit tests
