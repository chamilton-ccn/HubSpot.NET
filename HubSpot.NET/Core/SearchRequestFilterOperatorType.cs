using System.Runtime.Serialization;

namespace HubSpot.NET.Core
{
	public enum SearchRequestFilterOperatorType
	{
		[EnumMember(Value = "LT")]
		LessThan,

		[EnumMember(Value = "LTE")]
		LessThanOrEqualTo,

		[EnumMember(Value = "GT")]
		GreaterThan,

		[EnumMember(Value = "GTE")]
		GreaterThanOrEqualTo,

		[EnumMember(Value = "EQ")]
		EqualTo,

		[EnumMember(Value = "NEQ")]
		NotEqualTo,
		
		/// <remarks>
		/// The "BETWEEN" operator is inclusive.
		/// </remarks>
		[EnumMember(Value = "BETWEEN")]
		Between,
		
		[EnumMember(Value = "IN")]
		In,
		
		[EnumMember(Value = "NOT_IN")]
		NotIn,

		[EnumMember(Value = "HAS_PROPERTY")]
		HasAValue,

		[EnumMember(Value = "NOT_HAS_PROPERTY")]
		DoesNotHaveAValue,

		[EnumMember(Value = "CONTAINS_TOKEN")]
		ContainsAToken,

		[EnumMember(Value = "NOT_CONTAINS_TOKEN")]
		DoesNotContainAToken
	}
}