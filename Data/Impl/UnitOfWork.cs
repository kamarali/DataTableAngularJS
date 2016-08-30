using System;
using System.Data.Objects;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Iata.IS.Core.DI;

namespace Iata.IS.Data.Impl
{
  /// <summary>
  /// Enables saving the changes based on the same <see cref="ObjectContext"/>. Implements <see cref="IDisposable"/>.
  /// </summary>
  public class UnitOfWork : IUnitOfWork
  {
    /// <summary>
    /// The Context allows for any operations to be directly on the <see cref="ObjectContext"/>.
    /// This is a read-only property.
    /// </summary>
    public IObjectContext Context { get; private set; }

    /// <summary>
    /// Constructor. Expects the <see cref="ObjectContext"/> to be injected.
    /// </summary>
    /// <param name="context"></param>
    public UnitOfWork(IObjectContext context)
    {
      Contract.Requires(context != null, "Injected context should not be NULL.");
      Context = context;
    }

    /// <summary>
    /// Commits changes for all the repositories sharing the same <see cref="ObjectContext"/>.
    /// </summary>
    public void Commit()
    {
      // Save the changes in the context.
      Context.SaveChanges();
    }

    /// <summary>
    /// Retrieves the context from the container and then saves the changes for that context.
    /// </summary>
    public static void CommitDefault()
    {
      // Note: Do not create a new container.
      // IWindsorContainer container = new WindsorContainer(new XmlInterpreter(new ConfigResource("castle")));

      // Resolve the IObjectContext - this will give the context instance that is per web-request.
      var context = Ioc.Resolve<IUnitOfWork>();

      // If we get a context for the current web request then save the changes.
      if (context != null)
      {
        context.Commit();
      }
    }

    /// <summary>
    /// Creates a new context and wraps the context in a <see cref="IUnitOfWork"/>. Returns this <see cref="IUnitOfWork"/> instance.
    /// </summary>
    /// <returns></returns>
    public static IUnitOfWork CreateNew()
    {
      // Create a new instance of the Windsor container. We do not need the same instance returned by the default container.
      IWindsorContainer container = new WindsorContainer(new XmlInterpreter(new ConfigResource("castle")));

      // Create a fresh unit of work and return.
      var unitOfWork = (IUnitOfWork) container.Resolve(typeof(IUnitOfWork));

      return unitOfWork;
    }

    /// <summary>
    /// Disposes the enclosed context.
    /// </summary>
    public void Dispose()
    {
      if (Context != null)
      {
        Context.Dispose();
      }

      GC.SuppressFinalize(this);
    }
  }
}