using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Focus动画
/// </summary>
public class UIFocusHelper : MonoBehaviour
{
    public Image target;


    private Vector4 center;
    private Material _material;
    private float diameter; // 直径
    //private float current = 0f;

    //UI Rect宽度的一半
    private float offsetX = 0f;
    //UI Rect高度的一半
    private float offsetY = 0f;
    //当前遮挡宽度
    private float currentX = 0f;
    //当前遮挡高度
    private float currentY = 0f;

    Vector3[] corners = new Vector3[4];
    Canvas _canvas;

    public Canvas canvas
    {
        get
        {
            if (_canvas == null)
            {
                _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            }
            return _canvas;
        }
    }

    public Camera UICamera
    {
        get
        {
            return canvas.GetComponent<Camera>();
        }
    }
    [SerializeField]
    Image _coverImage;

    private void Start()
    {
        FocusOn(target.GetComponent<RectTransform>());
    }

    public void FocusOn(RectTransform targetRectTransform)
    {
        targetRectTransform.GetWorldCorners(corners);
        //diameter = Vector2.Distance(WordToCanvasPos(canvas, corners[0]), WordToCanvasPos(canvas, corners[2])) / 2f;
        offsetX = Vector2.Distance(WordToCanvasPos(canvas, corners[0]), WordToCanvasPos(canvas, corners[3])) / 2f;
        offsetY = Vector2.Distance(WordToCanvasPos(canvas, corners[0]), WordToCanvasPos(canvas, corners[1])) / 2f;
        float x = corners[0].x + ((corners[3].x - corners[0].x) / 2f);
        float y = corners[0].y + ((corners[1].y - corners[0].y) / 2f);

        Vector3 center = new Vector3(x, y, 0f);
        Vector2 position = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, center, UICamera, out position);

        center = new Vector4(position.x, position.y, 0f, 0f);

        _material = _coverImage.material;

        _material.SetVector("_Center", center);

        (canvas.transform as RectTransform).GetWorldCorners(corners);

        for (int i = 0; i < corners.Length; i++)
        {
            if(i % 2 == 0)
            {
                currentX = Mathf.Max(Vector3.Distance(WordToCanvasPos(canvas, corners[i]), center), currentX);
            }
            else
            {
                currentY = Mathf.Max(Vector3.Distance(WordToCanvasPos(canvas, corners[i]), center), currentY);
            }
        }

        _material.SetFloat("_SliderX", currentX);
        _material.SetFloat("_SliderY", currentY);

    }


    float yVelocity = 0f;
    float xVelocity = 0f;
    void Update()
    {
        //float value = Mathf.SmoothDamp(current, diameter, ref yVelocity, 0.3f);
        //if (!Mathf.Approximately(value, current))
        //{
        //    current = value;
        //    _material.SetFloat("_Slider", current);
        //}
        float valueX = Mathf.SmoothDamp(currentX, offsetX, ref xVelocity, 0.3f);
        float valueY = Mathf.SmoothDamp(currentY, offsetY, ref yVelocity, 0.3f);
        if (!Mathf.Approximately(valueX, currentX))
        {
            currentX = valueX;
            _material.SetFloat("_SliderX", currentX);
        }
        if (!Mathf.Approximately(valueY, currentY))
        {
            currentY = valueY;
            _material.SetFloat("_SliderY", currentY);
        }

    }

    static Vector2 WordToCanvasPos(Canvas canvas, Vector3 world)
    {
        Vector2 position = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, world, canvas.GetComponent<Camera>(), out position);
        return position;
    }
}

