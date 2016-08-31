using System.Collections.Generic;
using Iata.IS.Model.Common;


namespace Iata.IS.Business.Common
{
    public interface IInvPaymentStatusManager
    {
        /// <summary>
        /// Adds the InvPaymentStatus.
        /// </summary>
        /// <param name="miscPaymentStatus">The InvPaymentStatus</param>
        /// <returns></returns>
        InvPaymentStatus AddInvPaymentStatus(InvPaymentStatus miscPaymentStatus);

        /// <summary>
        /// Delete the specified Invoice Payment Status by id
        /// </summary>
        /// <param name="id"></param>
        void DeleteInvPaymentStatus(int id);

        /// <summary>
        /// Updates the Invoice Payment Status.
        /// </summary>
        /// <param name="invPaymentStatus">The invPaymentStatus.</param>
        /// <returns></returns>
        InvPaymentStatus UpdateInvPaymentStatus(InvPaymentStatus invPaymentStatus);


        /// <summary>
        /// Gets the Invoice Payment Status details.
        /// </summary>
        /// <param name="id">The Invoice Payment Status id.</param>
        /// <returns></returns>
        InvPaymentStatus GetInvPaymentStatusDetails(int id);

        /// <summary>
        /// Gets the Invoice Payment List based on description
        /// </summary>
        /// <param name="description">The description</param>
        /// <param name="applicableFor">The Invoice Payment Status Applicable For.</param>
        /// <returns></returns>
        List<InvPaymentStatus> GetInvPaymentStatusList(string description, int applicableFor);


        /// <summary>
        /// Gets all Invoice Payment Status list.
        /// </summary>
        /// <returns></returns>
        List<InvPaymentStatus> GetAllInvPaymentStatusList();


    }
}
