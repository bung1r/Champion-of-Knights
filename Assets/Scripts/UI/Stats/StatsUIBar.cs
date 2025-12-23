using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatsUIBar : MonoBehaviour
{
    public GameObject fullBar;
    public GameObject currBar;
    private Vector3 targetVector = new Vector3(1f,1f,1f);
    void Start()
    {
        if (currBar == null) Debug.LogError("Please assign currBar in the inspector!");
        if (fullBar == null) {
            fullBar = gameObject;
        }
    }

    void Update()
    {
        if (Mathf.Abs(currBar.transform.localScale.x - targetVector.x) < 0.001f) return;
        currBar.transform.localScale = Vector3.Lerp(
            currBar.transform.localScale, 
            targetVector,
            Time.deltaTime * 20f);
    }

    public void UpdateBar(float min, float max)
    {
        float ratio = min / max;
        targetVector = new Vector3(ratio, 1, 1);
    }

    public void UpdateBar(float ratio)
    {
        targetVector = new Vector3(ratio, 1, 1);
    }
}
