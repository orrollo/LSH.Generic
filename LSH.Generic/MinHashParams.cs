using System;

namespace LSH.Generic
{
    public class MinHashParams
    {
        public int Count { get; protected set; }
        public int Bits { get; protected set; }
        public int Mask { get; protected set; }

        protected int Gcd(int a, int b)
        {
            if (a < b) return Gcd(b, a);

            while (b != 0)
            {
                var c = a % b;
                a = b;
                b = c;
            }

            return a;
        }

        public int GetA(int index) => cA[index];
        public int GetB(int index) => cB[index];

        public MinHashParams(int count, int bits) : base()
        {
            Initialize(count, bits, MakeRandomPair, new Random());
        }

        private void MakeRandomPair(object context, int index, out int a, out int b)
        {
            if (!(context is Random rnd)) throw new ArgumentException();
            var stop = false;
            a = 0;
            b = 0;
            while (!stop)
            {
                a = rnd.Next(100, 65536) * 8 + 5;
                b = rnd.Next(100, 65536) * 2 + 1;
                stop = Gcd(a, b) == 1;
            }
        }

        protected int[] cA, cB;

        protected delegate void GetterDelegate(object context, int index, out int a, out int b);

        private void Initialize(int count, int bits, GetterDelegate getter, object context)
        {
            Count = count;
            Bits = bits;
            Mask = (1 << bits) - 1;
            cA = new int[count];
            cB = new int[count];
            for (int i = 0; i < count; i++) getter(context, i, out cA[i], out cB[i]);
        }

        public MinHashParams(int[] coefA, int[] coefB, int bits)
        {
            if (coefA.Length != coefB.Length || coefA.Length == 0) throw new ArgumentException();

            void Setter(object context, int index, out int a, out int b)
            {
                a = coefA[index];
                b = coefB[index];
            }

            Initialize(coefA.Length, bits, Setter, null);
        }
    }
}