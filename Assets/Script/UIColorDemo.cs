using System.Collections;
using System.Collections.Generic;
using CSharp.UI.ColorBoard;
using UnityEngine;
using UnityEngine.UI;

public class UIColorDemo : MonoBehaviour
{
    private Image _imageShow;

    private ColorBoard _colorBoard;

    private Slider _alphaSlider;
    // Start is called before the first frame update
    void Start()
    {
        _imageShow = transform.Find("img_show").GetComponent<Image>();

        _colorBoard = transform.Find("img_bg/rImg_colorBoard").GetComponent<ColorBoard>();

        _alphaSlider = transform.Find("img_bg/slider_alpha").GetComponent<Slider>();

        _colorBoard.OnColorChanged += ImageShowColor;

        _alphaSlider.onValueChanged.AddListener(ImageShowAlpha);
    }

    void ImageShowColor(Color color)
    {
        var _rawColor = _imageShow.color;
        _imageShow.color = new Color(color.r, color.g, color.b, _rawColor.a);
    }

    void ImageShowAlpha(float value)
    {
        var _rawColor = _imageShow.color;
        _imageShow.color = new Color(_rawColor.r, _rawColor.g, _rawColor.b, value);
    }
}
