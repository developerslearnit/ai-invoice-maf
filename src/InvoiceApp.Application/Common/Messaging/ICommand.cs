using InvoiceApp.Domain.Common;

namespace InvoiceApp.Application.Common.Messaging;

public interface ICommand<TResponse>
{
}

public interface ICommand : ICommand<Result>
{
}

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Result>
    where TCommand : ICommand
{
}
