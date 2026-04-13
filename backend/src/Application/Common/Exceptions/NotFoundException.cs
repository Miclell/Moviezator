namespace Application.Common.Exceptions;

public sealed class NotFoundException(string entity, Guid id)
    : Exception($"{entity} with id '{id}' was not found.");
