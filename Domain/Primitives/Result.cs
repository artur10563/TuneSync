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
			Error = error;
		}

		public bool IsSuccess { get; set; }
		public bool IsFailure => !IsSuccess;
		public Error Error { get; set; } // Replace with errors


		public static Result Success() => new(isSuccess: true, error: Error.None);
		public static Result Failure(Error error) => new(isSuccess: false, error: error);

		public static implicit operator Result(Error error) => Failure(error);
		public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
		public static Result<TValue> Failure<TValue>(Error error) => new(default!, false, error);
	}

	public class Result<TValue> : Result
	{
		protected internal Result(TValue value, bool isSuccess, Error error)
			: base(isSuccess, error)
			=> _value = value;

		private readonly TValue _value;
		public TValue Value => IsSuccess
			? _value
			: throw new InvalidOperationException("The value of a failure result can not be accessed.");

		public static implicit operator Result<TValue>(TValue value) => Success(value);
		public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);

	}
}
