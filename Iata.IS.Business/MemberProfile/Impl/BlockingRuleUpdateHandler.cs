using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Iata.IS.Core.Configuration;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Core.DI;
using System.Xml;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Data.Impl;
using log4net;
using System.Reflection;

namespace Iata.IS.Business.MemberProfile.Impl
{
  public class BlockingRuleUpdateHandler : IBlockingRuleUpdateHandler
  {
    // Logger instance.
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public IICHUpdateHandler ICHUpdateHandler { get; set; }
    public string blockingRuleUpdateXSDSchemaFileName = @"App_Data\SchemaFiles\BlockingRuleUpdate.xsd";  
    /// <summary>
    /// Following method generates XML for BlockingRule updates
    /// </summary>
    /// <returns>XML string</returns>
    public string GenerateXMLForBlockingRuleUpdates(int blockingRuleId)
    {
      // updateXml
      var updateXml = new XmlDocument();
        string blockingRuleUpdatedXml = string.Empty;
      // Declare object of type BlockingRuleUpdate
      BlockingRuleUpdate blockingRuleUpdate;

      // Create instance of BlockingRulesManager to access its methods
      var blockingRuleManager = Ioc.Resolve<IBlockingRulesManager>(typeof(IBlockingRulesManager));

      try
      {
        // Retrieve BlockingRule member details
        var blockingRuleDetails = blockingRuleManager.GetBlockingRuleDetails(blockingRuleId);

        // If BlockingRule exists populate BlockingRuleUpdateHandler object
        if (blockingRuleDetails != null)
        {
          // Instantiate BlockingRuleUpdateHandler
          blockingRuleUpdate = new BlockingRuleUpdate();

          // Set BlockingRule Id
          blockingRuleUpdate.RuleId = blockingRuleDetails.Id;
          // Set MemberCode
          blockingRuleUpdate.MemberCode = blockingRuleDetails.Member.MemberCodeAlpha + blockingRuleDetails.Member.MemberCodeNumeric;
          // Set Rule description
            blockingRuleUpdate.RuleDescription = HttpUtility.HtmlEncode(blockingRuleDetails.Description);
          //blockingRuleUpdate.RuleDescription = blockingRuleDetails.Description.Replace(@"/","//");
          // Instantiate Creditor List
          blockingRuleUpdate.BlockedCreditors = new List<Creditor>();
          // Instantiate Debtor List
          blockingRuleUpdate.BlockedDebtors = new List<Debtor>();
          // Instantiate BlockedByGroup class
          blockingRuleUpdate.BlockedByGroup = new BlockedByGroup();

          // Get Blocked Creditors list for the blockingRule
          var blockCreditorsList = blockingRuleManager.GetBlockMemberList(blockingRuleId, false);

          // If Blocked Creditors list is not equal to null, populate "Creditor" object
          if (blockCreditorsList != null)
          {
            // Iterate through blocked Creditor list and create billingCategory string
            foreach (var creditor in blockCreditorsList)
            {
              // Call "CreateBillingCategoryString()" method which will create comma separated string of billing category
              string billingCategory = CreateBillingCategoryString(creditor.Pax, creditor.Cargo,
                  creditor.Misc, creditor.Uatp);

              // Create Creditor instance
              var blockCreditor = new Creditor();

              // Set MemberCode
              blockCreditor.MemberCode = creditor.Member.MemberCodeAlpha + creditor.Member.MemberCodeNumeric;
              // Set Billing category
              blockCreditor.BillingCategory = billingCategory;

              // Add "Creditor" object to List
              blockingRuleUpdate.BlockedCreditors.Add(blockCreditor);
            }// end foreach()
          }// end if()

          // Get Blocked Debtors list for the blockingRule
          var blockDebtorsList = blockingRuleManager.GetBlockMemberList(blockingRuleId, true);

          // If Blocked Debtors list is not equal to null, populate "Debtor" object
          if (blockDebtorsList != null)
          {
            // Iterate through blocked Debtor list and create billingCategory string
            foreach (var debtor in blockDebtorsList)
            {
              // Call "CreateBillingCategoryString()" method which will create comma separated string of billing category
              string billingCategory = CreateBillingCategoryString(debtor.Pax, debtor.Cargo, debtor.Misc, debtor.Uatp);

              // Create Debtor instance
              var blockDebtor = new Debtor();

              // Set MemberCode
              blockDebtor.MemberCode = debtor.Member.MemberCodeAlpha + debtor.Member.MemberCodeNumeric;
              // Set Billing category
              blockDebtor.BillingCategory = billingCategory;

              // Add "Debtor" object to List
              blockingRuleUpdate.BlockedDebtors.Add(blockDebtor);
            }// end foreach()
          }// end if()

          // Retrieve BlockGroup list for given blocking rule Id
          var blockByGroupList = blockingRuleManager.GetBlockGroupList(blockingRuleId);

          // If blockGroupList != null, add each blockGroup item to specific list 
          if (blockByGroupList != null)
          {
            // Instantiate "BlockedByGroup" class 
            BlockedByGroup blockGroup = new BlockedByGroup();
            // Instantiate Creditors list
            blockGroup.BlockedCreditors = new List<Creditor>();
            // Instantiate Debtors list
            blockGroup.BlockedDebtors = new List<Debtor>();

            // Create list of type Creditor
            List<Creditor> creditorList = new List<Creditor>();
            // Create list of type Debtor
            List<Debtor> debtorList = new List<Debtor>();

            // Iterate through blockByGroup list
            foreach (var item in blockByGroupList)
            {
              // If blockGroup item is of type Creditor, create Object of type Creditor and add to CreditorList
              if (item.IsBlockAgainst)
              {
                // Call "CreateBillingCategoryString()" method which will create comma separated string of billing category
                string billingCategory = CreateBillingCategoryString(item.Pax, item.Cargo,
                item.Misc, item.Uatp);

                // Instantiate Creditor
                Creditor creditor = new Creditor();
                // Set Zone
                creditor.Zone = item.DisplayZoneType;

                // Set Billing category
                creditor.BillingCategory = billingCategory;

                // Retrieve ExceptionList for BlockGroup
                var exceptionList = blockingRuleManager.GetBlockGroupExceptionsList(item.Id);

                // If Exceptions exist, add exception details to ExceptionList
                if (exceptionList != null)
                {
                  // Instantiate Exceptions list
                  creditor.Exceptions = new List<Exceptions>();

                  // Iterate through ExceptionList
                  foreach (var exception in exceptionList)
                  {
                    // Instantiate "Exceptions" class
                    Exceptions exp = new Exceptions();
                    // Set MemberCode
                    exp.MemberCode = exception.ExceptionMember.MemberCodeAlpha + exception.ExceptionMember.MemberCodeNumeric;

                    // Add Exception to ExceptionList
                    creditor.Exceptions.Add(exp);
                  }// end foreach()
                }// end if()

                // Add Creditor to CreditorList
                creditorList.Add(creditor);

              }// end if()
              else
              {
                // Call "CreateBillingCategoryString()" method which will create comma separated string of billing category
                string billingCategory = CreateBillingCategoryString(item.Pax, item.Cargo,
                item.Misc, item.Uatp);

                // Instantiate Debtor
                Debtor debtor = new Debtor();
                // Set Zone
                debtor.Zone = item.DisplayZoneType;
                // Set Billing category
                debtor.BillingCategory = billingCategory;

                // Retrieve ExceptionList for BlockGroup
                var exceptionList = blockingRuleManager.GetBlockGroupExceptionsList(item.Id);

                // If Exceptions exist, add exception details to ExceptionList
                if (exceptionList != null)
                {
                  // Instantiate Exceptions list
                  debtor.Exceptions = new List<Exceptions>();

                  foreach (var exception in exceptionList)
                  {
                    // Instantiate "Exceptions" class
                    Exceptions exp = new Exceptions();

                    // Set MemberCode
                    exp.MemberCode = exception.ExceptionMember.MemberCodeAlpha + exception.ExceptionMember.MemberCodeNumeric;
                    // Add Exception to Exceptions list
                    debtor.Exceptions.Add(exp);
                  }// end foreach()
                }// end if()

                // Add Debtor to DebtorList
                debtorList.Add(debtor);

              }// end else
            }// end foreach()

            // Add CreditorsList
            blockGroup.BlockedCreditors.AddRange(creditorList);
            // Add DebtorsList
            blockGroup.BlockedDebtors.AddRange(debtorList);
            // Add BlockGroup details
            // blockingRuleUpdate.BlockedByGroup.Add(blockGroup);
            blockingRuleUpdate.BlockedByGroup = blockGroup;
          }// end if()

          // Instantiate ICHXmlHandler class
          ICHXmlHandler xmlHandler = new ICHXmlHandler();

          // Call SerializeXml() method which will return XML string of blockingRuleUpdate object 
          string blockingRuleUpdateXml = xmlHandler.SerializeXml(blockingRuleUpdate, typeof(Model.MemberProfile.BlockingRuleUpdate));

          // Load Xml string retrieved after serialization 
          updateXml.LoadXml(blockingRuleUpdateXml);

          // Xslt string to remove blank Xml nodes from blocking rule xml
          string blankNodeRemoveStylesheet = "<?xml version='1.0' encoding='UTF-8'?> <xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:fn='http://www.w3.org/2005/xpath-functions'> <xsl:output method='xml' version='1.0' encoding='UTF-8' indent='yes'/> <xsl:strip-space elements='*'/> <xsl:template match='*[not(node()) and not(./@*)]'/> <xsl:template match='@* | node()'> <xsl:copy> <xsl:apply-templates select='@* | node()'/> </xsl:copy> </xsl:template> </xsl:stylesheet>";

          // Call CallXsltToModifyXml() method which will modify Xml to remove blank nodes
          updateXml = xmlHandler.CallXsltToModifyXml(updateXml, blankNodeRemoveStylesheet);

          // XSLT string required to modify "Exception" and "BlockByGroup" xml nodes from blocking rule Xml
          string XSLStylesheet = "<?xml version='1.0' encoding='UTF-8'?> <xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:fn='http://www.w3.org/2005/xpath-functions'> <xsl:output method='xml' version='1.0' encoding='UTF-8' indent='yes'/> <xsl:template match='Exceptions/Exceptions'> <xsl:apply-templates></xsl:apply-templates> </xsl:template> <xsl:template match='*'> <xsl:copy> <xsl:copy-of select='@*'/> <xsl:apply-templates/> </xsl:copy> </xsl:template> <xsl:template match='BlockedByGroup/BlockedCreditors'> <xsl:apply-templates></xsl:apply-templates> </xsl:template> <xsl:template match='*'> <xsl:copy> <xsl:copy-of select='@*'/> <xsl:apply-templates/> </xsl:copy> </xsl:template> <xsl:template match='BlockedByGroup/BlockedDebtors'> <xsl:apply-templates></xsl:apply-templates> </xsl:template> <xsl:template match='*'> <xsl:copy> <xsl:copy-of select='@*'/> <xsl:apply-templates/> </xsl:copy> </xsl:template> </xsl:stylesheet>";

          // Call CallXsltToModifyXml() method which will modify Xml
          updateXml = xmlHandler.CallXsltToModifyXml(updateXml, XSLStylesheet);

          // Remove Deleted node from Blocking rule Xml
          xmlHandler.RemoveOptionalNodes(updateXml.SelectSingleNode("BlockingRuleUpdate/Deleted"));

          // Rename "Creditors" node to "BlockedCreditors" under BlockByGroups
          XmlNodeList creditorNodeList = updateXml.SelectNodes("BlockingRuleUpdate/BlockedByGroup/Creditor");
          xmlHandler.RenameNode(creditorNodeList, "BlockedCreditors");

          // Rename "Debtors" node to "BlockedDebtors" under BlockByGroups
          XmlNodeList debtorNodeList = updateXml.SelectNodes("BlockingRuleUpdate/BlockedByGroup/Debtor");
          xmlHandler.RenameNode(debtorNodeList, "BlockedDebtors");

          // Save Xml result to a file
          // updateXml.Save(AppDomain.CurrentDomain.BaseDirectory + "bin\\" + "Result.xml");

          // Call Validate() method which will validate retrieved blocking rule Xml
          Logger.Info("Blocking Rules XML");
          Logger.Info(updateXml.InnerXml);
           blockingRuleUpdatedXml = updateXml.InnerXml;

           var XSDPath = string.Format("{0}{1}", ConnectionString.GetAppSetting("AppSettingPath"), blockingRuleUpdateXSDSchemaFileName);
            string validatedXml = xmlHandler.Validate(updateXml.InnerXml, XSDPath);//"BlockingRuleUpdate.xsd");


          Logger.Info("Validation Result");
          Logger.Info(validatedXml);

          //Get details from future update
          if (validatedXml != "OK")
          {
              var invalidXml = blockingRuleUpdatedXml;
              blockingRuleUpdatedXml = "Error";
            ICHUpdateHandler.SendAlertForXmlValidationFailure(blockingRuleDetails.MemberId,invalidXml,"BlockingRuleUpdate", validatedXml);
            Logger.Info(string.Format("Validation Erros [{0}]", validatedXml));
          }
          else
          {
            Logger.Info("XML generated successfully.");
          }



        }// end if()

      }// end try
      catch (Exception ex)
      {
        Logger.Info(ex.ToString());
        throw;
      }// end catch()

      return blockingRuleUpdatedXml;
    }// end GenerateXMLforBlockingRuleUpdates()

    /// <summary>
    /// Following method is used to create comma separated Billingcategory string i.e. P,C,M,U
    /// </summary>
    /// <param name="pax">Pax category</param>
    /// <param name="cargo">Cargo category</param>
    /// <param name="misc">Misc category</param>
    /// <param name="uatp">Uatp category</param>
    /// <returns>Billing Category comma separated string</returns>
    private string CreateBillingCategoryString(bool pax, bool cargo, bool misc, bool uatp)
    {
      // Declare string variable
      string billingCategory = string.Empty;

      // If Pax category is present append character "P" to billingCategory string
      if (pax)
        billingCategory = "P";

      // If Cargo category is present append character "C" to billingCategory string
      if (cargo)
      {
        if (billingCategory == string.Empty)
          billingCategory = "C";
        else
          billingCategory += ",C";
      }//end if()

      // If Misc category is present append character "M" to billingCategory string
      if (misc)
      {
        if (billingCategory == string.Empty)
          billingCategory = "M";
        else
          billingCategory += ",M";
      }//end if()

      // If Uatp category is present append character "U" to billingCategory string
      if (uatp)
      {
        if (billingCategory == string.Empty)
          billingCategory = "U";
        else
          billingCategory += ",U";
      }//end if()

      // Return BillingCatogory string
      return billingCategory;
    }// end CreateBillingCategoryString()

    /// <summary>
    /// Following method is used to generate xml for BlockingRule delete 
    /// </summary>
    /// <param name="blockingRuleId">Id of BlockingRule</param>
    /// <returns>Xml string for delete blocking rule</returns>
    public string GenerateXmlForBlockingRuleDelete(int blockingRuleId)
    {
      // Declare string variable for Xmlnig
      string blockingRuleDeleteXml = string.Empty;

      // Declare object of type BlockingRuleUpdate
      Model.MemberProfile.BlockingRuleUpdate blockingRuleUpdate;

      // Create instance of BlockingRulesManager to access its methods
      var blockingRuleManager = Ioc.Resolve<IBlockingRulesManager>(typeof(IBlockingRulesManager));

      // Create instance of BlockingRuleRepository to access its methods
      var blockingRuleRepository = Ioc.Resolve<IBlockingRulesRepository>(typeof(IBlockingRulesRepository));
      try
      {
        // Retrieve BlockingRule member details
        var blockingRuleDetails = blockingRuleManager.GetBlockingRuleDetails(blockingRuleId);

        // If BlockingRule exists populate BlockingRuleUpdateHandler object
        if (blockingRuleDetails != null)
        {
          // Instantiate BlockingRuleUpdateHandler
          blockingRuleUpdate = new Model.MemberProfile.BlockingRuleUpdate();

          // Set BlockingRule Id
          blockingRuleUpdate.RuleId = blockingRuleDetails.Id;
          // Set MemberCode
          blockingRuleUpdate.MemberCode = blockingRuleDetails.Member.MemberCodeAlpha + blockingRuleDetails.Member.MemberCodeNumeric;
          // Set Rule description
          blockingRuleUpdate.RuleDescription = blockingRuleDetails.Description;
          // Set Deleted flag to true
          blockingRuleUpdate.Deleted = true;

          // Instantiate ICHXmlHandler class
          ICHXmlHandler xmlHandler = new ICHXmlHandler();

          // Call SerializeXml() method which will return XML string of blockingRuleUpdate object 
          blockingRuleDeleteXml = xmlHandler.SerializeXml(blockingRuleUpdate, typeof(Model.MemberProfile.BlockingRuleUpdate));

          // Validate Xml 
          var XSDPath = string.Format("{0}{1}", ConnectionString.GetAppSetting("AppSettingPath"), blockingRuleUpdateXSDSchemaFileName);
            string validatedXml = xmlHandler.Validate(blockingRuleDeleteXml, XSDPath);// "BlockingRuleUpdate.xsd");

          Logger.Info("Validation Result");
          Logger.Info(validatedXml);

          //Get details from future update
          if (validatedXml != "OK")
          {
              var invalidXml = blockingRuleDeleteXml;
            blockingRuleDeleteXml = "Error";
            ICHUpdateHandler.SendAlertForXmlValidationFailure(blockingRuleDetails.MemberId,invalidXml,"BlockingRuleDelete", validatedXml);
            Logger.Info(string.Format("Validation Errors [{0}]", validatedXml));
          }
          else
          {
            Logger.Info("XML generated successfully.");
          }

          // Retrieve Blocking rule record from database for specified blockingRuleId
          var blockingRuleRecord = blockingRuleRepository.Single(br => br.Id == blockingRuleId);

          // If record exists delete record from database
          if (blockingRuleRecord != null)
          {
            blockingRuleRepository.Delete(blockingRuleRecord);
            UnitOfWork.CommitDefault();
          }// end if()

        }// end if()
      }// end try
      catch (Exception)
      {

      }// end catch

      // return blocking rule delete xml
      return blockingRuleDeleteXml;
    }// end GenerateXmlForBlockingRuleDelete()
  }// end BlockingRuleUpdateHandler class
}// end namespace
