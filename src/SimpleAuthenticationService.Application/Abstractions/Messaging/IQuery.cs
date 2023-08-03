using MediatR;

namespace SimpleAuthenticationService.Application.Abstractions.Messaging;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}