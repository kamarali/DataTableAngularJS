using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfile
{
    public class ContactContactTypeMatrix : EntityBase<string>
    {
        public Contact Contact { get; set; }
        public int ContactId { get; set; }
        public ContactType ContactType { get; set; }
        public int ContactTypeId { get; set; }

        /// <summary>
        /// Gets or sets the group id.
        /// </summary>
        /// <value>The group id.</value>
        public int GroupId { get; set; }
    }
}
