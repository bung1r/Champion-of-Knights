using System.Collections;
using UnityEngine;

public class SkillInfoHoverThingy : MonoBehaviour
{
    public SkilltreeManager skilltreeManager;
    public SkilltreeNode prevNode;
    public RectTransform rectTransform;

    public IEnumerator BeginExpansion(SkilltreeNode node)
    {
        Debug.Log("Expansion has begunm");
        rectTransform.anchoredPosition = node.transform.localPosition;
        rectTransform.localScale = new Vector3(0f,0f,1f);
        while (rectTransform.localScale.x < 0.99f)
        {
            yield return null;
            float temp = Mathf.Lerp(rectTransform.localScale.x, 1f, 0.3f);
            rectTransform.localScale += new Vector3(temp,temp,0f);
        }
        rectTransform.localScale = new Vector3(1f,1f,1f);
    }
    public IEnumerator ReverseExpansion()
    {
        while (rectTransform.localScale.x > 0.01f)
        {
            yield return null;
            float temp = Mathf.Lerp(rectTransform.localScale.x, 0f, 0.3f);
            rectTransform.localScale += new Vector3(temp,temp,0f);
        }
        rectTransform.localScale = new Vector3(0f,0f,1f);
    }
    public Coroutine ExpansionCoroutine;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        skilltreeManager = GetComponentInParent<SkilltreeManager>();
    }
    void Start()
    {
        rectTransform.localScale = new Vector3(0f,0f,1f);
    }
    public void HoverNodeChange()
    {
        if (ExpansionCoroutine != null) {
            StopCoroutine(ExpansionCoroutine);
            ExpansionCoroutine = null;
        }

        SkilltreeNode node = skilltreeManager.hoverNode;
        if (node == null)
        {
            ExpansionCoroutine = StartCoroutine(ReverseExpansion());
        } else
        {
            ExpansionCoroutine = StartCoroutine(BeginExpansion(node));
        }


        prevNode = node;
   }

}