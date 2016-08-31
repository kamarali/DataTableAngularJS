using System.Text;

namespace Iata.IS.Business.Common
{
  public class FormatUtility
  {
    public string FormatProrateSlipDetails(string prorateSlipDetails)
    {
      var formattedProrateSlipDetails = new StringBuilder();
      var splittedData = prorateSlipDetails.Split(new[] { '\n' });
      foreach (var data in splittedData)
      {
        var newData = data;
        var newLineCount = 0;
        if (data.Length > 80)
        {
          var counter = 80;
          while (counter < (data.Length + newLineCount))
          {
            newData = newData.Insert(counter, "\n");
            newLineCount++;
            counter += 81;
          }
        }

        formattedProrateSlipDetails.AppendFormat("{0}\n", newData);
      }
      return formattedProrateSlipDetails.ToString();
    }
  }
}
