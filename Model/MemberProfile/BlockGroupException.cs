using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfile
{
    public class BlockGroupException : EntityBase<int>
    {
        public int BlockGroupId { get; set; }

        public BlockGroup BlockGroup { get; set; }

        public Member ExceptionMember { get; set; }

        public int ExceptionMemberId { get; set; }

        public string ExceptionMemberText { get; set; }

        public string DisplayMemberCode
        {
            get
            {
                return ExceptionMember != null ? ExceptionMember.MemberCodeAlpha + "-" + ExceptionMember.MemberCodeNumeric : string.Empty;
            }
        }

        public string DisplayMemberCommercialName
        {
            get
            {
                return ExceptionMember != null ? ExceptionMember.CommercialName : string.Empty;
            }
        }

        // Property to get and set whether BlockGroupException row is deleted on client side. 
        public bool IsDeleted { get; set; }
    }
}
