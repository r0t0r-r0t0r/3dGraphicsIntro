using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Lib
{
    public class Option<T> : IEnumerable<T>
    {
        private readonly bool _hasValue;
        private readonly T _value;

        public Option(bool hasValue, T value)
        {
            _hasValue = hasValue;
            _value = value;
        }

        public T OrElse(T defaultValue)
        {
            return _hasValue ? _value : defaultValue;
        }

        public Option<TOut> Select<TOut>(Func<T, TOut> selector)
        {
            return _hasValue ? Option.Some(selector(_value)) : Option.None<TOut>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_hasValue)
                yield return _value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class Option
    {
        public static Option<T> Some<T>(T value)
        {
            return new Option<T>(true, value);
        }

        public static Option<T> None<T>()
        {
            return new Option<T>(false, default(T));
        }
    }

    public static class EnumerableOption
    {
        public static Option<TSource> LastOption<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");
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
