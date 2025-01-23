using System.Collections;
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
            if (isSuccess && errors.Any(x => x != Error.None) ||
                !isSuccess && errors.Count == 0)
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
        public static PaginatedResult<TValue> Success<TValue>(TValue items, int pageNumber, int totalItems) where TValue : IEnumerable 
            => new(items, true, Error.None, pageNumber, totalItems);

        public static Result Failure(Error error) => new(isSuccess: false, error: error);
        public static Result Failure(List<Error> errors) => new(isSuccess: false, errors: errors);
        public static Result<TValue> Failure<TValue>(Error error) => new(default!, false, error);
        
        //TotalItems just to change signature 
        public static PaginatedResult<TValue> Failure<TValue>(Error error, int totalItems = 0) where TValue : IEnumerable 
            => new(default!, false, error, totalCount: totalItems);
        public static PaginatedResult<TValue> Failure<TValue>(List<Error> errors, int totalItems = 0) where TValue : IEnumerable 
            => new(default!, false, errors, totalCount: totalItems);

        public static implicit operator Result(Error error) => Failure(error);
        public static implicit operator Result(List<Error> errors) => Failure(errors);
        public static Result<TValue> Failure<TValue>(List<Error> errors) => new(default!, false, errors);
        
    }

    public class Result<TValue> : Result
    {
        protected internal Result(TValue value, bool isSuccess, Error error)
            : this(value, isSuccess, [error])
        {
        }

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

    public class PaginatedResult<TValue> : Result<TValue> where TValue : IEnumerable
    {
        protected internal PaginatedResult(TValue value, bool isSuccess, Error error, int page = 1, int totalCount = 0)
            : this(value, isSuccess, [error], page, totalCount)
        {
        }

        protected internal PaginatedResult(TValue value, bool isSuccess, List<Error> errors, int page = 1, int totalCount = 0) 
            : base(value, isSuccess, errors)
        {
            if(page <= 0) throw new ArgumentException("Invalid page number", nameof(page));
            Page = page;
            TotalCount = totalCount;
        }
        
        public int Page { get; set; }
        public int PageSize { get; set; } = 25;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        
        public static implicit operator PaginatedResult<TValue>((TValue value, int page, int totalCount) paginatedData ) 
            => new(paginatedData.value, true, Error.None, paginatedData.page, paginatedData.totalCount);
        public static implicit operator PaginatedResult<TValue>((List<Error> errors, int totalItems) failure ) => Failure<TValue>(failure.errors, failure.totalItems);
        public static implicit operator PaginatedResult<TValue>((Error error, int totalItems) failure ) => Failure<TValue>(failure.error, failure.totalItems);

        public PaginatedResponse<TValue> ToPaginatedResponse()
        {
            var pageInfo = new PageInfo(Page, PageSize, TotalCount, TotalPages);
            return new PaginatedResponse<TValue>(Items: Value, PageInfo: pageInfo);
        }
    }
    
    public sealed record PaginatedResponse<TValue>(TValue Items, PageInfo PageInfo) where TValue : IEnumerable;

    public sealed record PageInfo(int Page, int PageSize, int TotalCount, int TotalPages);
}
