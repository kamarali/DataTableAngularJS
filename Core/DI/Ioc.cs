using System.Reflection;
using Castle.Core;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using System;
using log4net;
using Castle.MicroKernel.Releasers;

namespace Iata.IS.Core.DI
{
  /// <summary>
  /// Inversion of Control class. This class is wrapper over WindsorCastle 
  /// IOC. It is use to resolve the object creation. 
  /// </summary>
  /// <remarks>
  /// Configuration is taken from the configuration file.
  /// </remarks>
  public static class Ioc
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static IWindsorContainer _container;

    /// <summary>
    /// Initializes the <see cref="Ioc"/> class. The client of this class MUST use this method to initialize the container.
    /// E.g. In a web application this method must be called in Application_Start in Global.ascx.
    /// </summary>
    public static void Initialize()
    {
      // Check if already initialized.
      if (_container != null)
      {
        Logger.Debug("IOC container already initialized.");
        return;
      }

      using (var config = new ConfigResource("castle"))
      {
        _container = new WindsorContainer(new XmlInterpreter(config));
        _container.Kernel.ReleasePolicy = new NoTrackingReleasePolicy();
      }
      Logger.Info("IOC container initialized.");
    }

    /// <summary>
    /// Initializes the container with the container that has been passed in. Useful for unit testing.
    /// </summary>
    /// <param name="container"></param>
    public static void Initialize(IWindsorContainer container)
    {
      // Check if already initialized.
      if (_container != null)
      {
        Logger.Debug("IOC container already initialized.");
        return;
      }

      _container = container;
      Logger.Info("IOC container initialized.");
    }

    /// <summary>
    /// Resolves the specified type.
    /// </summary>
    /// <typeparam name="T">Type of object to be resolved.</typeparam>
    /// <param name="type">The type of the component, service specified in configuration.</param>
    /// <returns></returns>
    public static T Resolve<T>(Type type)
    {
      if (Logger.IsDebugEnabled)
      {
        Logger.DebugFormat("Resolving type: [{0}]", type.FullName);
      }

      return (T)_container.Resolve(type);
    }

    /// <summary>
    /// Resolves the specific type.
    /// </summary>
    /// <typeparam name="T">Type of the object to be resolved.</typeparam>
    /// <returns>The resolved object instance.</returns>
    public static T Resolve<T>()
    {
      if (Logger.IsDebugEnabled)
      {
        Logger.DebugFormat("Resolving type: [{0}]", typeof(T).FullName);
      }

      return _container.Resolve<T>();
    }

    /// <summary>
    /// Resolves the specified key.
    /// </summary>
    /// <typeparam name="T">Type of the object to be resolved.</typeparam>
    /// <param name="key">The key specified in configuration.</param>
    /// <returns>The resolved object instance.</returns>
    public static T Resolve<T>(string key)
    {
      return _container.Resolve<T>(key);
    }
    
    /// <summary>
    /// Releases an instance from the container.
    /// </summary>
    /// <param name="instance">The object instance to be released.</param>
    public static void Release(object instance)
    {
      _container.Release(instance);
    } 

    /// <summary>
    /// Adds the component life style.
    /// </summary>
    /// <param name="type">The type to be added.</param>
    /// <param name="lifestyleType">The default Lifestyle is considered as Transient.</param>
    /// <remarks>
    /// Full name of the type is used as key while adding the component.
    /// The Lifestyle is considered as Transient.
    /// Note: May need to modify the behavior of all components getting added with Transient lifestyle.
    /// </remarks>
    public static void Register(Type type, LifestyleType lifestyleType = LifestyleType.Transient)
    {
      if (type == null)
      {
        throw new ArgumentNullException("type", "type parameter for Ioc.Register cannot be null.");
      }

      _container.Register(Component.For(type).Named(type.FullName).LifeStyle.Is(lifestyleType));
    }
  }
}
