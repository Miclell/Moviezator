namespace Core.Abstractions.Exceptions;

public class InvalidCursorException(string cursorValue, string message)
    : Exception($"Error while processing cursor with value: {cursorValue} and message: {message}");
