using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Iata.IS.Core;
using Iata.IS.Data;
using Iata.IS.Data.Cargo;
using Iata.IS.Data.MemberProfile.Impl;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Cargo.ParsingModel;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Pax.ParsingModel;
using log4net;

namespace Iata.IS.Business.Cargo.Impl
{
  public class OutputGeneratorManager : IOutputGeneratorManager
  {
    /// <summary>
    /// Logger instance for logging
    /// </summary>
    private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// CargoInvoiceRepository
    /// </summary>
    public ICargoInvoiceRepository CargoInvoiceRepository { get; set; }

    /// <summary>
    /// Gets or sets the member repository.
    /// </summary>
    /// <value>The member repository.</value>
    public IRepository<Member> MemberRepository { get; set; }


    /// <summary>
    /// Gets Pax invoices matching the specified search criteria
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <returns></returns>
    public CargoInvoiceModel GetCargoOutputData(Model.Pax.SearchCriteria searchCriteria)
    {
      var invoiceModel = new CargoInvoiceModel();

      var cargoInvoices = CargoInvoiceRepository.GetCargoInvoiceHierarchy(billedMemberId: searchCriteria.BilledMemberId > 0 ? (int?)searchCriteria.BilledMemberId : null, billingPeriod: searchCriteria.BillingPeriod,
                          billingMonth: searchCriteria.BillingMonth, billingYear: searchCriteria.BillingYear, invoiceStatusIds: searchCriteria.InvoiceStatusIds,
                          billingCode: searchCriteria.BillingCode, billingMemberId: searchCriteria.BillingMemberId > 0 ? (int?)searchCriteria.BillingMemberId : null);
      
      invoiceModel.CgoInvoiceCollection = cargoInvoices;
      if (cargoInvoices.Count > 0)
      {
          //Changes done to handle type B member
          var billedMemberCodeNumeric = GetNumericMemberCode(GetBilledMemberCodeNumeric(searchCriteria.BilledMemberId));
          invoiceModel.FileHeader = GetFileHeader(billedMemberCodeNumeric);
          invoiceModel.FileTotal = GetFileTotalModel(cargoInvoices, billedMemberCodeNumeric);
      }
        return invoiceModel;
    }

    /// <summary>
    /// Returns MemberCodeNumeric of respective billed member
    /// </summary>
    /// <param name="billedMemberId"></param>
    /// <returns></returns>
    private string GetBilledMemberCodeNumeric(int billedMemberId)
    {
      var member = MemberRepository.First(i => i.Id == billedMemberId);
      if (member != null) return member.MemberCodeNumeric;
      return "0";
    }

    /// <summary>
    /// Gets the numeric member code.
    /// </summary>
    /// <param name="memberCode">The member code.</param>
    /// <returns></returns>
    public int GetNumericMemberCode(string memberCode)
    {
      var index = 0;
      int value;

      if (Validators.IsWholeNumber(memberCode))
      {
        return Convert.ToInt32(memberCode);
      }

      var memberCodeAsciiChars = new byte[memberCode.Length];
      Encoding.ASCII.GetBytes(memberCode.ToUpper(), 0, memberCode.Length, memberCodeAsciiChars, 0);
      foreach (var memberCodeAsciiValue in memberCodeAsciiChars)
      {
        if (memberCodeAsciiValue <= 90 && memberCodeAsciiValue >= 65)
        {
          //To get A = 10, B=11
          value = memberCodeAsciiValue - 55;
          string toReplace = memberCode.Substring(index, 1);
          memberCode = memberCode.Replace(toReplace, value.ToString());
        }
        index++;
      }

      int numericMemberCode;
      int returnValue;
      if (Int32.TryParse(memberCode, out numericMemberCode))
      {
        returnValue = numericMemberCode > 9999 ? 0 : numericMemberCode;
      }
      else
      {
        returnValue = 0;
      }

      return returnValue;
    }

    /// <summary>
    /// Generates and returns FileHeader model
    /// </summary>
    /// <param name="billedMemberId"></param>
    /// <returns></returns>
    private FileHeader GetFileHeader(int billedMemberId)
    {
      var fileHeader = new FileHeader();
      fileHeader.AirlineCode = billedMemberId;
      return fileHeader;
    }

    /// <summary>
    /// Generates and returns the FileTotal model 
    /// </summary>
    /// <param name="invoiceCollection"></param>
    /// <param name="billedMemberId"></param>
    /// <returns></returns>
    private FileTotal GetFileTotalModel(IEnumerable<CargoInvoice> invoiceCollection, int billedMemberId)
    {
      var fileTotal = new FileTotal();
     
      decimal totalWeightCharge = 0, totalOtherCharge = 0, totalValuationCharge = 0, totalIscAmount = 0, netInvoiceTotal = 0, netInvoiceBillingTotal = 0, totalVatAmount = 0;
      var noOfBillingRecords = 0;

      foreach (var invoice in invoiceCollection)
      {
        if (invoice.CGOInvoiceTotal != null)
        {
          totalWeightCharge += invoice.CGOInvoiceTotal.TotalWeightCharge < 0 ? -(invoice.CGOInvoiceTotal.TotalWeightCharge) : invoice.CGOInvoiceTotal.TotalWeightCharge;
          totalOtherCharge += invoice.CGOInvoiceTotal.TotalOtherCharge < 0 ? -(invoice.CGOInvoiceTotal.TotalOtherCharge) : invoice.CGOInvoiceTotal.TotalOtherCharge;
          totalIscAmount += invoice.CGOInvoiceTotal.TotalIscAmount < 0 ? -(invoice.CGOInvoiceTotal.TotalIscAmount) : invoice.CGOInvoiceTotal.TotalIscAmount;
          netInvoiceTotal += invoice.CGOInvoiceTotal.NetTotal < 0 ? -(invoice.CGOInvoiceTotal.NetTotal) : invoice.CGOInvoiceTotal.NetTotal;
          netInvoiceBillingTotal += invoice.CGOInvoiceTotal.NetBillingAmount < 0 ? -(invoice.CGOInvoiceTotal.NetBillingAmount) : invoice.CGOInvoiceTotal.NetBillingAmount;
          noOfBillingRecords += invoice.CGOInvoiceTotal.NoOfBillingRecords < 0 ? -(invoice.CGOInvoiceTotal.NoOfBillingRecords) : invoice.CGOInvoiceTotal.NoOfBillingRecords;
          totalValuationCharge += invoice.CGOInvoiceTotal.TotalValuationCharge < 0 ? -(invoice.CGOInvoiceTotal.TotalValuationCharge) : invoice.CGOInvoiceTotal.TotalValuationCharge;
          totalVatAmount += invoice.CGOInvoiceTotal.TotalVatAmount < 0 ? -(invoice.CGOInvoiceTotal.TotalVatAmount) : invoice.CGOInvoiceTotal.TotalVatAmount;
        }
      }

      fileTotal.TotalWeightCharges = totalWeightCharge;
      fileTotal.TotalOtherCharges = totalOtherCharge;
      fileTotal.TotalInterlineServiceChargeAmount = totalIscAmount;
      fileTotal.NetTotal = netInvoiceTotal;
      fileTotal.NetBillingAmount = netInvoiceBillingTotal;
      fileTotal.NoOfBillingRecords = noOfBillingRecords;
      fileTotal.TotalValuationCharges = totalValuationCharge;
      fileTotal.TotalVatAmount = totalVatAmount;
      fileTotal.BilledAirline = billedMemberId;

      return fileTotal;
    }
  }
}
