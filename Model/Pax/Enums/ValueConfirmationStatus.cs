namespace Iata.IS.Model.Pax.Enums
{
    public enum ValueConfirmationStatus
    {
        None = 0,
        /// <summary>
        /// Value Confirmation is Not Required.
        /// </summary>
        NotRequired = 1,

        /// <summary>
        /// Request Sent To ATPCO.
        /// </summary>
        Requested = 2,

        /// <summary>
        /// Processing successful.
        /// </summary>
        Completed = 3,

        /// <summary>
        /// Required But Not Requested
        /// </summary>
        RequiredButNotRequested = 4,

        /// <summary>
        /// Pending Status for intermediate Requested & RequiredButNotRequested
        /// </summary>
        Pending = 5

    }
}
