using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ViaPadelClub.Presentation.WebAPI.Common;


[ApiController, Route("api")]
public abstract class EndpointBase : ControllerBase;

// Nested static classess allows for - ApiEndpoint.WithoutRequest.AndResult .....
public static class ApiEndpoint
{
    public static class WithoutRequest
    {
        public abstract class AndResult<TResult> : EndpointBase
            where TResult : IResult
        {
            public abstract Task<TResult> HandleAsync();
        }
        
        public abstract class AndResults<TResult1, TResult2> : EndpointBase
            where TResult1 : IResult
            where TResult2 : IResult
        {
            public abstract Task<Results<TResult1, TResult2>> HandleAsync();
        }
        public abstract class AndResults<TResult1, TResult2, TResult3> : EndpointBase
            where TResult1 : IResult
            where TResult2 : IResult
            where TResult3 : IResult
        {
            public abstract Task<Results<TResult1, TResult2, TResult3>> HandleAsync();
        }
    }
    
    public static class WithRequest<TRequest>
    {
        public abstract class AndResult<TResult> : EndpointBase
            where TResult : IResult
        {
            public abstract Task<TResult> HandleAsync(TRequest request);
        }

        public abstract class AndResults<TResult1, TResult2> : EndpointBase
            where TResult1 : IResult
            where TResult2 : IResult
        {
            public abstract Task<Results<TResult1, TResult2>> HandleAsync(TRequest request);
        }

        public abstract class AndResults<TResult1, TResult2, TResult3> : EndpointBase
            where TResult1 : IResult
            where TResult2 : IResult
            where TResult3 : IResult
        {
            public abstract Task<Results<TResult1, TResult2, TResult3>> HandleAsync(TRequest request);
        }
    }
}