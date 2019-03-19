using System;

namespace LSH.Generic
{
    public abstract class GenericMinHash<T>
    {
        private readonly MinHashParams _hashParams;

        public Func<T[],int>[] HashFn { get; protected set; }

        protected abstract int Converter(T src);

        public GenericMinHash(MinHashParams hashParams) : base()
        {
            _hashParams = hashParams;
            HashFn = new Func<T[], int>[_hashParams.Count];
            for (int i = 0; i < HashFn.Length; i++) HashFn[i] = src => CalcHash(src, _hashParams.GetA(i), _hashParams.GetB(i), _hashParams.Mask);
        }

        private int CalcHash(T[] src, int a, int b, int mask)
        {
            unchecked
            {
                int result = int.MaxValue;
                foreach (var value in src) result = ((result + Converter(value)) * a + b) >> 3;
                return result & mask;
            }
        }

        public int[] InitHashBlock()
        {
            var result = new int[_hashParams.Count];
            for (int i = 0; i < result.Length; i++) result[i] = int.MaxValue;
            return result;
        }

        public void ProcessData(T[] src, ref int[] hashBlock)
        {
            if (hashBlock.Length != HashFn.Length) throw new ArgumentException();
            for (int i = 0; i < hashBlock.Length; i++)
            {
                var value = HashFn[i](src);
                if (value < hashBlock[i]) hashBlock[i] = value;
            }
        }
    }
}