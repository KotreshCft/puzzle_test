using System.Collections;
using UnityEngine;

public class FloatingImageEffect : MonoBehaviour
{
    public float fadeInDuration = 0.5f;
    public float moveUpDistance = 50f;
    public float fadeOutDuration = 0.5f;
    public float totalDuration = 2f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>(); // Ensures fade control
    }

    private void OnEnable()
    {
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        Vector3 startPos = rectTransform.anchoredPosition;
        Vector3 endPos = startPos + new Vector3(0, moveUpDistance, 0);

        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, elapsedTime / totalDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;

        // Pause for a moment
        yield return new WaitForSeconds(totalDuration - fadeInDuration - fadeOutDuration);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            rectTransform.anchoredPosition = Vector3.Lerp(endPos, endPos + new Vector3(0, 10f, 0), elapsedTime / fadeOutDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;

        // Destroy object after animation
        Destroy(gameObject);
    }
}
