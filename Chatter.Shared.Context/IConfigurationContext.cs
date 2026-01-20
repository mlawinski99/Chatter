using Chatter.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chatter.Shared.Context;

public interface IConfigurationContext
{
    DbSet<ConfigurationData> ConfigurationData { get; set; } 
}