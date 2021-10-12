namespace CutPlanner
{
    /// <summary>
    /// Encapsulates a design specification, including the parts of the design and the available stock lengths.
    /// </summary>
    class Spec
    {
        #region Specs
        /// <summary>
        /// A real example of the parts needed for a mini garden house.
        /// </summary>
        public static readonly Spec MiniGarden = new Spec(
            parts: new Part[]
            {
                new Part(54, 2),
                new Part(43.5, 4),
                new Part(22, 4),
                new Part(48, 3),
                new Part(9.75, 8),
            },
            stockLengths: new double[]
            {
                6 * 12,
                8 * 12
            },
            endTrimLength: 0.25,
            bladeWidth: 0.125
        );

        /// <summary>
        /// A real example of the 2x4 parts needed for a climbing wall.
        /// </summary>
        public static readonly Spec ClimbingWall2x4 = new Spec(
            parts: new Part[]
            {
                new Part(42.75, 6),
                new Part(44.5, 6),
                new Part(112, 2),
                new Part(55.5, 1),
                new Part(35.25, 1),
            },
            stockLengths: new double[]
            {
                8 * 12,
                10 * 12,
                12 * 12,
            },
            endTrimLength: 0.25,
            bladeWidth: 0.125
        );

        /// <summary>
        /// A real example of the 4x4 parts needed for a climbing wall.
        /// </summary>
        public static readonly Spec ClimbingWall4x4 = new Spec(
            parts: new Part[]
            {
                new Part(75, 2),
                new Part(70, 2),
                new Part(106, 5),
                new Part(42.75, 4),
                new Part(44.5, 4),
            },
            stockLengths: new double[]
            {
                8 * 12,
                10 * 12,
                12 * 12,
            },
            endTrimLength: 0.25,
            bladeWidth: 0.125
        );

        /// <summary>
        /// The contrived example provided in the README.
        /// </summary>
        public static readonly Spec ReadmeExample = new Spec(
            parts: new Part[]
            {
                new Part(60, 2),
                new Part(10, 4),
            },
            stockLengths: new double[]
            {
                6 * 12,
                8 * 12,
            },
            endTrimLength: 0.25,
            bladeWidth: 0.125
        );

        /// <summary>
        /// A contrived example for which the greedy solution will not provide an optimal plan. 
        /// </summary>
        public static readonly Spec GreedCounter = new Spec(
            parts: new Part[]
            {
                new Part(5, 2),
                new Part(3, 4),
            },
            stockLengths: new double[]
            {
                12,
            },
            endTrimLength: 0.25,
            bladeWidth: 0.125
        );

        /// <summary>
        /// A contrived example where some parts are longer than some stocks.
        /// </summary>
        public static readonly Spec OversizedParts = new Spec(
            parts: new Part[]
            {
                new Part(5, 2),
                new Part(3, 2),
            },
            stockLengths: new double[]
            {
                6,
                4,
                2,
            },
            endTrimLength: 0.25,
            bladeWidth: 0.125
        );

        /// <summary>
        /// A contrived example where the blade width is critical to calculating a correct result.
        /// </summary>
        public static readonly Spec BladeWidthTest = new Spec(
            parts: new Part[]
            {
                new Part(10, 1),
                new Part(1, 3),
            },
            stockLengths: new double[]
            {
                10.5,
                10,
                3.625,
            },
            endTrimLength: 0.25,
            bladeWidth: 0.125
        );
        #endregion

        /// <summary>
        /// The parts needed for the design.
        /// </summary>
        public readonly Part[] Parts;

        /// <summary>
        /// The available lengths of stock material.
        /// </summary>
        public readonly double[] StockLengths;

        /// <summary>
        /// The length which must be trimmed from each end of the stock material.
        /// </summary>
        public double EndTrimLength { get; private set; }

        /// <summary>
        /// The length of material which is lost from each cut.
        /// </summary>
        public double BladeWidth { get; private set; }

        /// <param name="parts">The parts needed for the design.</param>
        /// <param name="stockLengths">The available lengths of stock material.</param>
        /// <param name="endTrimLength">The amount of material that must be removed from each end of raw stock.</param>
        /// <param name="bladeWidth">The width of material which is lost from each cut.</param>
        public Spec(Part[] parts, double[] stockLengths, double endTrimLength, double bladeWidth)
        {
            this.Parts = parts;
            this.StockLengths = stockLengths;
            this.EndTrimLength = endTrimLength;
            this.BladeWidth = bladeWidth;
        }
    }
}
