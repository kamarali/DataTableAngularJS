using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Iata.IS.Model.MemberProfile;
using log4net;

namespace Iata.IS.Business.MemberProfile.Impl
{
  class MemberLocationCsvGenerator : IMemberLocationCsvGenerator
  {
    public char Separator { get; set; }
    public string FilePath { get; set; }

    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public bool CreateCsvFile(List<Location> list)
    {
      var flag = false;
      try
      {
        // Write the headers.
        StringBuilder csvString = new StringBuilder();

        csvString.Append("Location Code" + Separator + "Member Legal Name" + Separator + "Member Commercial Name");
        csvString.Append(Separator + "Is Active" + Separator + "Registration Id");
        csvString.Append(Separator + "Tax Vat Registration Number");
        csvString.Append(Separator + "Additional Tax Vat Registration Number");

        csvString.Append(Separator + "Address Line1");
        csvString.Append(Separator + "Country");
        csvString.Append(Separator + "Address Line2");
        csvString.Append(Separator + "Address Line3");
        csvString.Append(Separator + "City Name");

        csvString.Append(Separator + "SubDivision Code" + Separator + "SubDivision Name");
        csvString.Append(Separator + "PostalCode");
        csvString.Append(Separator + "Legal Text" + Separator + "Iban");

        csvString.Append(Separator + "Swift" + Separator + "Bank Code" + Separator + "Branch Code" + Separator + "Bank Account Number");
        csvString.Append(Separator + "Bank Account Name" + Separator + "Bank Name" + Separator + "Currency" + Separator + "Is Uatp Location");

        csvString.Append(Environment.NewLine);

        // Now write all the rows.
        foreach (var item in list)  
        {
       
          var active = item.IsActive ? "Yes" : "No";

          csvString.Append(("\"" + item.LocationCode + "\"" ?? String.Empty) + Separator + ("\"" + item.MemberLegalName + "\"" ?? String.Empty) + Separator + ("\"" + item.MemberCommercialName + "\"" ?? String.Empty));
          csvString.Append(Separator + active + Separator + ("\"" + item.RegistrationId + "\"" ?? String.Empty));
          csvString.Append(Separator + ("\"" + item.TaxVatRegistrationNumber + "\"" ?? String.Empty));
          csvString.Append(Separator + ("\"" + item.AdditionalTaxVatRegistrationNumber + "\"" ?? String.Empty));

          csvString.Append(Separator + ("\"" + item.AddressLine1 + "\"" ?? String.Empty) + Separator);
          csvString.Append((item.Country != null ? "\"" + item.Country.Name + "\"" : String.Empty));
          csvString.Append(Separator + ("\"" + item.AddressLine2 + "\"" ?? String.Empty));
          csvString.Append(Separator + ("\"" + item.AddressLine3 + "\"" ?? String.Empty));
          csvString.Append(Separator + ("\"" + item.CityName + "\"" ?? String.Empty));

          csvString.Append(Separator + ("\"" + item.SubDivisionCode+"\"" ?? String.Empty) + Separator + ("\""+item.SubDivisionName+"\"" ?? String.Empty));
          csvString.Append(Separator + ("\""+item.PostalCode+"\"" ?? String.Empty));
          csvString.Append(Separator + ("\"" + item.LegalText+"\"" ?? String.Empty) + Separator + ("\""+item.Iban+"\"" ?? String.Empty));

          csvString.Append(Separator + ("\"" + item.Swift + "\"" ?? String.Empty) + Separator + ("\"" + item.BankCode + "\"" ?? String.Empty) + Separator + ("\"" + item.BranchCode + "\"" ?? String.Empty) + Separator + ("\"" + item.BankAccountName + "\"" ?? String.Empty));
          csvString.Append(Separator + ("\"" + item.BankAccountName + "\"" ?? String.Empty) + Separator + ("\"" + item.BankName + "\"" ?? String.Empty) + Separator);
          csvString.Append((item.Currency != null ? "\"" + item.Currency.Code + "\"" : String.Empty));
          csvString.Append(Separator + item.IsUatpLocation.ToString());

          csvString.Append(Environment.NewLine);
        }

        var sw = new StreamWriter(FilePath, false);
        sw.WriteLine(csvString.ToString());
        sw.Close();

        flag = true;
      }
      catch (Exception exception)
      {
        Logger.Error("Error occurred while generating csv file using MemberLocationCsvGenerator service.", exception);
        flag = false;
      }

      return flag;
    }
  }
}
