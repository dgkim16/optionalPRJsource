using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party 
{
    public CharacterInterface[] party = new CharacterInterface[4];

    public CharacterInterface[] GetParty() {
        return party;
    }

    public void SetParty(CharacterInterface[] party) {
        this.party = party;
    }
    // helper variable containing character that is at the targeted position of party
    public static CharacterInterface charTarget;

    // place character in party. if party is full, remove the character in place of the new one (no need to work on this)
    // if character is already in party, change spot.
    // prompt asking if you want to change spot between characters in same party 추가 할수도 있음
    public void SetMember(int membNumb, CharacterInterface character) {
        int charCurrentPos = CheckSameParty(character);
        charTarget = GetParty()[membNumb];
        // 파티에 넣으려는 캐릭터가 있을경우
        if(charCurrentPos != -1) {
            // if(GetParty()[memberNumb] != null) {} // prompt asking if you want to change spot between characters in same party 추가 할수도 있음
            GetParty()[charCurrentPos] = null;
            GetParty()[membNumb] = null;
            GetParty()[charCurrentPos] = charTarget;
            GetParty()[membNumb] = character;
            return;
        }
        // 파티에 넣으려는 현재 캐릭터가 없을경우. 그 자리가 void 였던 누가 있었던 그 자리에 있던 캐릭터에 어느 party 속했는지 정보저장을 안하기에 그냥 replace
        GetParty()[membNumb] = character;
    }

    // 파티에 넣으려는 캐릭터가 현재 파티에 있는지 확인
    // 있다면 현재 캐릭터의 위치를 리턴
    // object == null 을 오류 없이 hanlde 가능하면 ok 아니면 다시 짜야함.
    public int CheckSameParty(CharacterInterface character) {
        int toReturn = 0;
        foreach(CharacterInterface chara in GetParty()) {
            if(chara == character) {
                return toReturn;
            }
            toReturn++;
        }
        return -1;  // 파티에 없음
    }

}
