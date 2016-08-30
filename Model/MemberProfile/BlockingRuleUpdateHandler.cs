using System.Collections.Generic;

namespace Iata.IS.Model.MemberProfile
{
    public class BlockingRuleUpdate
    {
        // Property to get and set BlockingRuleId
        public int RuleId { get; set; }

        // Property to get and set Blocking Rule description
        public string RuleDescription { get; set; }

        // Property to get and set MemberCode
        public string MemberCode { get; set; }

        // Property to get and set BlockedCreditors collection
        public List<Creditor> BlockedCreditors { get; set; }

        // Property to get and set BlockedDebtors collection
        public List<Debtor> BlockedDebtors { get; set; }

        // Property to get and set BlockedByGroup collection
        public BlockedByGroup BlockedByGroup { get; set; }

        // Property to get and set Deleted flag
        public bool Deleted { get; set; }
    }// end BlockingRuleUpdateHandler class

    public class Creditor
    {
        // Property to get and set MemberCode
        public string MemberCode { get; set; }

        // Property to get and set ZoneId
        public string Zone { get; set; }

        // Property to get and set BillingCategory Id
        public string BillingCategory { get; set; }

        // Property to get and set Exceptions
        public List<Exceptions> Exceptions { get; set; }
    }// end BlockedCreditors class 

    public class Debtor
    {
        // Property to get and set MemberCode
        public string MemberCode { get; set; }

        // Property to get and set ZoneId
        public string Zone { get; set; }

        // Property to get and set BillingCategory Id
        public string BillingCategory { get; set; }

        // Property to get and set Exceptions
        public List<Exceptions> Exceptions { get; set; }
    }// end BlockedDebtors class

    public class BlockedByGroup
    {
        // Proprty to get and set BlockedByGroup Creditors
        public List<Creditor> BlockedCreditors { get; set; }

        // Proprty to get and set BlockedByGroup Debtors
        public List<Debtor> BlockedDebtors { get; set; }

    }// end BlockedByGroup class

    public class Exceptions
    {
        public string MemberCode { get; set; }
    }// end class Exceptions
}// end namespace
