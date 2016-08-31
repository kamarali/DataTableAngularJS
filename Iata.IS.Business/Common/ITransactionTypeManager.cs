using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax;

namespace Iata.IS.Business.Common
{
    public interface ITransactionTypeManager
    {
        /// <summary>
        /// Adds the type of the transaction.
        /// </summary>
        /// <param name="transactionType">Type of the transaction.</param>
        /// <returns></returns>
         TransactionType AddTransactionType( TransactionType  transactionType);

         /// <summary>
         /// Updates the type of the transaction.
         /// </summary>
         /// <param name="transactionType">Type of the transaction.</param>
         /// <returns></returns>
         TransactionType UpdateTransactionType( TransactionType  transactionType);

         /// <summary>
         /// Deletes the type of the transaction.
         /// </summary>
         /// <param name="transactionTypeId">The transaction type id.</param>
         /// <returns></returns>
        bool DeleteTransactionType(int  transactionTypeId);

        /// <summary>
        /// Gets the transaction type details.
        /// </summary>
        /// <param name="transactionTypeId">The transaction type id.</param>
        /// <returns></returns>
         TransactionType GetTransactionTypeDetails(int  transactionTypeId);

         /// <summary>
         /// Gets all transaction type list.
         /// </summary>
         /// <returns></returns>
        List< TransactionType> GetAllTransactionTypeList();

        /// <summary>
        /// Gets the transaction type list.
        /// </summary>
        /// <param name="BillingCategoryCode">The billing category code.</param>
        /// <param name="Description">The description.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        List<TransactionType> GetTransactionTypeList(int BillingCategoryCode, string Description, string Name);
    }
}
