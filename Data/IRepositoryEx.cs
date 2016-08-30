using Iata.IS.Model.Base;

namespace Iata.IS.Data
{
  public interface IRepositoryEx<E, B> : IRepository<E> 
      where B: ModelBase
      where E:B
  {
   
  }
}