namespace Iata.IS.Business.MemberProfile
{
  public interface IBlockingRuleUpdateHandler
  {
    /// <summary>
    /// This method will accept memberID for which BlockingRule details are updated and generate Update Xml
    /// </summary>
    /// <param name="blockingRuleId">BlockingRuleId</param>
    /// <returns>Blocking rule update Xml</returns>
    string GenerateXMLForBlockingRuleUpdates(int blockingRuleId);

    /// <summary>
    /// This method will accept memberID for which BlockingRule details are updated and generate Delete Xml
    /// </summary>
    /// <param name="blockingRuleId">Blocking rule Id</param>
    /// <returns>Blocking rule delete Xml</returns>
    string GenerateXmlForBlockingRuleDelete(int blockingRuleId);
  }
}
