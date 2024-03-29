﻿#nullable enable
// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY T4. DO NOT CHANGE IT. CHANGE THE .tt FILE INSTEAD.
// </auto-generated>
namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
	public static bool Parse_Discard(ref Context context, ref Result result, uint elementTokenId, ulong hash)
	{
		switch (hash)
		{
			case 0x0000000000009A08UL: // str
			case 0x00000000000C050FUL: // fkey
			case 0x00000000026BCFD6UL: // loyal
			case 0x00000000014FAAADUL: // brave
			case 0x000000002C61FBD1UL: // arbeit
			case 0x000000003387A1FCUL: // change
			case 0x000000004538FF41UL: // ground
			case 0x000035C26C87D5CDUL: // gun_delay
				return Parse_Discard_LOYAL(ref context, ref result, elementTokenId);
			case 0x000000000016B9C9UL: // text
				return Parse_Discard_TEXT(ref context, ref result, elementTokenId);
			case 0x00000000000E2D45UL: // icon
			case 0x000000000018F5A3UL: // wave
			case 0x00000000016EF8F8UL: // cutin
			case 0x0000000001823699UL: // diplo
			case 0x000000003459F96EUL: // consti
			case 0x000000005864A53DUL: // league
			case 0x0000000059950A0AUL: // loyals
			case 0x000000005C93EE5AUL: // merits
			case 0x000000008F4B4AB6UL: // yorozu
			case 0x00F9F8A46910BCA5UL: // enemy_power
			case 0x34CDD9F7EEC7D69BUL: // leader_skill
			case 0x1A9C679DC1FF7621UL: // assist_skill
				return Parse_Discard_CONSTI(ref context, ref result, elementTokenId);
			case 0x000000000015602FUL: // roam
			case 0x0000000000162D72UL: // spot
			case 0x0000000002DE2982UL: // power
				return Parse_Discard_ROAM(ref context, ref result, elementTokenId);
			case 0x000000000008020AUL: // add2
			case 0x00000000000E86BBUL: // item
			case 0x00000000028088F9UL: // merce
			case 0x00000000029D4469UL: // next2
			case 0x00000000029D446AUL: // next3
			case 0x000000000333EAE2UL: // sound
			case 0x000000005C8FE96EUL: // member
			case 0x0000000D8A4BD447UL: // monster
			case 0x00003C0A5AAE4096UL: // item_hold
			case 0x00003C0A5AB67638UL: // item_sale
			case 0x00003F5ABF1C46D5UL: // just_next
			case 0x1E59378280F0B886UL: // castle_guard
				return Parse_Discard_MEMBER(ref context, ref result, elementTokenId);
			case 0x0000000001A27237UL: // enemy
			case 0x0000000003375C19UL: // staff
			case 0x000000004111EBAEUL: // friend
			case 0x0000000064EBA9E9UL: // offset
			case 0x000001280DF26B20UL: // delskill
			case 0x00002ACA04097BA2UL: // delskill2
			case 0x000E9E701D86CC99UL: // voice_type
				return Parse_Discard_OFFSET(ref context, ref result, elementTokenId);
			case 0x00000000000091F7UL: // ray
			case 0x000000000013D608UL: // poli
			case 0x0000000000097F1DUL: // camp
			case 0x00000000000DA741UL: // home
			case 0x00000000028CC92CUL: // multi
			case 0x0000000002639753UL: // learn
			case 0x00000000016A2BE4UL: // color
			case 0x0000000002324A4DUL: // joint
			case 0x0000000003309300UL: // skill
			case 0x0000000076053F02UL: // skill2
			case 0x0000000085DBFBB0UL: // weapon
			case 0x0000001358CB6072UL: // weapon2
			case 0x0000210CFE52C9D5UL: // activenum
			case 0x0000324AF6D61DE1UL: // friend_ex
				return Parse_Discard_RAY(ref context, ref result, elementTokenId);
			default:
				return Parse_Discard_DEFAULT(ref context, ref result, elementTokenId);
		}
	}
}