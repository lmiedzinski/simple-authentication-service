using MediatR;

namespace SimpleAuthenticationService.Application.Abstractions.Messaging;

public interface ICommand : IRequest, ICommandBase
{
}

public interface ICommand<out TResponse> : IRequest<TResponse>, ICommandBase
{
}

public interface ICommandBase
{
}