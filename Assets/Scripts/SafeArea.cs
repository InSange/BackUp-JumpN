using UnityEngine;

// 폰 화면에서 위, 아래 공백 크기를 적용해서 화면에 맞는 UI 조정 cs
public class SafeArea : MonoBehaviour
{
    RectTransform _rectTransform;
    Rect _safeArea;
    Vector2 _minAnchor;
    Vector2 _maxAnchor;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _safeArea = Screen.safeArea;
        _minAnchor = _safeArea.position;
        _maxAnchor = _minAnchor + _safeArea.size;

        _minAnchor.x /= Screen.width;
        _minAnchor.y /= Screen.height;
        _maxAnchor.x /= Screen.width;
        _maxAnchor.y /= Screen.height;

        _rectTransform.anchorMin = _minAnchor;
        _rectTransform.anchorMax = _maxAnchor;
    }
}