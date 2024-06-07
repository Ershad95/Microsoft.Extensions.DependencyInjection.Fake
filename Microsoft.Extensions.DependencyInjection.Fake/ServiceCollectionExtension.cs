using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ServiceCollector.Fake.Configuration;

public static class ServiceCollectionExtension
{
    private static readonly IFixture AutoFixture;

    static ServiceCollectionExtension()
    {
        AutoFixture = new Fixture()
            .Customize(new AutoNSubstituteCustomization()
            {
                ConfigureMembers = true
            });
    }

    public static IServiceCollection Fake<TService>(
        this IServiceCollection serviceCollection,
        Action<ServiceConfigExtension.FakeConfiguration<TService>>? action = null,
        string currentEnvironment = "Development",
        string targetEnvironment = "Development")
        where TService : class
    {
        var obj = AutoFixture.Create<TService>();
        var fakeConfiguration = new ServiceConfigExtension.FakeConfiguration<TService>(obj);
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
        Action<ServiceConfigExtension.FakeConfigurationWithMultiEnvironment<TService>> action,
        string currentEnvironment = "Development")
        where TService : class
    {
        var fakeConfiguration =
            new ServiceConfigExtension.FakeConfigurationWithMultiEnvironment<TService>(currentEnvironment);
        action(fakeConfiguration);

        var service = fakeConfiguration.Services[currentEnvironment];
        if (service != null)
        {
            serviceCollection.Replace(new ServiceDescriptor(typeof(TService), service));
        }

        return serviceCollection;
    }
}