using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Disunity.Data.Common;
using Disunity.Parsing;

namespace ParserTests
{
    public delegate bool ParsingResultPredicate<T>(Either<Exception, Tuple<T, ParserState>> result);

    public static class ParserAssert
    {
        public static void That<T>(Either<Exception, Tuple<T, ParserState>> parsingResult, Func<ParsingResultPredicates<T>, ParsingResultPredicate<T>> predicateFactory)
        {
            Assert.That(predicateFactory(new ParsingResultPredicates<T>())(parsingResult));
        }
    }

    public static class ParserAssertParserExtensions
    {
        public static Either<Exception, Tuple<T, ParserState>> OnInput<T>(this Parser<T> p, string input)
        {
            return p.Run(new ParserState(0, input, "unit test")).Run();
        }

        public static Either<Exception, Tuple<T, ParserState>> OnAnyInput<T>(this Parser<T> p)
        {
            const string input = "Ё(!@~tygZ732";
            return p.Run(new ParserState(0, input, "unit test")).Run();
        }
    }

    public class ParsingResultPredicates<T>
    {
        internal ParsingResultPredicates()
        {
        }

        public ParsingResultPredicate<T> Consumes(string str)
        {
            return result =>
            {
                return result.Select(r => 
                {
                    var state = r.Item2;
                    return state.Position == str.Length && state.String.StartsWith(str);
                }).GetOrElse(() => false);
            };
        }

        public ParsingResultPredicate<T> ConsumesNothing()
        {
            return Consumes("");
        }

        public ParsingResultPredicate<T> Produces(T value)
        {
            return Produces(value, ComparingBy.Value<T>());
        }

        public ParsingResultPredicate<T> Produces(T value, Func<T, T, bool> equaler)
        {
            return result =>
            {
                return result.Select(r =>
                {
                    return equaler(value, r.Item1);
                }).GetOrElse(() => false);
            };
        }

        public ParsingResultPredicate<T> Fails()
        {
            return result =>
            {
                return result.Select(_ => false).GetOrElse(() => true);
            };
        }
    }

    public static class ParsingResultPredicates
    {
        public static ParsingResultPredicate<T> And<T>(this ParsingResultPredicate<T> p1, ParsingResultPredicate<T> p2)
        {
            return result => p1(result) && p2(result);
        }

        public static ParsingResultPredicate<string> ConsumesAndProduces(this ParsingResultPredicates<string> p, string str)
        {
            return p.Consumes(str).And(p.Produces(str));
        }
    }

    public static class ComparingBy
    {
        public static Func<T, T, bool> Value<T>()
        {
            return (x1, x2) => Object.Equals(x1, x2);
        }

        public static Func<T, T, bool> Reference<T>()
        {
            return (x1, x2) => Object.ReferenceEquals(x1, x2);
        }

        public static Func<IEnumerable<T>, IEnumerable<T>, bool> Items<T>()
        {
            return (x1, x2) => Enumerable.SequenceEqual(x1, x2);
        }
    }

    public class ValueWithEqualer<T>
    {
        internal ValueWithEqualer(T value, Func<T, T, bool> equaler)
        {
            Value = value;
            Equaler = equaler;
        }

        public T Value { get; }
        public Func<T, T, bool> Equaler { get; }
    }
}
