using System;
using System.Linq;
using UnityEngine;
using Utils;

public class InfiniteManager : MonoBehaviour
{
    public Section[] Sections;

    [SerializeField] private float movementSpeed;

    private Vector3[] defaultSectionPositions;
    private Section[] defaultSections;

    private Dispatcher _dispatcher;

    [SerializeField] private Section currentSection;
    [SerializeField] private Section nextSection;

    [SerializeField] private Character lionCharacter;

    private int counter;

    private void OnEnable()
    {
        _dispatcher = GameManager.Dispatcher;
        _dispatcher.Subscribe(EventId.ResetLevel, ResetSections);
    }

    private void Awake()
    {
        SetDefaultPositions();
        AssignSectionData();
    }

    private void Start()
    {
        currentSection = Sections[0];
        nextSection = currentSection.sectionData.nextSection;
        lionCharacter = GameManager.Instance.lionCharacter;
    }

    private void OnDisable()
    {
        _dispatcher.Unsubscribe(EventId.ResetLevel, ResetSections);
    }

    private void LateUpdate()
    {
        if (!GameManager.Instance.IsStarted)
            return;


        MoveSections();
        CheckPlayerForCurrentSection();
    }

    public void CheckPlayerForCurrentSection()
    {
        Vector2 position = lionCharacter.transform.position;

        if (currentSection == null)
            return;
        SectionData nextSectionData = currentSection.sectionData.nextSection.sectionData;
        Vector2 leftEnd = nextSectionData.leftEnd.position;
        Vector2 rightEnd = nextSectionData.rightEnd.position;

        if (position.x > leftEnd.x && position.x < rightEnd.x)
        {
            if (currentSection != nextSection && nextSection != null)
            {
                Debug.Log("Current Section Changed to " + nextSection.transform.name);
                currentSection = nextSection;
                nextSection = currentSection.sectionData.nextSection;
                counter++;
                if (counter >= 2 && currentSection.sectionData.previousSection.sectionData.previousSection == nextSection)
                {
                    if (currentSection.startPlatform.gameObject.activeInHierarchy)
                    {
                        currentSection.startPlatform.gameObject.SetActive(false);
                    }
                    Vector3 nextTargetPosition = currentSection.sectionData.rightEnd.position;
                    nextTargetPosition.z = 0f;
                    nextTargetPosition.y = 0f;
                    currentSection.sectionData.nextSection.transform.position = nextTargetPosition;
                    Sections = Sections.OrderBy(section => section.transform.position.x).ToArray();
                    AssignSectionData();
                }
            }
        }
    }

    private void AssignSectionData()
    {
        for (int s = 0; s < Sections.Length - 1; s++)
        {
            if (s == 0)
            {
                Sections[s].sectionData.previousSection = Sections[^1];
                Sections[^1].sectionData.nextSection = Sections[s];
            }

            Sections[s].sectionData.nextSection = Sections[s + 1];
            Sections[s + 1].sectionData.previousSection = Sections[s];
        }
    }

    private void SetDefaultPositions()
    {
        defaultSectionPositions = new Vector3[Sections.Length];
        defaultSections = Sections;
        for (int i = 0; i < Sections.Length; i++)
        {
            defaultSectionPositions[i] = Sections[i].transform.position;
        }
    }

    private void MoveSections()
    {
        foreach (var section in Sections)
        {
            section.transform.Translate(Vector2.right * (movementSpeed * Time.smoothDeltaTime));
        }
    }

    public void ResetSections(EventArgs args)
    {
        counter = 0;
        Sections = defaultSections;
        for (int i = 0; i < Sections.Length; i++)
        {
            if (i == 0)
            {
                Sections[i].startPlatform.gameObject.SetActive(true);
            }

            Sections[i].transform.position = defaultSectionPositions[i];
        }
    }
}