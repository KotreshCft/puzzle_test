using UnityEngine;
using TMPro;
using System.Collections;
public class FloatingText : MonoBehaviour
{
    public float moveUpDistance = 50f; // Distance the text moves up
    public float duration = 1f; // Duration of the animation
    public float fadeOutTime = 0.5f; // Time to fade out at the end

    public TMP_Text textMesh;
    public RectTransform rectTransform;

    public void Animate(string text)
    {
        textMesh.text = text;
        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        Vector3 startPosition = rectTransform.position;
        Vector3 endPosition = startPosition + new Vector3(0, moveUpDistance, 0);

        float elapsedTime = 0;

        // Animate the popup and movement
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Lerp position upwards
            rectTransform.position = Vector3.Lerp(startPosition, endPosition, t);

            // Fade out at the end of the animation
            if (t > (1 - fadeOutTime / duration))
            {
                float fadeT = (t - (1 - fadeOutTime / duration)) / (fadeOutTime / duration);
                textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 1 - fadeT);
            }

            yield return null;
        }

        Destroy(gameObject); // Clean up after animation
    }
}
