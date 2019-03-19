namespace LSH.Generic
{
    public class NGramMinHash : GenericMinHash<char>
    {
        public NGramMinHash(MinHashParams hashParams) : base(hashParams)
        {
        }

        protected override int Converter(char src) => (int) src;
    }
}