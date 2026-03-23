using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveUIManager : MonoBehaviour
{
    [SerializeField] private List<ObjectiveUIElement> objectiveUIElements = new List<ObjectiveUIElement>();
    public GameObject objectiveUIPrefab; //assign
    public RectTransform parentRect; //assign?
    public Vector3 properPosition;
    void Awake()
    {
        parentRect = transform.parent.GetComponent<RectTransform>();
        properPosition = parentRect.anchoredPosition;   
    }
    public Coroutine MovementCoroutine;
    private IEnumerator BringToMiddle()
    {
        while (Vector3.Distance(Vector3.zero, parentRect.anchoredPosition) > 1f)
        {
            yield return null;
            parentRect.anchoredPosition = Vector3.Lerp(
                parentRect.anchoredPosition, Vector3.zero, 0.15f
            );
        }
    }
    private IEnumerator BringToProperPosition()
    {
        while (Vector3.Distance(properPosition, parentRect.anchoredPosition) > 1f)
        {
            yield return null;
            parentRect.anchoredPosition = Vector3.Lerp(
                parentRect.anchoredPosition, properPosition, 0.1f
            );
        }
    }
    public void BringToMiddleInstant()
    {
        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
            MovementCoroutine = null;
        }
        Debug.Log(parentRect.anchoredPosition);
        parentRect.anchoredPosition = Vector3.zero;
        Debug.Log("taking it to the middle!!");

    }
    public void TakeToProperPosition()
    {
        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
            MovementCoroutine = null;
        }

        StartCoroutine(BringToProperPosition());
    }
    public void BringToMiddleRegular()
    {
        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
            MovementCoroutine = null;
        }

        StartCoroutine(BringToMiddle());
    }
    public void AddEntry(ObjectiveScaling objScaling, Objective objective) 
    {
        if (objectiveUIElements.Count >= 6) return; //max 6 objectives displayed

        GameObject obj = Instantiate(objectiveUIPrefab, transform);
        ObjectiveUIElement newElement = obj.GetComponent<ObjectiveUIElement>();
        newElement.foundationString = objScaling.objectiveDescription;
        newElement.objective = objective;
        objectiveUIElements.Add(newElement);
    }

    public void ClearAllEntries()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        objectiveUIElements.Clear();
    }


}