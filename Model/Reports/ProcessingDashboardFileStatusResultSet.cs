using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports
{
    public class ProcessingDashboardFileStatusResultSet
    {

      //this is the id of the file
      public Guid IsFileLogId { get; set; }

      //this is the total number of invoices in the file
      public int NumberOfInvoicesInFile { get; set; }

      //this is the total number of valid invoices in the file
      public int NumberOfValidInvoicesInFile { get; set; }

      //this is the total number of invalid invoices in the file
      public int NumberOfInvalidInvoicesInFile { get; set; }

      // Property to get and set FileName
      public string FileName { get; set; }

      // Property to get and set Billing Member Id
      public int BillingMemberId { get; set; }

      // Property to get and set Billing Member Code
      public string BillingMemberCode { get; set; }

      // Property to get and set Billing Member Name
      public string BillingMemberName { get; set; }

      // Property to get and set Billing Category Id
      public int BillingCategoryId { get; set; }

      // Property to get and set Billing Category
      public string BillingCategory { get; set; }

      // Property to get and set File Format Id
      public int FileFormatId { get; set; }

      // Property to get and set File Format
      public string FileFormat { get; set; }

      // Property to get and set Date Time when the File was uploaded in IS
      public DateTime ReceivedByIS { get; set; }

      // Property to Format ReceivedInIS date.
      public string FormatedReceivedByISDate
      {
          get
          {
              string formatedDate = string.Empty;
              formatedDate = ReceivedByIS.ToString("dd MMM yyyy HH:mm");
              return formatedDate;
          }
      }

      public DateTime FileGeneratedDate { get; set; }

      public string FormatedFileGeneratedDate
      {
          get
          {
              string formatedDate = string.Empty;
              formatedDate = FileGeneratedDate.ToString("dd MMM yyyy HH:mm");
              return formatedDate;
          }
      }
      // Property to get and set File Status Id
      public int FileStatusId { get; set; }
      
      // Property to get and set File Status
      public string FileStatus { get; set; }

      public int RejectOnValidationFailure { get; set; }

      public int InvoicesInPeriodError { get; set; }

      /* CMP #675: Progress Status Bar for Processing of Billing Data Files. 
      * Desc: New column Added. */
      public int FileProgressStatus { get; set; }

    }// end ProcessingDashboardFileStatus class
}// end namespace
