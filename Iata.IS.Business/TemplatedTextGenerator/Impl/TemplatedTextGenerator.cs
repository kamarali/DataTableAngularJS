using System;
using System.Collections;
using System.Linq;
using NVelocity;
using NVelocity.App;
using Commons.Collections;
using System.IO;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Common;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Model.Pax;
using log4net;
using System.Reflection;
using NVelocity.Runtime;

namespace Iata.IS.Business.TemplatedTextGenerator.Impl
{
  class TemplatedTextGenerator : ITemplatedTextGenerator
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public string GenerateTemplatedText(EmailTemplateId templateId, VelocityContext context)
    {
      try
      {
        //get email settings
        var emailTemplateRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
        var fileServerRepository = Ioc.Resolve<IRepository<FileServer>>(typeof(IRepository<FileServer>));

        Logger.DebugFormat("Email template id [{0}]", templateId);

        //Get the eMail settings for member profile future update mail for own member update contact type
        var emailSettings = emailTemplateRepository.Get(es => es.Id == (int)templateId);

        var velocity = new VelocityEngine();
        var props = new ExtendedProperties();
        var templateBasePath = System.Configuration.ConfigurationManager.AppSettings["AppSettingPath"].ToString();
        props.AddProperty(RuntimeConstants.RESOURCE_LOADER, "file");
        props.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, templateBasePath);
        velocity.Init(props);

        //Get details of the Templates File Server
        var templateFileServerPath = fileServerRepository.Get(fs => fs.ServerType == "Templates" && fs.Status == 1);

        //combine root path of template file server, relative path of templates folder and the template file name.
        //merge this template with data
        var emailTemplateEntry = emailSettings.SingleOrDefault();

        
        
        //change to read directory name form app config file
        //var fileServerEntry = templateFileServerPath.SingleOrDefault();
        //Logger.DebugFormat("File server entry [{0}]", fileServerEntry == null ? "notfound" : fileServerEntry.BasePath);
        Logger.DebugFormat("Template base path [{0}]", string.IsNullOrEmpty(templateBasePath) ? "notfound" : templateBasePath);
        
        if (emailTemplateEntry == null)
        {
          Logger.InfoFormat("Email template entry [not found]");
        }
        else
        {
          Logger.InfoFormat("Email template entry [{0}, [{1}, {2}]", emailTemplateEntry.TemplateFileName, emailTemplateEntry.Subject, emailTemplateEntry.Remarks);
        }
        Logger.InfoFormat("Current Directory [{0}]", templateBasePath);

        //var templateFilePath = Path.Combine(fileServerEntry == null ? string.Empty : fileServerEntry.BasePath.Trim(), emailTemplateEntry.TemplatePath.Trim(), emailTemplateEntry.TemplateFileName.Trim());
        //var templateFilePath = Path.Combine(string.IsNullOrEmpty(templateBasePath) ? string.Empty : templateBasePath.Trim(), emailTemplateEntry.TemplatePath.Trim(), emailTemplateEntry.TemplateFileName.Trim());
        var templateFilePath = Path.Combine( emailTemplateEntry.TemplatePath.Trim(), emailTemplateEntry.TemplateFileName.Trim());
        Logger.InfoFormat("Template file path: [{0}]", templateFilePath);
        var template = velocity.GetTemplate(templateFilePath);
        var writer = new StringWriter();
        template.Merge(context, writer);
        string retVal = writer.ToString();
          writer.Close();
          return retVal;
      }
      catch (Exception exception)
      {
        Logger.Error("Error generated templated text", exception);
        throw;
      }
    }

    /// <summary>
    /// Generates the embedded templated text.
    /// </summary>
    /// <param name="templateName">Name of the template.</param>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    public string GenerateEmbeddedTemplatedText(string templateName, VelocityContext context)
    {
      try
      {
        string templatestring;
        var assembly = Assembly.GetExecutingAssembly();
        using (var textStreamReader = new StreamReader(assembly.GetManifestResourceStream(templateName)))
        {
          templatestring = textStreamReader.ReadToEnd();
          textStreamReader.Close();
        }

        var engine = new VelocityEngine();
        var properties = new ExtendedProperties();
        engine.Init(properties);

        string outputText;
        using (var writer = new StringWriter())
        {
          engine.Evaluate(context, writer, null, templatestring);
          outputText = writer.ToString();
          writer.Close();
        }

        templatestring = null;
        return outputText;
      }
      catch (Exception exception)
      {
        Logger.Error("Error generated Embedded templated text", exception);
        throw;
      }
    }
  }
}
