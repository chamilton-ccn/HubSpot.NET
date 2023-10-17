using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

        /*[EnumMember(Value = "1")]
        ContactToCompanyPrimary,
        [EnumMember(Value = "2")]
        CompanyToContactPrimary,
        [EnumMember(Value = "3")]
        DealToContact,
        [EnumMember(Value = "4")]
        ContactToDeal,
        [EnumMember(Value = "5")]
        DealToCompanyPrimary,
        [EnumMember(Value = "6")]
        CompanyToDealPrimary,
        [EnumMember(Value = "15")]
        ContactToTicket,
        [EnumMember(Value = "16")]
        TicketToContact,
        [EnumMember(Value = "25")]
        CompanyToTicketPrimary,
        [EnumMember(Value = "26")]
        TicketToCompanyPrimary,
        [EnumMember(Value = "27")]
        DealToTicket,
        [EnumMember(Value = "28")]
        TicketToDeal,
        [EnumMember(Value = "82")]
        ContactToCommunication,
        [EnumMember(Value = "84")]
        TicketToCommunication,
        [EnumMember(Value = "86")]
        DealToCommunication,
        [EnumMember(Value = "88")]
        CompanyToCommunication,
        [EnumMember(Value = "181")]
        CompanyToCall,
        [EnumMember(Value = "185")]
        CompanyToEmail,
        [EnumMember(Value = "187")]
        CompanyToMeeting,
        [EnumMember(Value = "189")]
        CompanyToNote,
        [EnumMember(Value = "191")]
        CompanyToTask,
        [EnumMember(Value = "193")]
        ContactToCall,
        [EnumMember(Value = "197")]
        ContactToEmail,
        [EnumMember(Value = "199")]
        ContactToMeeting,
        [EnumMember(Value = "201")]
        ContactToNote,
        [EnumMember(Value = "203")]
        ContactToTask,
        [EnumMember(Value = "205")]
        DealToCall,
        [EnumMember(Value = "209")]
        DealToEmail,
        [EnumMember(Value = "211")]
        DealToMeeting,
        [EnumMember(Value = "213")]
        DealToNote,
        [EnumMember(Value = "215")]
        DealToTask,
        [EnumMember(Value = "219")]
        TicketToCall,
        [EnumMember(Value = "223")]
        TicketToEmail,
        [EnumMember(Value = "225")]
        TicketToMeeting,
        [EnumMember(Value = "227")]
        TicketToNote,
        [EnumMember(Value = "229")]
        TicketToTask,
        [EnumMember(Value = "279")]
        ContactToCompany,
        [EnumMember(Value = "280")]
        CompanyToContact,
        [EnumMember(Value = "339")]
        TicketToCompany,
        [EnumMember(Value = "340")]
        CompanyToTicket,
        [EnumMember(Value = "341")]
        DealToCompany,
        [EnumMember(Value = "342")]
        CompanyToDeal,
        [EnumMember(Value = "454")]
        ContactToPostalMail,
        [EnumMember(Value = "456")]
        TicketToPostalMail,
        [EnumMember(Value = "458")]
        DealToPostalMail,
        [EnumMember(Value = "460")]
        CompanyToPostalMail*/
    }
}