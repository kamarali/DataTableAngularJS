using System.Collections.Generic;
using System.Linq;
using Iata.IS.Data;
using Iata.IS.Model.Common;
using Iata.IS.Data.Impl;

namespace Iata.IS.Business.Common.Impl
{
    public class InvPaymentStatusManager : IInvPaymentStatusManager 
    {
        /// <summary>
        /// Gets or sets the InvPaymentStatus repository.
        /// </summary>
        /// <value>
        /// The InvPaymentStatus repository.
        /// </value>
        public IRepository<InvPaymentStatus> InvPaymentStatusRepository { get; set; }


        /// <summary>
        /// Adds the InvPaymentStatus.
        /// </summary>
        /// <param name="invPaymentStatus">The InvPaymentStatus.</param>
        /// <returns></returns>
        public InvPaymentStatus AddInvPaymentStatus(InvPaymentStatus invPaymentStatus)
        {
            var invPaymentStatusData = InvPaymentStatusRepository.Single(type => type.Id == invPaymentStatus.Id);
            
            //Call repository method for adding InvPaymentStatus
            InvPaymentStatusRepository.Add(invPaymentStatus);
            UnitOfWork.CommitDefault();
            return invPaymentStatus;
        }

        /// <summary>
        /// Updates the Invoice Payment Status.
        /// </summary>
        /// <param name="invPaymentStatus">The invPaymentStatus.</param>
        /// <returns></returns>
        public InvPaymentStatus UpdateInvPaymentStatus(InvPaymentStatus invPaymentStatus)
        {
            var miscPaymentStatusData = InvPaymentStatusRepository.Single(type => type.Id == invPaymentStatus.Id);

            var updatedmiscPaymentStatus = InvPaymentStatusRepository.Update(invPaymentStatus);
            UnitOfWork.CommitDefault();
            return updatedmiscPaymentStatus;
        }

        /// <summary>
        /// Gets the Invoice Payment Status Details By id
        /// </summary>
        /// <param name="id">The Invoice Payment Status id.</param>
        /// <returns></returns>
        public InvPaymentStatus GetInvPaymentStatusDetails(int id)
        {
            var invPaymentStatus = InvPaymentStatusRepository.Single(type => type.Id == id);
            return invPaymentStatus;
        }

        /// <summary>
        /// Gets all Invoice Payment Status list.
        /// </summary>
        /// <param name="description">The Invoice Payment Status description.</param>
        /// <param name="applicableFor">The Invoice Payment Status Applicable For.</param>
        /// <returns></returns>
        public List<InvPaymentStatus> GetInvPaymentStatusList(string description, int applicableFor)
        {
            var invPaymentStatusList = new List<InvPaymentStatus>();

            invPaymentStatusList = InvPaymentStatusRepository.GetAll().ToList();

            invPaymentStatusList = invPaymentStatusList.Where(c1 => c1.IsSystemDefined == false).ToList();

            if (!string.IsNullOrEmpty(description))
            {
                invPaymentStatusList = invPaymentStatusList.Where(cl => cl.Description.ToLower().Contains(description.ToLower())).ToList();
            }

            if (applicableFor > 0)
            {
                invPaymentStatusList = invPaymentStatusList.Where(cl => cl.ApplicableFor.Equals(applicableFor)).ToList();
            }

            return invPaymentStatusList.ToList();
        }

        

        /// <summary>
        /// Gets all Invoice Payment Status list.
        /// </summary>
        /// <returns></returns>
        public List<InvPaymentStatus> GetAllInvPaymentStatusList()
        {
            var miscPaymentStatusList = InvPaymentStatusRepository.GetAll();
            return miscPaymentStatusList.ToList();
        }

        /// <summary>
        /// Deletes the Invoice Payment Status.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void DeleteInvPaymentStatus(int id)
        {
            var miscPaymentStatus = InvPaymentStatusRepository.Single(type => type.Id == id);
            if (miscPaymentStatus != null)
            {
                miscPaymentStatus.IsActive = !(miscPaymentStatus.IsActive);
                InvPaymentStatusRepository.Update(miscPaymentStatus);
                UnitOfWork.CommitDefault();
            }
        }
    }
}
