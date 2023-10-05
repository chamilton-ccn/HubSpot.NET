using System.Runtime.Serialization;

namespace HubSpot.NET.Api.Association.Dto
{
    /// <summary>
    /// See: <a href="https://developers.hubspot.com/docs/api/crm/associations#association-type-id-values">
    /// Association Type IDs
    /// </a>
    /// For more information
    /// </summary>
    public enum AssociationType
    {
        [EnumMember(Value = "1")]
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
        CompanyToPostalMail
    }
}