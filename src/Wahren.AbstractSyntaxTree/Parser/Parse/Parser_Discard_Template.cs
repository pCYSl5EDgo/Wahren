﻿#nullable enable
// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY T4. DO NOT CHANGE IT. CHANGE THE .tt FILE INSTEAD.
// </auto-generated>
namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
	public static bool Parse_Discard(ref Context context, ref Result result, uint elementTokenId, ReadOnlySpan<char> elementRest, ulong element4)
	{
		switch (elementRest.Length)
		{
			case 0:
				switch (element4)
				{
					case 0x0000007200740073UL: goto LOYAL; // Wahren.AbstractSyntaxTree.TextTemplateHelper.ElementInfo
					case 0x0000007900610072UL: goto RAY; // Wahren.AbstractSyntaxTree.TextTemplateHelper.ElementInfo
					case 0x00790065006B0066UL: goto LOYAL; // Wahren.AbstractSyntaxTree.TextTemplateHelper.ElementInfo
					case 0x0074007800650074UL: goto TEXT; // Wahren.AbstractSyntaxTree.TextTemplateHelper.ElementInfo
					case 0x006E006F00630069UL: goto CONSTI; // Wahren.AbstractSyntaxTree.TextTemplateHelper.ElementInfo
					case 0x0065007600610077UL: goto CONSTI; // Wahren.AbstractSyntaxTree.TextTemplateHelper.ElementInfo
					case 0x006D0061006F0072UL: goto ROAM; // Wahren.AbstractSyntaxTree.TextTemplateHelper.ElementInfo
					case 0x0074006F00700073UL: goto ROAM; // Wahren.AbstractSyntaxTree.TextTemplateHelper.ElementInfo
					case 0x0032006400640061UL: goto MEMBER; // Wahren.AbstractSyntaxTree.TextTemplateHelper.ElementInfo
					case 0x006D006500740069UL: goto MEMBER; // Wahren.AbstractSyntaxTree.TextTemplateHelper.ElementInfo
					case 0x0069006C006F0070UL: goto RAY; // Wahren.AbstractSyntaxTree.TextTemplateHelper.ElementInfo
					case 0x0070006D00610063UL: goto RAY; // Wahren.AbstractSyntaxTree.TextTemplateHelper.ElementInfo
					case 0x0065006D006F0068UL: goto RAY; // Wahren.AbstractSyntaxTree.TextTemplateHelper.ElementInfo
				}
				goto default;
			case 1:
				switch (element4)
				{
					case 0x00610079006F006CUL when elementRest[0] == 'l': goto LOYAL; // loyal
					case 0x0076006100720062UL when elementRest[0] == 'e': goto LOYAL; // brave
					case 0x0069007400750063UL when elementRest[0] == 'n': goto CONSTI; // cutin
					case 0x006C007000690064UL when elementRest[0] == 'o': goto CONSTI; // diplo
					case 0x00650077006F0070UL when elementRest[0] == 'r': goto ROAM; // power
					case 0x006300720065006DUL when elementRest[0] == 'e': goto MEMBER; // merce
					case 0x007400780065006EUL when elementRest[0] == '2': goto MEMBER; // next2
					case 0x007400780065006EUL when elementRest[0] == '3': goto MEMBER; // next3
					case 0x006E0075006F0073UL when elementRest[0] == 'd': goto MEMBER; // sound
					case 0x006D0065006E0065UL when elementRest[0] == 'y': goto OFFSET; // enemy
					case 0x0066006100740073UL when elementRest[0] == 'f': goto OFFSET; // staff
					case 0x0074006C0075006DUL when elementRest[0] == 'i': goto RAY; // multi
					case 0x007200610065006CUL when elementRest[0] == 'n': goto RAY; // learn
					case 0x006F006C006F0063UL when elementRest[0] == 'r': goto RAY; // color
					case 0x006E0069006F006AUL when elementRest[0] == 't': goto RAY; // joint
					case 0x006C0069006B0073UL when elementRest[0] == 'l': goto RAY; // skill
				}
				goto default;
			case 2:
				switch (element4)
				{
					case 0x0065006200720061UL when elementRest.SequenceEqual("it"): goto LOYAL; // arbeit
					case 0x006E006100680063UL when elementRest.SequenceEqual("ge"): goto LOYAL; // change
					case 0x0075006F00720067UL when elementRest.SequenceEqual("nd"): goto LOYAL; // ground
					case 0x0073006E006F0063UL when elementRest.SequenceEqual("ti"): goto CONSTI; // consti
					case 0x006700610065006CUL when elementRest.SequenceEqual("ue"): goto CONSTI; // league
					case 0x00610079006F006CUL when elementRest.SequenceEqual("ls"): goto CONSTI; // loyals
					case 0x006900720065006DUL when elementRest.SequenceEqual("ts"): goto CONSTI; // merits
					case 0x006F0072006F0079UL when elementRest.SequenceEqual("zu"): goto CONSTI; // yorozu
					case 0x0062006D0065006DUL when elementRest.SequenceEqual("er"): goto MEMBER; // member
					case 0x0065006900720066UL when elementRest.SequenceEqual("nd"): goto OFFSET; // friend
					case 0x007300660066006FUL when elementRest.SequenceEqual("et"): goto OFFSET; // offset
					case 0x006C0069006B0073UL when elementRest.SequenceEqual("l2"): goto RAY; // skill2
					case 0x0070006100650077UL when elementRest.SequenceEqual("on"): goto RAY; // weapon
				}
				goto default;
			case 3:
				switch (element4)
				{
					case 0x0073006E006F006DUL when elementRest.SequenceEqual("ter"): goto MEMBER; // monster
					case 0x0070006100650077UL when elementRest.SequenceEqual("on2"): goto RAY; // weapon2
				}
				goto default;
			case 4:
				switch (element4)
				{
					case 0x0073006C00650064UL when elementRest.SequenceEqual("kill"): goto OFFSET; // delskill
				}
				goto default;
			case 5:
				switch (element4)
				{
					case 0x005F006E00750067UL when elementRest.SequenceEqual("delay"): goto LOYAL; // gun_delay
					case 0x006D006500740069UL when elementRest.SequenceEqual("_hold"): goto MEMBER; // item_hold
					case 0x006D006500740069UL when elementRest.SequenceEqual("_sale"): goto MEMBER; // item_sale
					case 0x007400730075006AUL when elementRest.SequenceEqual("_next"): goto MEMBER; // just_next
					case 0x0073006C00650064UL when elementRest.SequenceEqual("kill2"): goto OFFSET; // delskill2
					case 0x0069007400630061UL when elementRest.SequenceEqual("venum"): goto RAY; // activenum
					case 0x0065006900720066UL when elementRest.SequenceEqual("nd_ex"): goto RAY; // friend_ex
				}
				goto default;
			case 6:
				switch (element4)
				{
					case 0x00630069006F0076UL when elementRest.SequenceEqual("e_type"): goto OFFSET; // voice_type
				}
				goto default;
			case 7:
				switch (element4)
				{
					case 0x006D0065006E0065UL when elementRest.SequenceEqual("y_power"): goto CONSTI; // enemy_power
				}
				goto default;
			case 8:
				switch (element4)
				{
					case 0x006400610065006CUL when elementRest.SequenceEqual("er_skill"): goto CONSTI; // leader_skill
					case 0x0069007300730061UL when elementRest.SequenceEqual("st_skill"): goto CONSTI; // assist_skill
					case 0x0074007300610063UL when elementRest.SequenceEqual("le_guard"): goto MEMBER; // castle_guard
				}
				goto default;
			default: return Parse_Discard_DEFAULT(ref context, ref result, elementTokenId);
		}
    LOYAL: 
		return Parse_Discard_LOYAL(ref context, ref result, elementTokenId);
    TEXT: 
		return Parse_Discard_TEXT(ref context, ref result, elementTokenId);
    CONSTI: 
		return Parse_Discard_CONSTI(ref context, ref result, elementTokenId);
    ROAM: 
		return Parse_Discard_ROAM(ref context, ref result, elementTokenId);
    MEMBER: 
		return Parse_Discard_MEMBER(ref context, ref result, elementTokenId);
    OFFSET: 
		return Parse_Discard_OFFSET(ref context, ref result, elementTokenId);
    RAY: 
		return Parse_Discard_RAY(ref context, ref result, elementTokenId);
	}
}