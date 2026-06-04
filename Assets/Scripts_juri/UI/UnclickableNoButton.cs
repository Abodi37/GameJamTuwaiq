using UnityEngine;
using UnityEngine.EventSystems;

public class UnclickableNoButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public RectTransform moveArea;
    public float padding = 80f;

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();

        if (moveArea == null && transform.parent != null)
            moveArea = transform.parent.GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MoveButton();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MoveButton();
    }

    void MoveButton()
    {
        if (rect == null || moveArea == null) return;

        float width = moveArea.rect.width * 0.5f - padding;
        float height = moveArea.rect.height * 0.5f - padding;

        rect.anchoredPosition = new Vector2(
            Random.Range(-width, width),
            Random.Range(-height, height)
        );
    }
}