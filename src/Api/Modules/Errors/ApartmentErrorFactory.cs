using Application.Apartments.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class ApartmentErrorFactory
{
    public static ObjectResult ToObjectResult(this ApartmentException error)
    {
        return new ObjectResult(error.Message)
        {
            StatusCode = error switch
            {
                ApartmentAlreadyExistsException => StatusCodes.Status409Conflict,
                ApartmentNotFoundException => StatusCodes.Status404NotFound,
                ApartmentUnhandledException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Apartment error handler is not implemented.")
            }
        };
    }
}
