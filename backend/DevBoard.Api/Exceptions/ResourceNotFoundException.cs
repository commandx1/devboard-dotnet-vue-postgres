namespace DevBoard.Api.Exceptions;

public sealed class ResourceNotFoundException(string resource, object id)
    : Exception($"{resource} with id {id} not found");
