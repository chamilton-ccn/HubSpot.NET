using System;
using System.Runtime.Serialization;

// TODO - marked for removal
namespace HubSpot.NET.Api.Contact.Dto
{
	[Obsolete("DEPRECATED: Use SearchRequestSort instead.")]
	public enum SortingOrderType
	{
		[EnumMember(Value = "ASC")]
		Ascending,

		[EnumMember(Value = "DESC")]
		Descending
	}
}