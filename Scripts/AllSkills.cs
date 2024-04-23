using System;
using UnityEngine;
// json data 구조, 모든 스킬들이 담기는 클래스.
// DataManager 클래스 에서 json 파일을 읽어서 이 클라스의 인스턴스를 리턴한다.
// Backend 클래스 에서 이 클래스의 인스턴스가 저장된다.


[Serializable]
public class AllSkills
{
    public SkillData[] ults;
    public NormList[] normals;

    [Serializable]
    public class NormList {
        public SkillData[] normgroup;
    }

    [Serializable]
    public class SkillData {
        public string skillID;
        public string skillName;
        public string skillType;
        public float skillFactor;
    }
}