using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfile
{
  public class ProfileMetadata : EntityBase<int>
  {
    public string TableName { get; set; }

    public string FieldName { get; set; }

    public string FieldType { get; set; }

    public string Description { get; set; }

    public bool IsFieldRequireForReceivingInvoices { get; set; }

    public bool IsOalSpecific { get; set; }

    public bool IsBilateral { get; set; }

    public int ChangeEffectiveYear { get; set; }

    public int ChangeEffectiveMonth { get; set; }

    public int ChangeEffectivePeriod { get; set; }

    public bool ToIncludeInIchOrAchOpsMemberReport { get; set; }

    public bool ToIncludeInIchOrAchOpsContactReport { get; set; }

    public bool ToIncludeInIsAccountMemberDetailReport { get; set; }

    public bool ToIncludeInIsAccountContactDetailReport { get; set; }

    public bool ToIncludeInIsAccountOwnMemberDetailReport { get; set; }

    public bool ToIncludeInIsAccountOwnContactDetailReport { get; set; }

    public bool ToIncludeInIchMemberProfileXml { get; set; }

    public string Notes { get; set; }

		public bool IsViewableToMember { get; set; }

		public bool IsViewableToIch { get; set; }

		public bool IsViewableToAch { get; set; }

		public bool IsViewableToSis { get; set; }

		public bool IsEditableToMember { get; set; }

		public bool IsEditableToIch { get; set; }

		public bool IsEditableToAch { get; set; }

		public bool IsEditableToSis { get; set; }

    public bool IchOpsMemRpt { get; set; }

    public bool IchOpsContactRpt { get; set; }

    public string FieldDisplayName { get; set; }


  }
}
