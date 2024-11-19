using System;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
sealed class LimitationAttribute(string message) : Attribute
{
    public string Message { get; } = message;
}