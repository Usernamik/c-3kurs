using Domain.Apartments;

namespace Application.Apartments.Exceptions;

public abstract class ApartmentException(ApartmentId apartmentId, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public ApartmentId ApartmentId { get; } = apartmentId;
}

public class ApartmentAlreadyExistsException(ApartmentId apartmentId)
    : ApartmentException(apartmentId, $"Apartment already exists under id {apartmentId}");

public class ApartmentNotFoundException(ApartmentId apartmentId)
    : ApartmentException(apartmentId, $"Apartment not found under id {apartmentId}");

public class ApartmentUnhandledException(ApartmentId apartmentId, Exception? innerException = null)
    : ApartmentException(apartmentId, $"Unhandled exception under id {apartmentId}", innerException);
