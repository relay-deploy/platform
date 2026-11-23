using Relay.Chassis.Postgres.Sessions;
using Relay.Platform.Application.Persistence;

namespace Relay.Platform.Infrastructure.Persistence;

public sealed class PostgresUnitOfWork(PostgresSession session) : IUnitOfWork
{
    public Task Commit(CancellationToken cancellationToken) => session.Commit(cancellationToken);
}
