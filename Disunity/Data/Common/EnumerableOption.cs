using System;
using System.Collections.Generic;

namespace Disunity.Data.Common
{
    public static class EnumerableOption
    {
        public static Option<TSource> LastOption<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IList<TSource> list = source as IList<TSource>;
            if (list != null)
            {
                int count = list.Count;
                if (count > 0) return Option.Some(list[count - 1]);
            }
            else
            {
                using (IEnumerator<TSource> e = source.GetEnumerator())
                {
                    if (e.MoveNext())
                    {
                        TSource result;
                        do
                        {
                            result = e.Current;
                        } while (e.MoveNext());
                        return Option.Some(result);
                    }
                }
            }
            return Option.None<TSource>();
        }
    }
}