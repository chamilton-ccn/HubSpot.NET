using System.Runtime.Serialization;

namespace HubSpot.NET.Api.Associations.Dto
{
    /// <summary>
    /// See: <a href="https://developers.hubspot.com/docs/api/crm/associations#association-type-id-values">
    /// Association Type IDs
    /// </a>
    /// For more information
    /// </summary>
    [DataContract]
    public enum AssociationType
    {
        [EnumMember(Value = "1")]
        ContactToCompanyPrimary = 1,
        [EnumMember(Value = "2")]
        CompanyToContactPrimary = 2,
        [EnumMember(Value = "3")]
        DealToContact = 3,
        [EnumMember(Value = "4")]
        ContactToDeal = 4,
        [EnumMember(Value = "5")]
        DealToCompanyPrimary = 5,
        [EnumMember(Value = "6")]
        CompanyToDealPrimary = 6,
        [EnumMember(Value = "15")]
        ContactToTicket = 15,
        [EnumMember(Value = "16")]
        TicketToContact = 16,
        [EnumMember(Value = "25")]
        CompanyToTicketPrimary = 25,
        [EnumMember(Value = "26")]
        TicketToCompanyPrimary = 26,
        [EnumMember(Value = "27")]
        DealToTicket = 27,
        [EnumMember(Value = "28")]
        TicketToDeal = 28,
        [EnumMember(Value = "82")]
        ContactToCommunication = 82,
        [EnumMember(Value = "84")]
        TicketToCommunication = 84,
        [EnumMember(Value = "86")]
        DealToCommunication = 86,
        [EnumMember(Value = "88")]
        CompanyToCommunication = 88,
        [EnumMember(Value = "181")]
        CompanyToCall = 181,
        [EnumMember(Value = "185")]
        CompanyToEmail = 185,
        [EnumMember(Value = "187")]
        CompanyToMeeting = 187,
        [EnumMember(Value = "189")]
        CompanyToNote = 189,
        [EnumMember(Value = "191")]
        CompanyToTask = 191,
        [EnumMember(Value = "193")]
        ContactToCall = 193,
        [EnumMember(Value = "197")]
        ContactToEmail = 197,
        [EnumMember(Value = "199")]
        ContactToMeeting = 199,
        [EnumMember(Value = "201")]
        ContactToNote = 201,
        [EnumMember(Value = "203")]
        ContactToTask = 203,
        [EnumMember(Value = "205")]
        DealToCall = 205,
        [EnumMember(Value = "209")]
        DealToEmail = 209,
        [EnumMember(Value = "211")]
        DealToMeeting = 211,
        [EnumMember(Value = "213")]
        DealToNote = 213,
        [EnumMember(Value = "215")]
        DealToTask = 215,
        [EnumMember(Value = "219")]
        TicketToCall = 219,
        [EnumMember(Value = "223")]
        TicketToEmail = 223,
        [EnumMember(Value = "225")]
        TicketToMeeting = 225,
        [EnumMember(Value = "227")]
        TicketToNote = 227,
        [EnumMember(Value = "229")]
        TicketToTask = 229,
        [EnumMember(Value = "279")]
        ContactToCompany = 279,
        [EnumMember(Value = "280")]
        CompanyToContact = 280,
        [EnumMember(Value = "339")]
        TicketToCompany = 339,
        [EnumMember(Value = "340")]
        CompanyToTicket = 340,
        [EnumMember(Value = "341")]
        DealToCompany = 341,
        [EnumMember(Value = "342")]
        CompanyToDeal = 342,
        [EnumMember(Value = "454")]
        ContactToPostalMail = 454,
        [EnumMember(Value = "456")]
        TicketToPostalMail = 456,
        [EnumMember(Value = "458")]
        DealToPostalMail = 458,
        [EnumMember(Value = "460")]
        CompanyToPostalMail = 460
    }
}