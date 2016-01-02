namespace Yuizumi.TexasHoldem
{
    public struct Stats
    {
        public int Win { get; internal set; }
        public int Lose { get; internal set; }
        public int Split { get; internal set; }

        public double WinProbability
        {
            get { return Win / Denominator; }
        }

        public double LoseProbability
        {
            get { return Lose / Denominator; }
        }

        public double SplitProbability
        {
            get { return Split / Denominator; }
        }

        private double Denominator
        {
            get { return (double) (Win + Lose + Split); }
        }
    }
}
