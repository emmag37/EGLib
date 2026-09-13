using System;
using System.Collections.Generic;


/// <summary>
/// Static registry mapping service interface types to a single implementation instance each,
/// providing simple global access to shared services (audio, save system, etc.) without hard
/// dependencies between classes.
/// </summary>
public static class ServiceLocator
{
    // ==================================================
    // Private Fields
    // ==================================================
    private static readonly Dictionary<Type, object> services = new();  // <key: interface, value: implementation>

    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Registers the given instance as the implementation for <typeparamref name="TService"/>.
	/// Adds a new entry if none exists for that type, or replaces the existing one if it does.
	/// </summary>
	/// <typeparam name="TService">The service type (typically an interface) to register under.</typeparam>
	/// <param name="service">The implementation instance to register. Must not be null.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> is null.</exception>
    public static void Register<TService>(TService service)
    {
        if (service == null)
            throw new ArgumentNullException(nameof(service));

        services[typeof(TService)] = service;
    }

    /// <summary>
	/// Retrieves the registered implementation for <typeparamref name="TService"/>.
	/// </summary>
	/// <typeparam name="TService">The service type to retrieve.</typeparam>
	/// <returns>The registered implementation instance.</returns>
	/// <exception cref="InvalidOperationException">Thrown if no service is registered for <typeparamref name="TService"/>.</exception>
    public static TService Get<TService>()
    {
        if (!services.TryGetValue(typeof(TService), out object foundService))
            throw new InvalidOperationException($"[ServiceLocator] No service registered for {typeof(TService)}");

        return (TService)foundService;
    }

    /// <summary>
	/// Unregisters the implementation currently registered for <typeparamref name="TService"/>,
	/// if any. Removal is by type only.
	/// </summary>
	/// <typeparam name="TService">The service type to unregister.</typeparam>
	/// <returns><c>true</c> if a service was registered for <typeparamref name="TService"/> and removed, <c>false</c> otherwise.</returns>
    public static bool Remove<TService>() => services.Remove(typeof(TService));
}
