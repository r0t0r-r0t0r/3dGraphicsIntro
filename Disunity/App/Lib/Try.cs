using System;

namespace Disunity.App.Lib
{
    public class Try<TException, TValue>
        where TException : Exception
    {
        private readonly bool _success;
        private readonly TException _exception;
        private readonly TValue _value;

        internal Try(Either<TException, TValue> either)
        {
            var result = either.Match(
                leftCase: exception => new { Success = false, Exception = exception, Value = default(TValue) },
                rightCase: value => new { Success = true, Exception = default(TException), Value = value }
            );

            _success = result.Success;
            _exception = result.Exception;
            _value = result.Value;
        }

        public TValue GetValue()
        {
            if (_success)
                return _value;
            else
                throw _exception;
        }
    }

    public static class Try
    {
        public static Try<TException, TValue> FromEither<TException, TValue>(Either<TException, TValue> either)
            where TException : Exception
        {
            return new Try<TException, TValue>(either);
        }
    }
}
