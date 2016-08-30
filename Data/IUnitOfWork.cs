using System;
using System.Data.Objects;

namespace Iata.IS.Data
{
  /// <summary>
  /// IUnitOfWork consists of the method Commit in order to save changes for all repositories using the <see cref="ObjectContext"/>.
  /// </summary>
  public interface IUnitOfWork : IDisposable
  {
    IObjectContext Context { get; }

    /// <summary>
    /// Commits changes for all the repositories sharing the same <see cref="ObjectContext"/>.
    /// </summary>
    void Commit();
  }
}