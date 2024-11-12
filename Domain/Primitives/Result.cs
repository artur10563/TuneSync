using Domain.Errors;

namespace Domain.Primitives
{
    public class Result
    {
        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None ||
                !isSuccess && error == Error.None)
            {
                throw new ArgumentException("Invalid error", nameof(error));
            }

            IsSuccess = isSuccess;
            Errors.Add(error);
        }
        protected Result(bool isSuccess, List<Error> errors)
        {
            if (isSuccess && errors.Count() > 0 ||
                !isSuccess && errors.Count() == 0)
            {
                throw new ArgumentException("Invalid errors list", nameof(errors));
            }

            IsSuccess = isSuccess;
            Errors = errors;
        }


        public bool IsSuccess { get; set; }
        public bool IsFailure => !IsSuccess;
        public List<Error> Errors { get; set; } = new List<Error>();


        public static Result Success() => new(isSuccess: true, error: Error.None);
        public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

        public static Result Failure(Error error) => new(isSuccess: false, error: error);
        public static Result Failure(List<Error> errors) => new(isSuccess: false, errors: errors);
        public static Result<TValue> Failure<TValue>(Error error) => new(default!, false, error);

        public static implicit operator Result(Error error) => Failure(error);
        public static implicit operator Result(List<Error> errors) => Failure(errors);
        public static Result<TValue> Failure<TValue>(List<Error> errors) => new(default!, false, errors);
    }

    public class Result<TValue> : Result
    {
        protected internal Result(TValue value, bool isSuccess, Error error)
            : base(isSuccess, error)
            => _value = value;

        protected internal Result(TValue value, bool isSuccess, List<Error> errors)
            : base(isSuccess, errors)
            => _value = value;

        private readonly TValue _value;
        public TValue Value => IsSuccess
            ? _value
            : throw new InvalidOperationException("The value of a failure result can not be accessed.");

        public static implicit operator Result<TValue>(TValue value) => Success(value);
        public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);
        public static implicit operator Result<TValue>(List<Error> errors) => Failure<TValue>(errors);


    }
}
