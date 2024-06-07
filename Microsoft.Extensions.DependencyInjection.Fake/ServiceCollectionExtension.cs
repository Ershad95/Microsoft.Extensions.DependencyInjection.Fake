using Microsoft.Extensions.DependencyInjection.Extensions;
using ServiceCollector.Fake.Configuration;

namespace Microsoft.Extensions.DependencyInjection.Fake;

public static class ServiceCollectionExtension
{
    public static IServiceCollection Fake<TService>(
        this IServiceCollection serviceCollection,
        Action<FakeConfiguration<TService>>? action = null,
        string currentEnvironment = "Development",
        string targetEnvironment = "Development")
        where TService : class
    {
        var obj = BaseGenerator.Create<TService>();
        var fakeConfiguration = new FakeConfiguration<TService>(obj);
        action?.Invoke(fakeConfiguration);

        if (string.Equals(currentEnvironment, targetEnvironment, StringComparison.InvariantCultureIgnoreCase))
        {
            serviceCollection
                .Replace(new ServiceDescriptor(typeof(TService), fakeConfiguration.Service));
        }

        return serviceCollection;
    }
    
    public static IServiceCollection FakeInMultiEnvironments<TService>(
        this IServiceCollection serviceCollection,
        Action<FakeConfigurationWithMultiEnvironment<TService>> action,
        string currentEnvironment = "Development")
        where TService : class
    {
        var fakeConfiguration = new FakeConfigurationWithMultiEnvironment<TService>(currentEnvironment);
        action(fakeConfiguration);

        var service = fakeConfiguration.Services[currentEnvironment];
        if (service != null)
        {
            serviceCollection.Replace(new ServiceDescriptor(typeof(TService), service));
        }

        return serviceCollection;
    }
}