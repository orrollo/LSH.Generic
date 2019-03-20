using System;

namespace LSH.Generic
{
    public static class MathHelper
    {
        public static double Diff(double x0, Func<double, double> fn, double error = 1e-8, double h0 = 0.01)
        {
            int finDiff = 3, half = 4, size = half * 2 + 1;

            double[,] vals = new double[2,size];
            double fnMax = -1;
            for (int i = 0, j = -half; i < size; i++, j++)
            {
                var tx = x0 + j * h0;
                var current = fn(tx);
                vals[0, i] = current;
                fnMax = Math.Max(fnMax, Math.Abs(current));
            }

            int t = 1, c = 0, k = 8;
            for (int step = 0; step < finDiff; step++, t = 1-t, c = 1-c, k--) for (int i = 0; i < k; i++) vals[t, i] = vals[c, i + 1] - vals[c, i];

            double cand = -1;
            for (int i = 0; i <= k; i++) cand = Math.Max(cand, Math.Abs(vals[c, i]));

            double m3 = cand / Math.Pow(h0, 3);

            double hOpt = Math.Pow((3 * fnMax * 1e-10) / m3, 1.0 / 3.0);

            return (fn(x0 + hOpt) - fn(x0 - hOpt)) / (2 * hOpt);
        }
    }

    public static class LshParamsHelper
    {
        public class LshParamsSet
        {
            public double BandsCount { get; set; }
            public double RowsInBand { get; set; }
        }

        public static double Probability(double sim, double rows, double bands)
        {
            return 1.0 - Math.Pow(1.0 - Math.Pow(sim, rows), bands);
        }

        public static double Probability(double sim, LshParamsSet set)
        {
            return Probability(sim, set.RowsInBand, set.BandsCount);
        }

        public static LshParamsSet Solve(double sim1, double prob1, double sim2, double prob2, double squareError = 1e-5)
        {
            double r = 0.1, b = 0.1, r0 = 0, b0 = 0;

            //double Fn(double s, double p, double tr, double tb) => Probability(s, tr, tb) - p;
            double DFnDr(double s, double p, double tr, double tb) => tb * Math.Pow(1.0 - Math.Pow(s, tr), tb - 1) * Math.Pow(s, tr) * Math.Log(s);
            double DFnDb(double s, double p, double tr, double tb) => -Math.Pow(1.0 - Math.Pow(s, tr), tb)*Math.Log(1.0 - Math.Pow(s, tr));

            double f1 = Probability(sim1, r, b) - prob1;
            double f2 = Probability(sim2, r, b) - prob2;

            double[] jacob = new double[4];

            while (!((f1 * f1 + f2 * f2) < squareError))
            {
                r0 = r;
                b0 = b;

                jacob[0] = DFnDr(sim1, prob1, r0, b0);
                jacob[1] = DFnDb(sim1, prob1, r0, b0);
                jacob[2] = DFnDr(sim2, prob2, r0, b0);
                jacob[3] = DFnDb(sim2, prob2, r0, b0);

                double det = jacob[0] * jacob[3] - jacob[1] * jacob[2];

                r = r0 - (f1 * jacob[3] - f2 * jacob[1]) / det;
                b = b0 - (- f1 * jacob[2] + f2 * jacob[0]) / det;

                f1 = Probability(sim1, r, b) - prob1;
                f2 = Probability(sim2, r, b) - prob2;
            }

            return new LshParamsSet()
            {
                BandsCount = b,
                RowsInBand = r
            };
        }
    }

    public class NGramMinHash : GenericMinHash<char>
    {
        public NGramMinHash(MinHashParams hashParams) : base(hashParams)
        {
        }

        protected override int Converter(char src) => (int) src;
    }
}