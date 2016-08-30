namespace Iata.IS.Model.Pax.Enums
{
    public enum TransactionDbStatus
    {
        /// <summary>
        /// None
        /// </summary>
        /// <remarks>
        /// Represents that is not set.
        /// </remarks>
        NotSet = 0,

        /// <summary>
        /// Found in DB.
        /// </summary>
        Found = 1,

        /// <summary>
        /// Not Found in DB.
        /// </summary>
        NotFound = 2
        
    }
}
