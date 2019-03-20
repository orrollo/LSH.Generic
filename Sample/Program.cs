using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LSH.Generic;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var sim1 = 0.65;
            var prob1 = 0.45;
            var sim2 = 0.85;
            var prob2 = 0.99;
            var set = LshParamsHelper.Solve(sim1, prob1, sim2, prob2);
            Console.WriteLine("rows={0}; bands={1}", set.RowsInBand, set.BandsCount);
            Console.WriteLine("sim={0}; wait is {1}; calc={2}", sim1, prob1, LshParamsHelper.Probability(sim1, set));
            Console.WriteLine("sim={0}; wait is {1}; calc={2}", sim2, prob2, LshParamsHelper.Probability(sim2, set));
            Console.ReadLine();
        }
    }
}
