using System;
using UnityEngine;

[Serializable]
public class SectionData
{
    public Section nextSection;
    public Section previousSection;
    public Transform leftEnd;
    public Transform rightEnd;
}

public class Section : MonoBehaviour
{
    public Borders border;

    public Transform startPlatform;

    public SectionData sectionData;
}