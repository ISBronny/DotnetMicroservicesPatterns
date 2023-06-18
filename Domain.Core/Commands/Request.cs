using FluentValidation.Results;
using MediatR;

namespace Domain.Core.Commands;

// public abstract class Request : IRequest
// {
// 	public ValidationResult ValidationResult { get; set; } = new();
//
// 	public abstract bool IsValid();
// }
//
// public abstract class Response<T>
// {
// 	public ValidationResult ValidationResult { get; set; } = new();
//
// 	public T? Entity { get; set; }
// }