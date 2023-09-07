using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TSVData", menuName = "ScriptableObjects/TSVData", order = 2)]
public class TSVData : ScriptableObject
{
    public TextAsset sinaDataText;
    public TextAsset localDataText;
    public TextAsset backGroundDataText;
    public TextAsset stuffDataText;
    public TextAsset stuffGradeDataText;
    public TextAsset storeDataText;
    public TextAsset cookDataText;
    public TextAsset questDataText;
    public TextAsset questConditionDataText;
    public TextAsset rewardDataText;
    public TextAsset tipDataText;
    public TextAsset skinDataText;
    public TextAsset resourceDataText;
}