using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveUIManager : MonoBehaviour
{
    [SerializeField] private List<ObjectiveUIElement> objectiveUIElements = new List<ObjectiveUIElement>();
    public GameObject objectiveUIPrefab; //assign
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