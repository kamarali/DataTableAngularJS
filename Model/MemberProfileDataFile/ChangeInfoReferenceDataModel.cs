using System.ComponentModel;

namespace Iata.IS.Model.MemberProfileDataFile
{
  
    public class ChangeInfoReferenceDataModel
    {
        [DisplayName("Serial No.")]
        public int SerialNo { get; set; }
      
        [DisplayName("Action")]
        public string Action { get; set; }

        [DisplayName("Participant Code")]
        public string ParticipantCode { get; set; }
      
        [DisplayName("Location Id")]
        public string LocationId { get; set; }

        [DisplayName("Field Name")]
        public string FieldName { get; set; }

        [DisplayName("Old Value")]
        public string OldValue { get; set; }

        [DisplayName("New Value")]
        public string NewValue { get; set; }
    }
}
