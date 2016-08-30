using System;
using Iata.IS.Model.MemberProfile.Enums;

namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
  public class TechnicalConfiguration : ProfileEntity
  {
    private const int PleaseSelectId = 0;

    //[ProfilePermission(ControlType = ControlType.OutputFileDeliveryDropdownList, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "PAX_OP_FILES_DELIVERY_METH_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    //public int IspaxDeliveryMethodId { get; set; }
    //public string IspaxDeliveryMethodIdDisplayValue { get; set; }
    //public string IspaxDeliveryMethodIdFutureDisplayValue { get; set; }

    //public OutputFileDeliveryMethod IspaxDeliveryMethod
    //{
    //  get
    //  {
    //    return (OutputFileDeliveryMethod)IspaxDeliveryMethodId;
    //  }
    //  set
    //  {
    //    IspaxDeliveryMethodId = Convert.ToInt32(value);
    //  }
    //}

    //[ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "OP_SERVER_IP_ADDR_PAX", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    //public string IspaxOutputServerIpAddress { get; set; }

    //[ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "PAX_USER_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    //public string IspaxOutputServerUserId { get; set; }

    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "PAX_PASSWORD", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    //public string IspaxOutputServerPassword { get; set; }

    // CGO Related Technical Detail Properties

    //[ProfilePermission(ControlType = ControlType.OutputFileDeliveryDropdownList, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "CGO_OP_FILES_DELIVERY_METH_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    //public int IscgoDeliveryMethodId { get; set; }
    //public string IscgoDeliveryMethodIdDisplayValue { get; set; }
    //public string IscgoDeliveryMethodIdFutureDisplayValue { get; set; }

    //public OutputFileDeliveryMethod IscgoDeliveryMethod
    //{
    //  get
    //  {
    //    return (OutputFileDeliveryMethod)IscgoDeliveryMethodId;
    //  }
    //  set
    //  {
    //    IscgoDeliveryMethodId = Convert.ToInt32(value);
    //  }
    //}

    //[ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "OP_SERVER_IP_ADDR_CGO", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    //public string IscgoOutputServerIpAddress { get; set; }

    //[ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "CGO_USER_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    //public string IscgoOutputServerUserId { get; set; }

    //[ProfilePermission(ControlType = ControlType.Password, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "CGO_PASSWORD", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    //public string IscgoOutputServerPassword { get; set; }

    // UATP Related Technical Detail Properties

    //[ProfilePermission(ControlType = ControlType.OutputFileDeliveryDropdownList, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "MSC_OP_FILES_DELIVERY_METH_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    //public int IsuatpDeliveryMethodId { get; set; }
    //public string IsuatpDeliveryMethodIdDisplayValue { get; set; }
    //public string IsuatpDeliveryMethodIdFutureDisplayValue { get; set; }

    //public OutputFileDeliveryMethod IsuatpDeliveryMethod
    //{
    //  get
    //  {
    //    return (OutputFileDeliveryMethod)IsuatpDeliveryMethodId;
    //  }
    //  set
    //  {
    //    IsuatpDeliveryMethodId = Convert.ToInt32(value);
    //  }
    //}

    //[ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "OP_SERVER_IP_ADDR_UATP", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    //public string IsuatpOutputServerIpAddress { get; set; }

    //[ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "UATP_USER_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    //public string IsuatpOutputServerUserId { get; set; }

    //[ProfilePermission(ControlType = ControlType.Password, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "UATP_PASSWORD", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    //public string IsuatpOutputServerPassword { get; set; }

    // MISC Related Technical Detail Properties
    //[ProfilePermission(ControlType = ControlType.OutputFileDeliveryDropdownList, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "MSC_OP_FILES_DELIVERY_METH_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    //public int IsmiscDeliveryMethodId { get; set; }
    //public string IsmiscDeliveryMethodIdDisplayValue { get; set; }
    //public string IsmiscDeliveryMethodIdFutureDisplayValue { get; set; }

    //public OutputFileDeliveryMethod IsmiscDeliveryMethod
    //{
    //  get
    //  {
    //    return (OutputFileDeliveryMethod)IsmiscDeliveryMethodId;
    //  }
    //  set
    //  {
    //    IsmiscDeliveryMethodId = Convert.ToInt32(value);
    //  }
    //}
    
    //[ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "OP_SERVER_IP_ADDR_MISC", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    //public string IsmiscOutputServerIpAddress { get; set; }

    //[ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "MSC_USER_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    //public string IsmiscOutputServerUserId { get; set; }

    //[ProfilePermission(ControlType = ControlType.Password, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "MSC_PASSWORD", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    //public string IsmiscOutputServerPassword { get; set; }

    [Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "IINET_MEMBER_CODE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    public string IiNetMemberCode { get; set; }

    public Member Member { get; set; }

    public int MemberId { get; set; }

    //[ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "PAX_IINET_FOLDER", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    //public string PaxIiNetFolder { get; set; }

    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    [Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "PAX_ACCOUNT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    public string PaxAccountId { get; set; }

    //[ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "CGO_IINET_FOLDER", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    //public string CgoIiNetFolder { get; set; }

    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    [Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "CGO_ACCOUNT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    public string CgoAccountId { get; set; }

    //[ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "MISC_IINET_FOLDER", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    //public string MiscIiNetFolder { get; set; }

    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    [Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "MISC_ACCOUNT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    public string MiscAccountId { get; set; }

    //[ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    //[Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "UATP_IINET_FOLDER", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    //public string UatpIiNetFolder { get; set; }

    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    [Audit(ElementGroup = ElementGroupType.Technical, ElementGroupDisplayName = "Technical",ElementTable = "MEM_TECHNICAL_CONFIGURATION", ElementName = "UATP_ACCOUNT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    public string UatpAccountId { get; set; }
  }
}