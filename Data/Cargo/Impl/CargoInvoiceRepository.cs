using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using Devart.Data.Oracle;
using Iata.IS.Core;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MemberProfile.Impl;
using Iata.IS.Data.MiscUatp.Impl;
using Iata.IS.Data.Pax.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Cargo.BillingHistory;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.ParsingModel;
using Iata.IS.Model.Pax.Sampling;
using Microsoft.Data.Extensions;

using InvoiceSearchCriteria = Iata.IS.Model.Pax.BillingHistory.InvoiceSearchCriteria;
using RejectionMemo = Iata.IS.Model.Pax.RejectionMemo;
using SearchCriteria = Iata.IS.Model.Pax.SearchCriteria;

namespace Iata.IS.Data.Cargo.Impl
{
    public class CargoInvoiceRepository : RepositoryEx<CargoInvoice, InvoiceBase>, ICargoInvoiceRepository
    {
        public CargoInvoice Single(Expression<Func<CargoInvoice, bool>> where)
        {
            throw new NotImplementedException("Use Overloaded Single instead.");
        }

        /// <summary>
        /// Singles the specified invoice id.
        /// </summary>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <param name="billingMonth">The billing month.</param>
        /// <param name="billingYear">The billing year.</param>
        /// <param name="billingPeriod">The billing period.</param>
        /// <param name="billingMemberId">The billing member id.</param>
        /// <param name="billedMemberId">The billed member id.</param>
        /// <param name="billingCode">The billing code.</param>
        /// <param name="id">The id.</param>
        /// <param name="invoiceStatusId"></param>
        /// <returns></returns>
        public CargoInvoice Single(string invoiceNumber = null, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null, Guid? id = null, int? invoiceStatusId = null)
        {
            var entities = new string[] { LoadStrategy.CargoEntities.BilledMember,LoadStrategy.CargoEntities.BillingMember, LoadStrategy.CargoEntities.MemberLocation, LoadStrategy.CargoEntities.InvoiceTotal, 
      LoadStrategy.Entities.ListingCurrency,LoadStrategy.CargoEntities.BillingCodeSubTotal};

            var loadStrategy = new LoadStrategy(string.Join(",", entities));

            string invoiceId = null;
            if (id.HasValue) invoiceId = ConvertUtil.ConvertGuidToString(id.Value);
            string invoiceStatusIdstr = null;
            if (invoiceStatusId.HasValue) invoiceStatusIdstr = invoiceStatusId.Value.ToString();
            var invoices = GetInvoiceLS(loadStrategy, invoiceNumber, billingMonth, billingYear, billingPeriod, billingMemberId, billedMemberId, billingCode, invoiceId, invoiceStatusIdstr);
            CargoInvoice invoice = null;
            if (invoices.Count > 0)
            {
                //TODO: Need to throw exception if result count > 1
                invoice = invoices[0];
            }
            return invoice;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CargoInvoiceRepository"/> class.
        /// </summary>
        public CargoInvoiceRepository()
        {
            InitializeObjectSet();
        }

        /// <summary>
        /// Get invoice Header Information 
        /// //SCP363971 - AT - ISXML file for 25th April - Mistake in SIS Validation R1/R2 report received.
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="includeBillingBilled"></param>
        /// <returns></returns>
        public CargoInvoice GetInvoiceHeader(Guid invoiceId, bool includeBillingBilled = false)
        {
          var entities = new string[] { LoadStrategy.CargoEntities.CargoInvoice };
          if (includeBillingBilled)
          {
            entities = new string[] { LoadStrategy.CargoEntities.CargoInvoice, LoadStrategy.CargoEntities.BillingMember, LoadStrategy.CargoEntities.BilledMember };
          }

          var loadStrategy = new LoadStrategy(string.Join(",", entities));
          string invoiceIdStr = ConvertUtil.ConvertGuidToString(invoiceId);

          var invoices = GetInvoiceLS(loadStrategy: loadStrategy, invoiceId: invoiceIdStr);
          if (invoices.Count > 0)
          {
            if (invoices.Count > 1) throw new ApplicationException("Multiple records found");
            return invoices[0];
          }
          return null;
        }


        /// <summary>
        /// Gets the pax old idec invoice LS.
        /// </summary>
        /// <param name="loadStrategy">The load strategy.</param>
        /// <param name="billingMonth">The billing month.</param>
        /// <param name="billingYear">The billing year.</param>
        /// <param name="billingPeriod">The billing period.</param>
        /// <param name="billingMemberId">The billing member id.</param>
        /// <returns></returns>
        public List<CargoInvoice> GetCargoOldIdecInvoiceLS(LoadStrategy loadStrategy, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? checkValueConfurmation = null)
        {
            return ExecuteLoadsSP(SisStoredProcedures.GetCargoOldIdecInvoices,
                                    loadStrategy,
                                    new[] { new OracleParameter(CargoInvoiceRepositoryConstants.ParameterNameBillingMonth, billingMonth ?? null) ,
                                  new OracleParameter(CargoInvoiceRepositoryConstants.ParameterNameBillingYear, billingYear ?? null) ,
                                  new OracleParameter(CargoInvoiceRepositoryConstants.ParameterNameBillingPeriod, billingPeriod ?? null) ,
                                  new OracleParameter(CargoInvoiceRepositoryConstants.ParameterNameBillingMemberId, billingMemberId ?? null)
                                 // new OracleParameter(InvoiceRepositoryConstants.ParameterNameCheckValueConfurmationStatus, checkValueConfurmation ?? null)
                                },
                                    r => this.FetchRecords(r));
        }
        ////SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
        ///// <summary>
        ///// Updates expiry period of current transaction and transactions prior to it.
        ///// </summary>
        ///// <param name="transactionId"></param>
        ///// <param name="transactionTypeId"></param>
        ///// <param name="expiryPeriod"></param>
        //public void UpdateExpiryDatePeriod(Guid transactionId, int transactionTypeId, DateTime expiryPeriod)
        //{
        //  var parameters = new ObjectParameter[3];

        //  parameters[0] = new ObjectParameter(CargoInvoiceRepositoryConstants.TransactionIdParameterName, typeof(Guid)) { Value = transactionId };
        //  parameters[1] = new ObjectParameter(CargoInvoiceRepositoryConstants.TransactionTypeParameterName, typeof(int)) { Value = transactionTypeId };
        //  parameters[2] = new ObjectParameter(CargoInvoiceRepositoryConstants.ExpiryPeriodParameterName, typeof(DateTime)) { Value = expiryPeriod };

        //  ExecuteStoredProcedure(CargoInvoiceRepositoryConstants.UpdateExpiryDatePeriodFunctionName, parameters);
        //}

        /// <summary>
        /// Initializes the object set.
        /// </summary>
        public override sealed void InitializeObjectSet()
        {
            EntityBaseObjectSet = Context.CreateObjectSet<InvoiceBase>();
            EntityObjectQuery = EntityBaseObjectSet.OfType<CargoInvoice>();
        }

        //Review : Not used anywhere , can be deleted.
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        public override IQueryable<CargoInvoice> GetAll()
        {
            return EntityObjectQuery
            .Include("BilledMember")
            .Include("ListingCurrency")
            .Include("IsInputFile")
            .Include("InvoiceTotalRecord")
            .Include("SamplingFormEDetails")
            .Include("InvoiceOwner");

            //var miscCodeRepository = new Repository<MiscCode>();
            //miscCodeRepository.Get(rec=>rec.Group == MiscGroups.)

            //BillingCurrency.HasValue ? EnumList.GetBillingCurrencyDisplayValue(BillingCurrency.Value) : string.Empty;
        }

        /// <summary>
        /// Get all payables
        /// </summary>
        /// <returns></returns>
        public IQueryable<CargoInvoice> GetAllPayables()
        {
            return EntityObjectQuery
            .Include("BillingMember")
            .Include("ListingCurrency")
            .Include("IsInputFile")
            .Include("InvoiceTotalRecord")
            .Include("SamplingFormEDetails");
        }
        /// <summary>
        /// Populates the Invoice object with its child model
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<CargoInvoice> GetInvoiceHierarchy(System.Linq.Expressions.Expression<Func<CargoInvoice, bool>> where)
        {
            throw new NotImplementedException("Use overloaded GetInvoiceHierarchy instead.");
        }

        /// <summary>
        /// Old IDEC : Populates the Invoice object with its child model (using Load strategy)
        /// </summary>
        /// <param name="billedMemberId"></param>
        /// <param name="billingPeriod"></param>
        /// <param name="billingMonth"></param>
        /// <param name="billingYear"></param>
        /// <param name="invoiceStatusId"></param>
        /// <returns></returns>
        public List<CargoInvoice> GetOldIdecInvoiceHierarchy(int? billedMemberId = null, int? billingPeriod = null, int? billingMonth = null, int? billingYear = null, int? invoiceStatusId = null)
        {
            var entities = new string[] { LoadStrategy.CargoEntities.BilledMember, LoadStrategy.CargoEntities.BillingMember, LoadStrategy.CargoEntities.AwbRecord,
                                    LoadStrategy.CargoEntities.InvoiceTotal};

            var loadStrategy = new LoadStrategy(string.Join(",", entities));
            string invoiceStatusIdstr = null;
            if (invoiceStatusId.HasValue) invoiceStatusIdstr = invoiceStatusId.Value.ToString();
            var invoices = GetInvoiceLS(loadStrategy: loadStrategy, billingMonth: billingMonth, billingYear: billingYear, billingPeriod: billingPeriod, billedMemberId: billedMemberId, invoiceStatusIds: invoiceStatusIdstr);

            return invoices;
        }

        //public List<CargoInvoice> GetInvoiceHierarchy(string invoiceId)
        //{
        //  var entities = new string[] { LoadStrategy.Entities.BilledMember, LoadStrategy.Entities.BillingMember, LoadStrategy.Entities.MemberLocation, 
        //    LoadStrategy.Entities.Coupon, LoadStrategy.Entities.CouponTax, LoadStrategy.Entities.CouponVat, LoadStrategy.Entities.CouponDataVatIdentifier,
        //    LoadStrategy.Entities.BillingMemo, LoadStrategy.Entities.BillingMemoVat, LoadStrategy.Entities.BillingMemoVatIdentifier,
        //    LoadStrategy.Entities.BillingMemoCoupon, LoadStrategy.Entities.BillingMemoCouponTax, LoadStrategy.Entities.BillingMemoCouponVat,
        //    LoadStrategy.Entities.RejectionMemo, LoadStrategy.Entities.RejectionMemoVat, LoadStrategy.Entities.RejectionMemoVatIdentifier,
        //    LoadStrategy.Entities.RejectionMemoCoupon, LoadStrategy.Entities.RejectionMemoCouponTax, LoadStrategy.Entities.RejectionMemoCouponVat, 
        //    LoadStrategy.Entities.CreditMemo, LoadStrategy.Entities.CreditMemoVat, LoadStrategy.Entities.CreditMemoVatIdentifier,  
        //    LoadStrategy.Entities.CreditMemoCoupon, LoadStrategy.Entities.CreditMemoCouponTax, LoadStrategy.Entities.CreditMemoCouponVat, 
        //    LoadStrategy.Entities.SamplingFormDRecord, LoadStrategy.Entities.SamplingFormDTax, LoadStrategy.Entities.SamplingFormDVat, 
        //    LoadStrategy.Entities.SamplingFormEDetails, LoadStrategy.Entities.SamplingFormEDetailVat, LoadStrategy.Entities.ProvisionalInvoiceDetails,
        //    LoadStrategy.Entities.InvoiceTotal, LoadStrategy.Entities.InvoiceTotalVat, LoadStrategy.Entities.InvoiceTotalVatIdentifier, 
        //    LoadStrategy.Entities.SourceCodeTotal, LoadStrategy.Entities.SourceCodeTotalVat, LoadStrategy.Entities.SourceCodeVatIdentifier, 
        //    LoadStrategy.Entities.ListingCurrency, };

        //  LoadStrategy loadStrategy = new LoadStrategy(string.Join(",", entities));
        //  var invoices = GetInvoiceLS(loadStrategy: loadStrategy, invoiceId:invoiceId);

        //  return invoices;
        //}
    
        public void UpdateBillingCodeTotalVat(Guid invoiceId)
        {
            var parameters = new ObjectParameter[1];
            parameters[0] = new ObjectParameter(CargoInvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };

            ExecuteStoredProcedure(CargoInvoiceRepositoryConstants.UpdateBillingCodeVatFunctionName, parameters);
        }

        //<summary>
        //SCP ID- 85837  : update CGO Sequence Number
        //</summary>
        //<param name="invoiceId"></param>

        public bool UpdateTransSeqNoWithInBatch(Guid invoiceId)
        {
            var parameters = new ObjectParameter[2];
            parameters[0] = new ObjectParameter(CargoInvoiceRepositoryConstants.InvoiceIdparam, typeof(Guid)) { Value = invoiceId };
            parameters[1] = new ObjectParameter(CargoInvoiceRepositoryConstants.IsUpdate, typeof(int));
            ExecuteStoredProcedure(CargoInvoiceRepositoryConstants.UpdateCgoSequenceNoFunctionName, parameters);

            var returnValue = parameters[1].Value.ToString();

            return returnValue == "1" ? true : false;


        }

        /// <summary>
        /// Gets the derived vat details for an Invoice.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>
        /// List of derived vat details for the Invoice.
        /// </returns>
        //public IList<DerivedVatDetails> GetDerivedVatDetails(Guid invoiceId)
        //{
        //  //var parameter = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid))
        //  //                  {
        //  //                    Value = invoiceId
        //  //                  };

        //  //var derivedVatDetails = ExecuteStoredFunction<DerivedVatDetails>(InvoiceRepositoryConstants.GetDerivedVatDetailsFunctionName, parameter);

        //  //return derivedVatDetails.ToList();
        //  return null;
        //}

        /// <summary>
        /// Gets the non applied vat details.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>
        /// List of non-applied vat details for the Invoice.
        /// </returns>
        //public IList<NonAppliedVatDetails> GetNonAppliedVatDetails(Guid invoiceId)
        //{
        //  //var parameter = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid))
        //  //                  {
        //  //                    Value = invoiceId
        //  //                  };

        //  //var nonAppliedVatDetails = ExecuteStoredFunction<NonAppliedVatDetails>(InvoiceRepositoryConstants.GetNonAppliedVatDetailsFunctionName, parameter);

        //  //return nonAppliedVatDetails.ToList();
        //  return null;
        //}


        public void UpdateInvoiceOnReadyForBilling(Guid invoiceId, int billingCatId, int billingMemberId, int billedMemberId, int billingCodeId)
        {
            var parameters = new ObjectParameter[5];
            parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdOnReadyForBillingParameterName, typeof(Guid)) { Value = invoiceId };
            parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceBillingCatIdParameterName, typeof(int)) { Value = billingCatId };
            parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceBillingMemberIdParameterName, typeof(int)) { Value = billingMemberId };
            parameters[3] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceBilledMemberIdParameterName, typeof(int)) { Value = billedMemberId };
            parameters[4] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceBillingCodeIdParameterName, typeof(int)) { Value = billingCodeId };

            ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdateInvoiceOnReadyForBillingFunctionName, parameters);
        }








        /// <summary>
        /// Gets the invoice LS.
        /// </summary>
        /// <param name="loadStrategy">The load strategy.</param>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <param name="billingMonth">The billing month.</param>
        /// <param name="billingYear">The billing year.</param>
        /// <param name="billingPeriod">The billing period.</param>
        /// <param name="billingMemberId">The billing member id.</param>
        /// <param name="billedMemberId">The billed member id.</param>
        /// <param name="billingCode">The billing code.</param>
        /// <param name="invoiceId">The id.</param>
        /// <param name="invoiceStatusIds">The invoice status id.</param>
        /// <param name="couponSearchCriteriaString">To load only selected coupons</param>
        /// <returns>Returns list of Passenger invoice.</returns>
        public List<CargoInvoice> GetInvoiceLS(LoadStrategy loadStrategy, string invoiceNumber = null, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null, string invoiceId = null, string invoiceStatusIds = null, string couponSearchCriteriaString = null)
        {
            return ExecuteLoadsSP(SisStoredProcedures.GetCargoInvoice,
                                      loadStrategy,
                                      new[] { new OracleParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, invoiceId ?? null),  
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameInvoiceNo, invoiceNumber ?? null) ,
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameBillingMonth, billingMonth ?? null) ,
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameBillingYear, billingYear ?? null) ,
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameBillingPeriod, billingPeriod ?? null) ,
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameBillingMemberId, billingMemberId ?? null) ,
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameBilledMemberId, billedMemberId ?? null) ,
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameBillingCode, billingCode ?? null), 
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameInvoiceStatusId, invoiceStatusIds ?? null),
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameCouponSearchCriteriaString,couponSearchCriteriaString ?? null), 
                                },
                                      r => this.FetchRecords(r));
            return null;
        }

        //SCP85837: PAX CGO Sequence No
        public int IsValidBatchSequenceNo(Guid invoiceId, int batchRecordSequenceNo, int batchSequenceNo, int billing_code, Guid memoId)
        {
            var parameters = new ObjectParameter[6];
            parameters[0] = new ObjectParameter(CargoInvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
            parameters[1] = new ObjectParameter(CargoInvoiceRepositoryConstants.BatchRecordSequenceNoParameterName, typeof(int)) { Value = batchRecordSequenceNo };
            parameters[2] = new ObjectParameter(CargoInvoiceRepositoryConstants.BatchSequenceNoParameterName, typeof(int)) { Value = batchSequenceNo };
            parameters[3] = new ObjectParameter(CargoInvoiceRepositoryConstants.BillingCode, typeof(int)) { Value = billing_code };
            parameters[4] = new ObjectParameter(CargoInvoiceRepositoryConstants.MemoId, typeof(Guid)) { Value = memoId };
            parameters[5] = new ObjectParameter(CargoInvoiceRepositoryConstants.IsUniqueNoParameterName, typeof(int));


            ExecuteStoredProcedure(CargoInvoiceRepositoryConstants.IsValidBatchSequenceNoFunctionName, parameters);

            return Convert.ToInt32(parameters[5].Value);
        }



        /// <summary>
        /// Returns multiple records extracted from the result set.
        /// This is done by calling the right repository to populate the object set in the repository.
        /// </summary>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        private List<CargoInvoice> FetchRecords(LoadStrategyResult loadStrategyResult)
        {
            List<CargoInvoice> invoices = new List<CargoInvoice>();
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.Invoice))
            // if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.CargoInvoice))
            {
                invoices = LoadEntities(base.EntityBaseObjectSet, loadStrategyResult, null);
            }

            return invoices;
        }



        /// <summary>
        /// Execute stored procedure 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sp"></param>
        /// <param name="oraInputParameters"></param>
        /// <param name="fetch"></param>
        /// <returns></returns>
        private T ExecuteSP<T>(StoredProcedure sp, OracleParameter[] oraInputParameters, Func<LoadStrategyResult, T> fetch)
        {
            using (var result = new LoadStrategyResult())
            {
                using (var cmd = Context.CreateStoreCommand(sp.Name, CommandType.StoredProcedure) as OracleCommand)
                {
                    cmd.Parameters.AddRange(oraInputParameters);

                    // Add result parameters to Oracle Parameter Collection
                    foreach (SPResultObject resObj in sp.GetResultSpec())
                    {
                        var resultParam = new OracleParameter(resObj.ParameterName, OracleDbType.Cursor);
                        resultParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(resultParam);

                        //if the entity is requested, add it to the result
                        result.Add(resObj.EntityName, resultParam);
                    }

                    using (cmd.Connection.CreateConnectionScope())
                    {
                        //Execute SP

                        //Set CommandTimeout value to value given in the Config file 
                        //if it NOT in the config then it will be set to default value 0.
                        cmd.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
                        cmd.ExecuteNonQuery();

                        //Allow the caller to populate results
                        return fetch(result);
                    }
                }
            }
        }










        public string ValidateMemo(Guid invoiceId)
        {
            var parameters = new ObjectParameter[2];
            parameters[0] = new ObjectParameter(CargoInvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
            parameters[1] = new ObjectParameter(CargoInvoiceRepositoryConstants.ErrorCodeParameterName, typeof(string));

            ExecuteStoredProcedure(CargoInvoiceRepositoryConstants.ValidateMemoFunctionName, parameters);
            return parameters[1].Value.ToString();
            return null;
        }



        /// <summary>
        /// Get Invoice Legal PDF path 
        /// </summary>
        /// <param name="invoiceId">Invoice Number </param>
        /// <returns> string of InvoiceLegalPdf </returns>
        public string GetInvoiceLegalPdfPath(Guid invoiceId)
        {
            var parameters = new ObjectParameter[2];
            parameters[0] = new ObjectParameter("INVOICE_NO_I", typeof(Guid))
            {
                Value = invoiceId
            };
            parameters[1] = new ObjectParameter("R_PATH_INV_O", typeof(string));

            ExecuteStoredProcedure("GetLegalInvoicePDFPath", parameters);
            return parameters[1].Value.ToString();
        }




        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        public static List<CargoInvoice> LoadEntities(ObjectSet<InvoiceBase> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoInvoice> link)
        {
            List<CargoInvoice> invoices = new List<CargoInvoice>();
            var cargoMaterializers = new CargoMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.Invoice))
            {
                // first result set includes the category
                invoices = cargoMaterializers.CargoInvoiceMaterializer.Materialize(reader).Bind(objectSet).ToList();
                reader.Close();
            }

            //Load MemberLocationInformation by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.MemberLocation) && invoices.Count != 0)
            {
                MemberLocationInformationRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<MemberLocationInformation>(),
                                                                       loadStrategyResult,
                                                                       c => c.Invoice = invoices.Find(i => i.Id == c.InvoiceId));
            }

            //Load BillingCode SubTotal by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BillingCodeSubTotal) && invoices.Count != 0)
            {
                CargoBillingCodeSubTotalRepository.LoadEntities(objectSet.Context.CreateObjectSet<BillingCodeSubTotal>(), loadStrategyResult, c => c.Invoice = invoices.Find(i => i.Id == c.InvoiceId));
            }

            //Load AwbRecord by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.AwbRecord) && invoices.Count != 0)
            {
                CargoAwbRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<AwbRecord>(), loadStrategyResult, c => c.Invoice = invoices.Find(i => i.Id == c.InvoiceId));
            }

            //Load RM by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.RejectionMemo) && invoices.Count != 0)
            {
                RejectionMemoRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<CargoRejectionMemo>(), loadStrategyResult, c => c.Invoice = invoices.Find(i => i.Id == c.InvoiceId));
            }

            //Load BM by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BillingMemo) && invoices.Count != 0)
            {
                CargoBillingMemoRepository.LoadEntities(objectSet.Context.CreateObjectSet<CargoBillingMemo>(), loadStrategyResult, c => c.Invoice = invoices.Find(i => i.Id == c.InvoiceId));
            }

            //Load CM by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.CreditMemo) && invoices.Count != 0)
            {
                CargoCreditMemoRepository.LoadEntities(objectSet.Context.CreateObjectSet<CargoCreditMemo>(), loadStrategyResult, c => c.Invoice = invoices.Find(i => i.Id == c.InvoiceId));
            }

            //Load Billed Members by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BilledMember) && invoices.Count != 0)
            {
                MemberRepository.LoadEntities(objectSet.Context.CreateObjectSet<Member>(),
                                                                       loadStrategyResult,
                                                                       null,
                                                                       LoadStrategy.CargoEntities.BilledMember);
            }

            //Load Billing Members by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BillingMember) && invoices.Count != 0)
            {
                MemberRepository.LoadEntities(objectSet.Context.CreateObjectSet<Member>(),
                                                                       loadStrategyResult,
                                                                       null,
                                                                       LoadStrategy.CargoEntities.BillingMember);
            }

            //Load ListingCurrency by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.ListingCurrency) && invoices.Count != 0)
            {
                CurrencyRepository.LoadEntities(objectSet.Context.CreateObjectSet<Currency>(),
                                                                       loadStrategyResult,
                                                                       null,
                                                                       LoadStrategy.CargoEntities.ListingCurrency);
            }

            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.CargoVatIdentifier) && invoices.Count != 0)
            {
                CargoVatIdentifierRepository.LoadEntities(objectSet.Context.CreateObjectSet<CgoVatIdentifier>(),
                                                                       loadStrategyResult,
                                                                       null,
                                                                       LoadStrategy.CargoEntities.CargoVatIdentifier);
            }

            //Load InvoiceTotalRecord by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.InvoiceTotal) && invoices.Count != 0)
            {
                CargoInvoiceTotalRepository.LoadEntities(objectSet.Context.CreateObjectSet<CargoInvoiceTotal>(), loadStrategyResult, i => i.Invoice = invoices.Find(j => j.Id == i.Id));
            }

            //Load InvoiceTotalVat by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.InvoiceTotalVat) && invoices.Count != 0)
            {
                CargoInvoiceTotalVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<CargoInvoiceTotalVat>(), loadStrategyResult, null);
            }
            return invoices;
        }

        #region Billing History

        public IList<CargoBillingHistorySearchResult> GetBillingHistorySearchResult(Model.Cargo.BillingHistory.InvoiceSearchCriteria invoiceSearchCriteria, CorrespondenceSearchCriteria corrSearchCriteria)
        {
            var parameters = new ObjectParameter[13];

            parameters[0] = new ObjectParameter("INVOICE_NO_I", typeof(String)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.InvoiceNumber : null };
            parameters[1] = new ObjectParameter("BILLING_YEAR_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.BillingYear : 0 };
            parameters[2] = new ObjectParameter("BILLING_MONTH_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.BillingMonth : 0 };
            parameters[3] = new ObjectParameter("BILLING_PERIOD_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.BillingPeriod : 0 };
            parameters[4] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(int)) { Value = corrSearchCriteria != null ? corrSearchCriteria.CorrBilledMemberId : invoiceSearchCriteria != null ? invoiceSearchCriteria.BilledMemberId : 0 };
            parameters[5] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int)) { Value = corrSearchCriteria != null ? corrSearchCriteria.CorrBillingMemberId : invoiceSearchCriteria != null ? invoiceSearchCriteria.BillingMemberId : 0 };
            parameters[6] = new ObjectParameter("MEMO_TYPE_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.MemoTypeId == 0 ? -1 : invoiceSearchCriteria.MemoTypeId : -1 };
            parameters[7] = new ObjectParameter("REJECTION_STAGE_I", typeof(int?)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.RejectionStageId : null };
            parameters[8] = new ObjectParameter("REASON_CODE_I", typeof(String)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.ReasonCodeId : null };
            parameters[9] = new ObjectParameter("AWB_SERIAL_NUMBER_I", typeof(int?)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.AwbSerialNumber : 0 };
            parameters[10] = new ObjectParameter("MEMO_NO_I", typeof(String)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.MemoNumber : null };
            parameters[11] = new ObjectParameter("ISSUING_AIRLINE_ID_I", typeof(String)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.IssuingAirline : null };
            parameters[12] = new ObjectParameter("BILLING_TYPE_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.BillingTypeId : 0 };

            var sourceTotals = ExecuteStoredFunction<CargoBillingHistorySearchResult>("GetCgoBillingHistorySearchResult", parameters);

            return sourceTotals.ToList();
        }

        public List<CargoBillingHistorySearchResult> GetBillingHistoryCorrSearchResult(CorrespondenceSearchCriteria correspondenceSearchCriteria)
        {
            var parameters = new ObjectParameter[11];

            parameters[0] = new ObjectParameter("FROM_DATE_I", typeof(String)) { Value = correspondenceSearchCriteria.FromDate };
            parameters[1] = new ObjectParameter("TO_DATE_I", typeof(int)) { Value = correspondenceSearchCriteria.ToDate };
            parameters[2] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int)) { Value = correspondenceSearchCriteria.CorrBillingMemberId };
            parameters[3] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(int)) { Value = correspondenceSearchCriteria.CorrBilledMemberId };
            parameters[4] = new ObjectParameter("CORRESPONDENCE_NO_I", typeof(int)) { Value = correspondenceSearchCriteria.CorrespondenceNumber };
            parameters[5] = new ObjectParameter("CORRESPONDENCE_STATUS_I", typeof(int)) { Value = correspondenceSearchCriteria.CorrespondenceStatusId };
            parameters[6] = new ObjectParameter("CORRESPONDENCE_SUB_STATUS_I", typeof(int)) { Value = correspondenceSearchCriteria.CorrespondenceSubStatusId };
            parameters[7] = new ObjectParameter("AUTHORITY_TO_BILL_I", typeof(int?)) { Value = correspondenceSearchCriteria.AuthorityToBill == false ? 0 : 1 };
            parameters[8] = new ObjectParameter("CORR_INIT_MEM_I", typeof(int?)) { Value = correspondenceSearchCriteria.InitiatingMember };

            parameters[9] = new ObjectParameter("NO_OF_DAYS_TO_EXPIRE_I", typeof(int?)) { Value = correspondenceSearchCriteria.NoOfDaysToExpiry };
            parameters[10] = new ObjectParameter("CORR_OWNER_ID_I", typeof(int?)) { Value = correspondenceSearchCriteria.CorrespondenceOwnerId };

            var sourceTotals = ExecuteStoredFunction<CargoBillingHistorySearchResult>("GetCgoBillingHistoryCorrSearchResult", parameters);

            return sourceTotals.ToList();
        }

        public List<CorrespondenceTrailSearchResult> GetCorrespondenceTrailSearchResult(CorrespondenceTrailSearchCriteria correspondenceTrailSearchCriteria)
        {
            var parameters = new ObjectParameter[7];

            parameters[0] = new ObjectParameter("FROM_DATE_I", typeof(String)) { Value = correspondenceTrailSearchCriteria.FromDate };
            parameters[1] = new ObjectParameter("TO_DATE_I", typeof(int)) { Value = correspondenceTrailSearchCriteria.ToDate };
            parameters[2] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int)) { Value = correspondenceTrailSearchCriteria.CorrBillingMemberId };
            parameters[3] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(int)) { Value = correspondenceTrailSearchCriteria.CorrBilledMemberId };
            parameters[4] = new ObjectParameter("CORRESPONDENCE_STATUS_I", typeof(int)) { Value = correspondenceTrailSearchCriteria.CorrespondenceStatusId };
            parameters[5] = new ObjectParameter("CORRESPONDENCE_SUB_STATUS_I", typeof(int)) { Value = correspondenceTrailSearchCriteria.CorrespondenceSubStatusId };
            parameters[6] = new ObjectParameter("CORR_INIT_MEM_I", typeof(int?)) { Value = correspondenceTrailSearchCriteria.InitiatingMember };

            var correspondenceTrailSearchResult = ExecuteStoredFunction<CorrespondenceTrailSearchResult>("GetCgoCorrespondenceTrailSearchResult", parameters);
            return correspondenceTrailSearchResult.ToList();

        }

        //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
        //Desc: More details about existing BM are added, hence returned cursor is changed.
        public List<CargoExistingBMTransaction> GetBillingMemosForCorrespondence(long correspondenceNumber, int billingMemberId)
        {
            var parameters = new ObjectParameter[2];

            parameters[0] = new ObjectParameter("CORRESPONDENCE_REF_NO_I", typeof(string)) { Value = correspondenceNumber };
            parameters[1] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int)) { Value = billingMemberId };

            var billingMemos = ExecuteStoredFunction<CargoExistingBMTransaction>(CargoInvoiceRepositoryConstants.GetBillingMemosForCorrespondenceFunctionName, parameters);

            return billingMemos.ToList();
        }

        /// <summary>
        /// Singles the specified transaction id.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        public CargoAuditTrail AuditSingle(Guid transactionId, string transactionType = null)
        {
            var entities = new[]
                       {
                         LoadStrategy.CargoEntities.CargoInvoice ,LoadStrategy.CargoEntities.AwbRecord, LoadStrategy.CargoEntities.RejectionMemo, LoadStrategy.CargoEntities.Correspondence, 
                         LoadStrategy.CargoEntities.BillingMemo, LoadStrategy.CargoEntities.RmAwb, LoadStrategy.CargoEntities.BmAwb, LoadStrategy.CargoEntities.Members, LoadStrategy.CargoEntities.RmAwbVat,
                         LoadStrategy.CargoEntities.AwbRecordVat, LoadStrategy.CargoEntities.AwbAttachment, LoadStrategy.CargoEntities.RejectionMemoVat, LoadStrategy.CargoEntities.RejectionMemoAttachments,
                         LoadStrategy.CargoEntities.CorrespondenceAttachment, LoadStrategy.CargoEntities.CmAwbAttachments, LoadStrategy.CargoEntities.CreditMemoAttachments, LoadStrategy.CargoEntities.CmAwb,
                         LoadStrategy.CargoEntities.CreditMemo, LoadStrategy.CargoEntities.Currency, LoadStrategy.CargoEntities.BmAwbVat, LoadStrategy.CargoEntities.BmAwbAttachments, LoadStrategy.CargoEntities.RmAwbAttachments, 
                         LoadStrategy.CargoEntities.BillingMemoVat, LoadStrategy.CargoEntities.BillingMemoAttachments
                       };

            var loadStrategy = new LoadStrategy(string.Join(",", entities));
            var invoiceIdStr = ConvertUtil.ConvertGuidToString(transactionId);

            return GetCargoInvoiceAuditLs(loadStrategy, invoiceIdStr, transactionType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadStrategy"></param>
        /// <param name="transactionId"></param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        private CargoAuditTrail GetCargoInvoiceAuditLs(LoadStrategy loadStrategy, string transactionId, string transactionType)
        {

            return ExecuteCargoLoadsAuditSP(SisStoredProcedures.GetAuditCargoInvoice,
                                  loadStrategy,
                                  new[]
                              {
                                new OracleParameter(InvoiceRepositoryConstants.CargoInvoiceTransactionIdParameterName, transactionId ?? null),
                                new OracleParameter(InvoiceRepositoryConstants.CargoInvoiceTransactionTypeParameterName, transactionType ?? null)
                              },
                                  this.FetchAuditRecord);
        }

        /// <summary>
        /// Returns multiple records extracted from the result set.
        /// This is done by calling the right repository to populate the object set in the repository.
        /// </summary>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        private CargoAuditTrail FetchAuditRecord(LoadStrategyResult loadStrategyResult)
        {
            CargoAuditTrail auditTrail = null;
            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.CargoInvoice))
            {
                auditTrail = LoadAuditEntities(EntityBaseObjectSet, loadStrategyResult, null);
            }

            return auditTrail;
        }

        public static CargoAuditTrail LoadAuditEntities(ObjectSet<InvoiceBase> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoInvoice> link)
        {
            var auditTrail = new CargoAuditTrail();
            var cargoMaterializers = new CargoMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.CargoInvoice))
            {
                // first result set includes the category
                auditTrail.Invoices = cargoMaterializers.CargoInvoiceAuditMaterializer.Materialize(reader).Bind(objectSet).ToList();
                reader.Close();
            }

            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.AwbRecord) && auditTrail.Invoices.Count != 0)
            {
                CargoAwbRecordRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<AwbRecord>(), loadStrategyResult,
                                                    c => c.Invoice = auditTrail.Invoices.Find(i => i.Id == c.InvoiceId));
                //The fetched child records should use the Parent entities.
            }

            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.RejectionMemo) && auditTrail.Invoices.Count != 0)
            {
                RejectionMemoRecordRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<CargoRejectionMemo>(),
                                                                       loadStrategyResult,
                                                                       null,
                                                                       LoadStrategy.CargoEntities.RejectionMemo);
            }

            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BillingMemo) && auditTrail.Invoices.Count != 0)
            {
                CargoBillingMemoRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<CargoBillingMemo>(),
                                                              loadStrategyResult,
                                                              null,
                                                              LoadStrategy.CargoEntities.BillingMemo);
            }

            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.Currency) && auditTrail.Invoices.Count != 0)
            {
                CurrencyRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<Currency>(),
                                                                       loadStrategyResult,
                                                                       null,
                                                                       LoadStrategy.CargoEntities.Currency);
            }

            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.Members) && auditTrail.Invoices.Count != 0)
            {
                MemberRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<Member>(),
                                                                       loadStrategyResult,
                                                                       null,
                                                                       LoadStrategy.CargoEntities.Members);
            }

            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CreditMemo) && auditTrail.Invoices.Count != 0)
            {
                CargoCreditMemoRepository.LoadEntities(objectSet.Context.CreateObjectSet<CargoCreditMemo>(), loadStrategyResult, null);
            }

            return auditTrail;
        }

        #endregion

        /// <summary>
        /// This will return list of Cargo invoices along with child objects
        /// </summary>
        /// <param name="billedMemberId">billed Member Id</param>
        /// <param name="billingPeriod">billing Period</param>
        /// <param name="billingMonth">billing Month</param>
        /// <param name="billingYear">billing Year</param>
        /// <param name="invoiceStatusIds">invoiceStatusIds : Comma seperated list of invoice statuses</param>
        /// <param name="billingCode">billing Code</param>
        /// <param name="invoiceId">invoiceId</param>
        /// <param name="billingMemberId">billing Member Id</param>
        /// <returns>list of Cargo invoices along with child objects</returns>
        public List<CargoInvoice> GetCargoInvoiceHierarchy(int? billedMemberId = null, int? billingPeriod = null, int? billingMonth = null, int? billingYear = null, string invoiceStatusIds = null, int? billingCode = null, string invoiceId = null, int? billingMemberId = null)
        {
            var entities = new string[] { LoadStrategy.CargoEntities.MemberLocation,LoadStrategy.CargoEntities.BillingMember,LoadStrategy.CargoEntities.BilledMember,LoadStrategy.CargoEntities.CargoVatIdentifier,LoadStrategy.CargoEntities.ListingCurrency,
                                    LoadStrategy.CargoEntities.BillingCodeSubTotal,LoadStrategy.CargoEntities.CargoBillingCodeSubTotalVat,
                                    LoadStrategy.CargoEntities.AwbRecord,LoadStrategy.CargoEntities.AwbRecordVat,LoadStrategy.CargoEntities.AwbAttachment,LoadStrategy.CargoEntities.AwbOtherCharge,
                                    LoadStrategy.CargoEntities.RejectionMemo,LoadStrategy.CargoEntities.RejectionMemoVat,LoadStrategy.CargoEntities.RejectionMemoAttachments,
                                    LoadStrategy.CargoEntities.RmAwb,LoadStrategy.CargoEntities.RmAwbVat,LoadStrategy.CargoEntities.RmAwbProrateLadder,LoadStrategy.CargoEntities.RmAwbAttachments,LoadStrategy.CargoEntities.RmAwbOtherCharges,
                                    LoadStrategy.CargoEntities.BillingMemo,LoadStrategy.CargoEntities.BillingMemoVat,LoadStrategy.CargoEntities.BillingMemoAttachments,
                                    LoadStrategy.CargoEntities.BmAwb,LoadStrategy.CargoEntities.BmAwbVat,LoadStrategy.CargoEntities.BmAwbProrateLadder,LoadStrategy.CargoEntities.BmAwbAttachments,LoadStrategy.CargoEntities.BmAwbOtherCharges,
                                    LoadStrategy.CargoEntities.CreditMemo,LoadStrategy.CargoEntities.CreditMemoVat,LoadStrategy.CargoEntities.CreditMemoAttachments,
                                    LoadStrategy.CargoEntities.CmAwb,LoadStrategy.CargoEntities.CmAwbVat,LoadStrategy.CargoEntities.CmAwbProrateLadder,LoadStrategy.CargoEntities.CmAwbAttachments,LoadStrategy.CargoEntities.CmAwbOtherCharges,
                                    LoadStrategy.CargoEntities.InvoiceTotal,LoadStrategy.CargoEntities.InvoiceTotalVat
                                  };

            var loadStrategy = new LoadStrategy(string.Join(",", entities));
            var invoices = GetInvoiceLS(loadStrategy: loadStrategy, billingMonth: billingMonth, billingYear: billingYear, billingPeriod: billingPeriod, billedMemberId: billedMemberId, invoiceStatusIds: invoiceStatusIds, billingCode: billingCode, invoiceId: invoiceId, billingMemberId: billingMemberId);

            return invoices;
        }

        /// <summary>
        /// Updates the Prime Billing Invoice total.
        /// </summary>
        /// <param name="invoiceId">The Invoice id.</param>
        /// <param name="billingCodeId">The billingCode Id.</param>
        /// <param name="batchSeqNumber">The batchSeq Number.</param>
        /// <param name="reqSeqNumber">The reqSeq Number.</param>
        /// <param name="userId">The user id.</param>
        public void UpdateAwbInvoiceTotal(Guid invoiceId, int userId, int billingCodeId, int batchSeqNumber, int reqSeqNumber)
        {
            var parameters = new ObjectParameter[5];

            parameters[0] = new ObjectParameter(CargoInvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
            parameters[1] = new ObjectParameter(CargoInvoiceRepositoryConstants.UserIdParameterName, typeof(int)) { Value = userId };
            parameters[2] = new ObjectParameter(CargoInvoiceRepositoryConstants.BillingCodeParameterName, typeof(int)) { Value = billingCodeId };
            parameters[3] = new ObjectParameter(CargoInvoiceRepositoryConstants.BatchSeqNumber, typeof(int)) { Value = batchSeqNumber };
            parameters[4] = new ObjectParameter(CargoInvoiceRepositoryConstants.RecordSeqNumber, typeof(int)) { Value = reqSeqNumber };

            ExecuteStoredProcedure(CargoInvoiceRepositoryConstants.UpdateAwbInvoiceTotal, parameters);
        }

        /// <summary>
        /// Gets the derived vat details for an Invoice.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>
        /// List of derived vat details for the Invoice.
        /// </returns>
        public IList<DerivedVatDetails> GetDerivedVatDetails(Guid invoiceId)
        {
            var parameter = new ObjectParameter(CargoInvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid))
            {
                Value = invoiceId
            };

            var derivedVatDetails = ExecuteStoredFunction<DerivedVatDetails>(CargoInvoiceRepositoryConstants.GetCgoDerivedVatDetails, parameter);

            return derivedVatDetails.ToList();
        }

        /// <summary>
        /// Gets the non applied vat details.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>
        /// List of non-applied vat details for the Invoice.
        /// </returns>
        public IList<NonAppliedVatDetails> GetNonAppliedVatDetails(Guid invoiceId)
        {
            var parameter = new ObjectParameter(CargoInvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid))
            {
                Value = invoiceId
            };

            var nonAppliedVatDetails = ExecuteStoredFunction<NonAppliedVatDetails>(CargoInvoiceRepositoryConstants.GetCgoNonAppliedVatDetails, parameter);

            return nonAppliedVatDetails.ToList();
        }

        public List<CargoTransaction> GetRejectedTransactionDetails(string memoId, string couponIds)
        {
            var parameters = new ObjectParameter[2];

            parameters[0] = new ObjectParameter("MEMO_ID_I", typeof(string)) { Value = memoId };
            parameters[1] = new ObjectParameter("AWB_IDS_I", typeof(string)) { Value = couponIds };

            var rejectedTransactions = ExecuteStoredFunction<CargoTransaction>(CargoInvoiceRepositoryConstants.GetRejectedTransactionDetailsFunctionName, parameters);

            return rejectedTransactions.ToList();
        }


        /// <summary>
        /// Updates the cargo BM invoice total.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="billingCodeId">The billing code id.</param>
        /// <param name="billingMemoId">The billing memo id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="isAwbDelete">if set to <c>true</c> [is awb delete].</param>
        public void UpdateCargoBMInvoiceTotal(Guid invoiceId, int billingCodeId, Guid billingMemoId, int userId, bool isAwbDelete = false)
        {
            var parameters = new ObjectParameter[5];
            parameters[0] = new ObjectParameter(CargoInvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
            parameters[1] = new ObjectParameter(CargoInvoiceRepositoryConstants.CargoBillingCodeIdParameterName, typeof(int)) { Value = billingCodeId };
            parameters[2] = new ObjectParameter(CargoInvoiceRepositoryConstants.CargoBillingMemoIdParameterName, typeof(Guid)) { Value = billingMemoId };
            parameters[3] = new ObjectParameter(CargoInvoiceRepositoryConstants.IsAwbDeleteParameterName, typeof(Guid)) { Value = isAwbDelete ? 1 : 0 };
            parameters[4] = new ObjectParameter(CargoInvoiceRepositoryConstants.UserIdParameterName, typeof(Guid)) { Value = userId };

            ExecuteStoredProcedure(CargoInvoiceRepositoryConstants.UpdateCargoBMInvoiceTotalFunctionName, parameters);
        }


        /// <summary>
        /// Updates the cargo RM invoice total.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="billingCodeId">The billing code id.</param>
        /// <param name="rejectionMemoId">The rejection memo id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="isAwbDelete">if set to <c>true</c> [is awb delete].</param>
        public void UpdateCargoRMInvoiceTotal(Guid invoiceId, int billingCodeId, Guid rejectionMemoId, int userId, bool isAwbDelete = false)
        {
            var parameters = new ObjectParameter[5];
            parameters[0] = new ObjectParameter(CargoInvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
            parameters[1] = new ObjectParameter(CargoInvoiceRepositoryConstants.CargoBillingCodeIdParameterName, typeof(int)) { Value = billingCodeId };
            parameters[2] = new ObjectParameter(CargoInvoiceRepositoryConstants.RejectionMemoIdParameterName, typeof(Guid)) { Value = rejectionMemoId };
            parameters[3] = new ObjectParameter(CargoInvoiceRepositoryConstants.IsAwbDeleteParameterName, typeof(Guid)) { Value = isAwbDelete ? 1 : 0 };
            parameters[4] = new ObjectParameter(CargoInvoiceRepositoryConstants.UserIdParameterName, typeof(Guid)) { Value = userId };

            ExecuteStoredProcedure(CargoInvoiceRepositoryConstants.UpdateCargoRMInvoiceTotalFunctionName, parameters);
        }

        /// <summary>
        /// Update Cargo Invoice file status.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="billingMemberId"></param>
        /// <param name="isBadFileExists"></param>
        /// <param name="processId"></param>
        /// <param name="laFlag"></param>
        public void UpdateCargoInvoiceStatus(string fileName, int billingMemberId, bool isBadFileExists, string processId, bool laFlag)
        {
            var parameters = new ObjectParameter[5];
            parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.FileNameParameterName, typeof(string)) { Value = fileName };
            parameters[1] = new ObjectParameter(MiscInvoiceRepositoryConstants.BillingMemeberIdParameterName, typeof(int)) { Value = billingMemberId };
            parameters[2] = new ObjectParameter(MiscInvoiceRepositoryConstants.ProcessId, typeof(string)) { Value = processId };
            parameters[3] = new ObjectParameter(MiscInvoiceRepositoryConstants.IsBadFileExists, typeof(int)) { Value = isBadFileExists ? 1 : 0 };
            parameters[4] = new ObjectParameter(MiscInvoiceRepositoryConstants.LaFlag, typeof(int)) { Value = laFlag ? 1 : 0 };
            ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.UpdateInvoiceAndFileStatusFunctionName, parameters);
        }

        /// <summary>
        /// Updates the cargo Credit Memo invoice total.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="billingCodeId">The billing code id.</param>
        /// <param name="creditMemoId">The credit memo id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="isAwbDelete">if set to <c>true</c> [is awb delete].</param>
        public void UpdateCargoCMInvoiceTotal(Guid invoiceId, int billingCodeId, Guid creditMemoId, int userId, bool isAwbDelete = false)
        {
            var parameters = new ObjectParameter[5];
            parameters[0] = new ObjectParameter(CargoInvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
            parameters[1] = new ObjectParameter(CargoInvoiceRepositoryConstants.CargoBillingCodeIdParameterName, typeof(int)) { Value = billingCodeId };
            parameters[2] = new ObjectParameter(CargoInvoiceRepositoryConstants.CargoCreditMemoIdParameterName, typeof(Guid)) { Value = creditMemoId };
            parameters[3] = new ObjectParameter(CargoInvoiceRepositoryConstants.IsAwbDeleteParameterName, typeof(Guid)) { Value = isAwbDelete ? 1 : 0 };
            parameters[4] = new ObjectParameter(CargoInvoiceRepositoryConstants.UserIdParameterName, typeof(Guid)) { Value = userId };

            ExecuteStoredProcedure(CargoInvoiceRepositoryConstants.UpdateCargoCMInvoiceTotalFunctionName, parameters);
        }

        /// <summary>
        /// Executes "PROC_CGO_IS_BAT_SEQ_NO_DUP" stored procedure which will return unique Max Batch and Sequence number
        /// </summary>
        /// <param name="invoiceId">Invoice Id</param>
        /// <param name="transactionTypeId">Transaction type. eg. BM, CM, RM, AWB</param>
        /// <param name="batchNumber">Retrieved Batch number</param>
        /// <param name="sequenceNumber">Retrieved Sequence number</param>
        public void GetBatchAndSequenceNumber(Guid invoiceId, int transactionTypeId, out int batchNumber, out int sequenceNumber)
        {
            var parameters = new ObjectParameter[4];
            parameters[0] = new ObjectParameter(CargoInvoiceRepositoryConstants.InvoiceIdParameter, typeof(Guid)) { Value = invoiceId };
            parameters[1] = new ObjectParameter(CargoInvoiceRepositoryConstants.TransactionTypeIdParameter, typeof(int)) { Value = transactionTypeId };
            parameters[2] = new ObjectParameter(CargoInvoiceRepositoryConstants.BatchNumberParameter, typeof(int));
            parameters[3] = new ObjectParameter(CargoInvoiceRepositoryConstants.SequenceNumberParameter, typeof(int));

            ExecuteStoredProcedure(CargoInvoiceRepositoryConstants.GetBatchAndSequenceNumber, parameters);

            batchNumber = Convert.ToInt32(parameters[2].Value);
            sequenceNumber = Convert.ToInt32(parameters[3].Value);
        }

        ///// <summary>
        ///// Updates the file log and invoice status depending on Validation Exception details.
        ///// </summary>
        ///// <param name="invoiceId"></param> 
        //public void UpdateInvoiceAndSetLaParameters(Guid invoiceId)
        //{
        //    var parameters = new ObjectParameter[1];
        //    parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, invoiceId);
        //    ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdateInvoiceSetLaParametersFunctionName, parameters);

        //}

        /// <summary>
        /// Deletes the RM AWB and re-sequences the breakdown serial numbers of the subsequent AWBs.
        /// </summary>
        /// <param name="rmAwbId">The RM AWB id.</param>
        public void DeleteRejectionMemoAwb(Guid rmAwbId)
        {
            var parameters = new ObjectParameter[1];
            parameters[0] = new ObjectParameter(CargoInvoiceRepositoryConstants.RejectionMemoAwbId, typeof(Guid)) { Value = rmAwbId };

            ExecuteStoredProcedure(CargoInvoiceRepositoryConstants.DeleteRejectionMemoAwb, parameters);
        }

        /// <summary>
        /// Deletes the BM AWB and re-sequences the breakdown serial numbers of the subsequent AWBs.
        /// </summary>
        /// <param name="bmAwbId">The BM AWB id.</param>
        public void DeleteBillingMemoAwb(Guid bmAwbId)
        {
            var parameters = new ObjectParameter[1];
            parameters[0] = new ObjectParameter(CargoInvoiceRepositoryConstants.BillingMemoAwbId, typeof(Guid)) { Value = bmAwbId };

            ExecuteStoredProcedure(CargoInvoiceRepositoryConstants.DeleteBillingMemoAwb, parameters);
        }

        /// <summary>
        /// Deletes the CM AWB and re-sequences the breakdown serial numbers of the subsequent AWBs.
        /// </summary>
        /// <param name="cmAwbId">The CM AWB id.</param>
        public void DeleteCreditMemoAwb(Guid cmAwbId)
        {
            var parameters = new ObjectParameter[1];
            parameters[0] = new ObjectParameter(CargoInvoiceRepositoryConstants.CreditMemoAwbId, typeof(Guid)) { Value = cmAwbId };

            ExecuteStoredProcedure(CargoInvoiceRepositoryConstants.DeleteCreditMemoAwb, parameters);
        }

        /// <summary>
        /// SCP85037
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNo"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        public List<CargoInvoiceSearchDetails> GetCargoManageInvoices(Model.Cargo.SearchCriteria searchCriteria, int pageSize, int pageNo, string sortColumn, string sortOrder)
        {
            var parameters = new ObjectParameter[15];
            parameters[0] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int)) { Value = searchCriteria.BillingMemberId };
            parameters[1] = new ObjectParameter("BILLING_MONTH_I", typeof(int)) { Value = searchCriteria.BillingMonth };
            parameters[2] = new ObjectParameter("BILLING_YEAR_I", typeof(int)) { Value = searchCriteria.BillingYear };

            parameters[3] = new ObjectParameter("INVOICE_STAUS_ID_I", typeof(int)) { Value = searchCriteria.InvoiceStatusId };
            parameters[4] = new ObjectParameter("BILLING_PERIOD_I", typeof(int)) { Value = searchCriteria.BillingPeriod };

            parameters[5] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(int)) { Value = searchCriteria.BilledMemberId };
            parameters[6] = new ObjectParameter("SETTELMENT_METHOD_ID_I", typeof(int)) { Value = searchCriteria.SettlementMethodId };
            parameters[7] = new ObjectParameter("SUBMISSION_METHOD_ID_I", typeof(int)) { Value = searchCriteria.SubmissionMethodId };
            parameters[8] = new ObjectParameter("OWNER_ID_I", typeof(int)) { Value = searchCriteria.OwnerId };
            parameters[9] = new ObjectParameter("PAGE_SIZE_I", typeof(int)) { Value = pageSize };
            parameters[10] = new ObjectParameter("FILENAME_I", typeof(string)) { Value = searchCriteria.FileName };
            parameters[11] = new ObjectParameter("INVOICE_NO_I", typeof(string)) { Value = searchCriteria.InvoiceNumber };

            parameters[12] = new ObjectParameter("PAGE_NO_I", typeof(int)) { Value = pageNo };

            parameters[13] = new ObjectParameter("SORT_COLUMN_I", typeof(string)) { Value = sortColumn };
            parameters[14] = new ObjectParameter("SORT_ORDER_I", typeof(string)) { Value = sortOrder };

            return ExecuteStoredFunction<CargoInvoiceSearchDetails>("GetCargoManageInvoices", parameters).ToList();
        }

        /// <summary>
        /// This function is used to get linked correspondence rejection memo list.
        /// </summary>
        /// <param name="CorrespondenceRefNo"></param>
        /// <returns></returns>
        //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
        public List<CgoLinkedCorrRejectionSearchData> GetLinkedCorrRejectionSearchResult(Guid correspondenceId)
        {
            var parameters = new ObjectParameter[1];

            parameters[0] = new ObjectParameter("CORRESPONDENCE_ID_I", typeof(Guid)) { Value = correspondenceId };

            //Execute stored procedure and fetch data based on criteria.
            var linkedCorrRejectionList = ExecuteStoredFunction<CgoLinkedCorrRejectionSearchData>("GetCgoLinkedCorrRejectionSearchResult", parameters);

            return linkedCorrRejectionList.ToList();
        }
    }
}
