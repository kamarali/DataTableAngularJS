using Iata.IS.Model.Base;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Model.Common
{
  public class OldIdecParticipation : MasterBase<int>
  {
    public int MemberId { get; set; }

    public bool PaxIncomingAllowed { get; set; }

    public bool PaxOutGoingAllowed { get; set; }

    public bool CgoIncomingAllowed { get; set; }

    public bool CgoOutGoingAllowed { get; set; }

    public Member Member { get; set; }

    private string _memberText;
    public string MemberText
    {
        get
        {
            return !string.IsNullOrEmpty(_memberText) ? _memberText : Member != null ? string.Format("{0}-{1}-{2}", Member.MemberCodeAlpha, Member.MemberCodeNumeric, Member.CommercialName) : string.Empty;
        }
        set
        {
            _memberText = value;
        }
    }
  }
}
