using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;

namespace Iata.IS.Data.Common.Impl
{
    public class ConcurrencyData : Repository<InvoiceBase>, IConcurrencyData
    {
       #region Constants
      
        private const string InvoiceId = "INVOICE_ID_I";
        private const string TransactionId = "TRANSACTION_ID_I";
        private const string CorrespondenceNo = "CORRESPONDENCE_NO_I";
        private const string CorrespondenceStage = "CORRESPONDENCE_STAGE_I";
        private const string TableName = "TABLE_NAME_I";
        private const string IsInvEditable = "CAN_EDIT_O";
        private const string CheckInvoiceStatus = "CheckInvoiceStatusForEdit";
      
        #endregion

        /// <summary>
        /// Check invoice is restricted or not.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="correspondenceNo">Correspondence Number</param>
        /// <param name="correspondenceStage">Correspondence Stage</param>
        /// <returns>
        /// CMP 400 change when invoice deleted. User not able to perform any transaction on it.
        /// 0 = invoice not able to edit.
        /// 1 = invoice can be edit.
        /// 2 = invoice has been deleted.
        /// </returns>
        public int IsInvoiceRestricted(Guid? invoiceId, Guid? transactionId, long? correspondenceNo, int? correspondenceStage, string tableName)
        {
            //var returnValue = true;

            try
            {
                var parameters = new ObjectParameter[6];
                parameters[0] = new ObjectParameter(InvoiceId, typeof(string))
                {
                    Value = invoiceId
                };
                parameters[1] = new ObjectParameter(TransactionId, typeof(string))
                {
                    Value = transactionId
                };
                parameters[2] = new ObjectParameter(CorrespondenceNo, typeof (long?))
                {
                    Value = correspondenceNo
                };
                parameters[3] = new ObjectParameter(CorrespondenceStage, typeof (int?))
                {
                    Value = correspondenceStage
                };
                parameters[4] = new ObjectParameter(TableName, typeof(string))
                {
                    Value = tableName
                };
            
                parameters[5] = new ObjectParameter(IsInvEditable, typeof(int));

                ExecuteStoredProcedure(CheckInvoiceStatus, parameters);
                return Convert.ToInt32(parameters[5].Value);
         
            }
            catch (Exception exception)
            {
                return 0;

            }
           
        }


    }
}
