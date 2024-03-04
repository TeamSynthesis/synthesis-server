using Microsoft.AspNetCore.Diagnostics;
using synthesis.api.Mappings;

namespace synthesis.api.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        await httpContext.Response.WriteAsJsonAsync(
            new GlobalResponse<Exception>(false, "err_internal", errors: [exception.Message])
        );
        return true;
    } 
}