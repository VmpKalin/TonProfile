using Argentics.Backend.Profile.Core.Abstractions;
using Argentics.Backend.Profile.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Argentics.Backend.Profile.Core;

public static class DependencyInjection
{
    public static void AddProfileCore(this IServiceCollection services)
    {
        services.AddScoped<IProfileService, ProfileService>();
    }
}