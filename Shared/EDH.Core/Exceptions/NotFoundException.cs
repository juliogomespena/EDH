using System;

namespace EDH.Core.Exceptions;

/// <summary>
/// Exception that is thrown when a requested entity is not found.
/// </summary>
public sealed class NotFoundException : Exception
{
	/// <summary>
	/// Gets the name of the entity that was not found.
	/// </summary>
	public string EntityName { get; }

	/// <summary>
	/// Gets the identifier that was used to search for the entity.
	/// </summary>
	public object EntityId { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="NotFoundException"/> class.
	/// </summary>
	public NotFoundException() : base("The requested entity was not found.")
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="NotFoundException"/> class with a custom message.
	/// </summary>
	/// <param name="message">The custom error message.</param>
	public NotFoundException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="NotFoundException"/> class with a custom message and inner exception.
	/// </summary>
	/// <param name="message">The custom error message.</param>
	/// <param name="innerException">The inner exception.</param>
	public NotFoundException(string message, Exception innerException) : base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="NotFoundException"/> class with entity information.
	/// </summary>
	/// <param name="entityName">Name of the entity that was not found.</param>
	/// <param name="entityId">Identifier used to search for the entity.</param>
	public NotFoundException(string entityName, object entityId)
		: base($"Entity \"{entityName}\" with id \"{entityId}\" was not found.")
	{
		EntityName = entityName;
		EntityId = entityId;
	}
}