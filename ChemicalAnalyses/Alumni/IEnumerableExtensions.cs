using System.Collections.Generic;
using System.Linq;

namespace ChemicalAnalyses.Alumni
{
    public static class IEnumerableExtensions
    {
        //Taken from https://stackoverflow.com/questions/419019/split-list-into-sublists-with-linq
        //Response of Sam Saffon
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            int pos = 0;
            while (source.Skip(pos).Any())
            {
                yield return source.Skip(pos).Take(chunksize);
                pos += chunksize;
            }
        }
    }
}