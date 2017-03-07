using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Lib.Parsing
{
    public class TailRec<T>
    {
        private readonly TailRec0 _tailRec;

        internal TailRec(TailRec0 tailRec)
        {
            _tailRec = tailRec;
        }

        public T Run()
        {
            return (T)_tailRec.Run();
        }

        public TailRec<TOut> SelectMany<TOut>(Func<T, TailRec<TOut>> func)
        {
            return new TailRec<TOut>(_tailRec.SelectMany(o => func((T)o)._tailRec));
        }
    }

    public static class TailRec
    {
        public static TailRec<T> Return<T>(T value)
        {
            return new TailRec<T>(TailRec0.Return(value));
        }

        public static TailRec<T> Suspend<T>(Func<T> resume)
        {
            return new TailRec<T>(TailRec0.Suspend(() => resume()));
        }

        public static TailRec<T> SuspendTailRec<T>(Func<TailRec<T>> resume)
        {
            return Suspend(() => Unit.Value).SelectMany(_ => resume());
        }

        public static TailRec<TOut> Select<TIn, TOut>(this TailRec<TIn> tr, Func<TIn, TOut> func)
        {
            return tr.SelectMany(x => Return(func(x)));
        }

        // For LINQ
        public static TailRec<TOut> SelectMany<TIn, TMid, TOut>(this TailRec<TIn> p, Func<TIn, TailRec<TMid>> func, Func<TIn, TMid, TOut> selector)
        {
            return p.SelectMany(x => func(x).Select(y => selector(x, y)));
        }
    }

    internal abstract class TailRec0
    {
        private class RunState
        {
            private readonly bool _isResult;
            private readonly object _result;
            private readonly TailRec0 _tailRec;

            private RunState(bool isResult, object result, TailRec0 tailRec)
            {
                _isResult = isResult;
                _result = result;
                _tailRec = tailRec;
            }

            public static RunState Result(object result)
            {
                return new RunState(true, result, null);
            }

            public static RunState Step(TailRec0 tailRec)
            {
                return new RunState(false, null, tailRec);
            }

            public bool MatchResult(out object result)
            {
                result = _result;
                return _isResult;
            }

            public bool MatchStep(out TailRec0 tailRec)
            {
                tailRec = _tailRec;
                return !_isResult;
            }
        }

        public object Run()
        {
            var current = this;

            while (true)
            {
                var state = current.Match(
                    returnCase: a => RunState.Result(a),
                    suspendCase: r => RunState.Result(r()),
                    selectManyCase: (x, f) => x.Match(
                        returnCase: a => RunState.Step(f(a)),
                        suspendCase: r => RunState.Step(f(r())),
                        selectManyCase: (y, g) => RunState.Step(y.SelectMany(a => g(a).SelectMany(f)))
                    )
                );

                object value;
                TailRec0 next;

                if (state.MatchResult(out value))
                {
                    return value;
                }
                else if (state.MatchStep(out next))
                {
                    current = next;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public static TailRec0 Return(object value)
        {
            return new ReturnTailRec(value);
        }

        public static TailRec0 Suspend(Func<object> resume)
        {
            return new SuspendTailRec(resume);
        }

        public TailRec0 SelectMany(Func<object, TailRec0> func)
        {
            return new SelectManyTailRec(this, func);
        }

        #region Matching
        public TOut Match<TOut>(Func<object, TOut> returnCase, Func<Func<object>, TOut> suspendCase, Func<TailRec0, Func<object, TailRec0>, TOut> selectManyCase)
        {
            return MatchCore(new MatchParams<TOut>(returnCase, suspendCase, selectManyCase));
        }

        protected abstract TOut MatchCore<TOut>(MatchParams<TOut> parameters);

        protected class MatchParams<TOut>
        {
            public Func<object, TOut> ReturnCase { get; }
            public Func<Func<object>, TOut> SuspendCase { get; }
            public Func<TailRec0, Func<object, TailRec0>, TOut> SelectManyCase { get; }

            public MatchParams(Func<object, TOut> returnCase, Func<Func<object>, TOut> suspendCase, Func<TailRec0, Func<object, TailRec0>, TOut> selectManyCase)
            {
                ReturnCase = returnCase;
                SuspendCase = suspendCase;
                SelectManyCase = selectManyCase;
            }
        }
        #endregion

        #region Case Classes
        private class ReturnTailRec : TailRec0
        {
            private readonly object _value;

            public ReturnTailRec(object value)
            {
                _value = value;
            }

            protected override TOut MatchCore<TOut>(MatchParams<TOut> parameters)
            {
                return parameters.ReturnCase(_value);
            }
        }

        private class SuspendTailRec : TailRec0
        {
            private readonly Func<object> _resume;

            public SuspendTailRec(Func<object> resume)
            {
                _resume = resume;
            }

            protected override TOut MatchCore<TOut>(MatchParams<TOut> parameters)
            {
                return parameters.SuspendCase(_resume);
            }
        }

        private class SelectManyTailRec : TailRec0
        {
            private readonly TailRec0 _subject;
            private readonly Func<object, TailRec0> _func;

            public SelectManyTailRec(TailRec0 subject, Func<object, TailRec0> func)
            {
                _subject = subject;
                _func = func;
            }

            protected override TOut MatchCore<TOut>(MatchParams<TOut> parameters)
            {
                return parameters.SelectManyCase(_subject, _func);
            }
        }
        #endregion
    }
}
