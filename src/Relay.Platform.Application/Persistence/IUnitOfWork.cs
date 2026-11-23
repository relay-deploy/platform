namespace Relay.Platform.Application.Persistence;

public interface IUnitOfWork
{
    Task Commit(CancellationToken cancellationToken);
}
