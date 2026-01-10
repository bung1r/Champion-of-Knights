using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GlitchEffectController : MonoBehaviour
{
    public Image glitchImage;
    public float maxAlpha = 0.25f;
    public float flickerSpeed = 0.05f;

    Coroutine glitchRoutine;

    public void SetCorruptionLevel(float corruption01)
    {
        if (corruption01 <= 0)
        {
            StopGlitch();
            return;
        }

        if (glitchRoutine == null)
            glitchRoutine = StartCoroutine(GlitchFlicker(corruption01));
    }

    void StopGlitch()
    {
        if (glitchRoutine != null)
        {
            StopCoroutine(glitchRoutine);
            glitchRoutine = null;
        }

        SetAlpha(0);
    }

    IEnumerator GlitchFlicker(float intensity)
    {
        while (true)
        {

            float scaled = Mathf.Pow(intensity, 1.5f);

            float alpha = Random.Range(0, maxAlpha * scaled);
            SetAlpha(alpha);

            glitchImage.rectTransform.anchoredPosition = Random.insideUnitCircle * (5f * intensity);

            float speed = Random.Range(flickerSpeed / 2f, flickerSpeed * 1.5f);
            speed *= 2f - intensity;

            yield return new WaitForSeconds(speed);
        }
    }

    void SetAlpha(float a)
    {
        Color c = glitchImage.color;
        c.a = a;
        glitchImage.color = c;
    }
}