using NVelocity;
using Iata.IS.Model.Enums;

namespace Iata.IS.Business.TemplatedTextGenerator
{
  public interface ITemplatedTextGenerator
  {
    string GenerateTemplatedText(EmailTemplateId templatedId, VelocityContext context);
    string GenerateEmbeddedTemplatedText(string templateName, VelocityContext context);
 

  }
}
