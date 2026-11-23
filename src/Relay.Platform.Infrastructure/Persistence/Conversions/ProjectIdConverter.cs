using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Infrastructure.Persistence.Conversions;

public sealed class ProjectIdConverter() : ValueConverter<ProjectId, Guid>(id => id.Value, value => new ProjectId(value));
