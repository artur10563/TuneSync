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
        public static PaginatedResult<TValue> Success<TValue>(TValue items, int pageNumber, int pageSize, int totalItems) where TValue : IEnumerable 
            => new(items, true, Error.None, pageNumber, pageSize, totalItems);

        public static Result Failure(Error error) => new(isSuccess: false, error: error);
        public static Result Failure(List<Error> errors) => new(isSuccess: false, errors: errors);
        public static Result<TValue> Failure<TValue>(Error error) => new(default!, false, error);
        
        //TotalItems just to change signature 
        public static PaginatedResult<TValue> Failure<TValue>(Error error, int page = 0) where TValue : IEnumerable 
            => new(default!, false, error, page: page);
        public static PaginatedResult<TValue> Failure<TValue>(List<Error> errors, int page = 0) where TValue : IEnumerable 
            => new(default!, false, errors, page: page);

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
            ? _value : default; // serialization will break for bg job if we throw. Custom serialization might fix it
            // : throw new InvalidOperationException("The value of a failure result can not be accessed.");

        public static implicit operator Result<TValue>(TValue value) => Success(value);
        public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);
        public static implicit operator Result<TValue>(List<Error> errors) => Failure<TValue>(errors);


    }

    public class PaginatedResult<TValue> : Result<TValue> where TValue : IEnumerable
    {
        protected internal PaginatedResult(
            TValue value, 
            bool isSuccess, 
            Error error, 
            int page = GlobalVariables.PaginationConstants.PageMin, 
            int pageSize = GlobalVariables.PaginationConstants.PageSize,
            int totalCount = 0,
            Dictionary<string, object>? metadata = null)
            : this(value, isSuccess, [error], page, pageSize, totalCount, metadata)
        {
        }

        protected internal PaginatedResult(
            TValue value, 
            bool isSuccess, 
            List<Error> errors, 
            int page = GlobalVariables.PaginationConstants.PageMin, 
            int pageSize = GlobalVariables.PaginationConstants.PageSize,
            int totalCount = 0,
            Dictionary<string, object>? metadata = null) 
            : base(value, isSuccess, errors)
        {
            if(page <= 0) throw new ArgumentException("Invalid page number", nameof(page));
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
            Metadata = metadata;
        }
        
        public PaginatedResult(TValue value, int page, int pageSize, int totalCount, Dictionary<string, object>? metadata)
            : this(value, true, Error.None, page, pageSize, totalCount, metadata)
        { }
        
        public PaginatedResult(TValue value, int page, int totalCount, Dictionary<string, object>? metadata)
            : this(value, true, Error.None, page, GlobalVariables.PaginationConstants.PageSize, totalCount, metadata)
        { }
        
        
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public Dictionary<string, object>? Metadata { get; set; } // Any other info about page
        
        public static implicit operator PaginatedResult<TValue>((TValue value, int page, int pageSize, int totalCount, Dictionary<string, object>? metadata) paginatedData ) 
            => new(paginatedData.value, true, Error.None, paginatedData.page, paginatedData.pageSize, paginatedData.totalCount, paginatedData.metadata);
        
        public static implicit operator PaginatedResult<TValue>((TValue value, int page, int pageSize, int totalCount) paginatedData ) 
            => new(paginatedData.value, true, Error.None, paginatedData.page, paginatedData.pageSize, paginatedData.totalCount);
        
        public static implicit operator PaginatedResult<TValue>((List<Error> errors, int page) failure ) => Failure<TValue>(failure.errors, failure.page);
        public static implicit operator PaginatedResult<TValue>((Error error, int page) failure ) => Failure<TValue>(failure.error, failure.page);

        public PaginatedResponse<TValue> ToPaginatedResponse()
        {
            var pageInfo = new PageInfo(Page, PageSize, TotalCount, TotalPages, Metadata);
            return new PaginatedResponse<TValue>(Items: Value, PageInfo: pageInfo);
        }
    }
    
    public sealed record PaginatedResponse<TValue>(TValue Items, PageInfo PageInfo) where TValue : IEnumerable;

    public sealed record PageInfo(int Page, int PageSize, int TotalCount, int TotalPages, Dictionary<string, object>? Metadata = null)
    {
        public static PageInfo Create(int page, int pageSize, int totalCount)
        {
            return new PageInfo(page, pageSize, totalCount, (int)Math.Ceiling((double)totalCount / pageSize));
        }
    };
}
