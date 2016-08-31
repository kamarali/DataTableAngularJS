using Iata.IS.Model.ValueConfirmation;

namespace Iata.IS.Business.ValueConfirmation
{
  public interface IResponseVCFFileHelper
  {
    ResponseVCFRequiredFields[] ReadResponseVCF(string fileLocation);
  }
}
