using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FileHelpers;
using FileHelpers.Events;
using Iata.IS.Model.ValueConfirmation;
using log4net;

namespace Iata.IS.Business.ValueConfirmation.Impl
{

  class ResponseVCFFileHelper : IResponseVCFFileHelper
  {
    string GroupHeaderVast;
    int GroupHeaderLineNo;
    private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public ResponseVCFRequiredFields[] ReadResponseVCF(string  fileLocation)
    {
      try
      {
        var engine = new FileHelperEngine(typeof (ResponseVCFRequiredFields));
        //var engine = new FileHelperEngine<ResponseVCFRequiredFields>(); 

        engine.BeforeReadRecord += BeforeEvent;
        engine.AfterReadRecord += AfterEvent;

        return (ResponseVCFRequiredFields[]) engine.ReadFile(fileLocation);
      }
      catch (Exception ex)
      {
        logger.Error("Exception occured while reading response VCF file for value confirmation updation records.", ex);
      }

      return null;
    }

      private void BeforeEvent(EngineBase engine, BeforeReadEventArgs<object> e)
    {

      if (e.RecordLine.StartsWith("01"))
      {
        GroupHeaderVast = e.RecordLine.Substring(26, 1).ToUpper();
        GroupHeaderLineNo = e.LineNumber;
        e.SkipThisRecord = true;
      }
      else if(e.RecordLine.StartsWith("VCF"))
      {
        e.SkipThisRecord = true;
      }
     
    }

      private void AfterEvent(EngineBase engine, AfterReadEventArgs<object> e)
    {

      if (e.RecordLine.StartsWith("02"))
      {
        ((ResponseVCFRequiredFields)e.Record).Vast = GroupHeaderVast;
        ((ResponseVCFRequiredFields)e.Record).GroupHeaderLineNo = Convert.ToString(GroupHeaderLineNo);
      }
    }
  }
}
