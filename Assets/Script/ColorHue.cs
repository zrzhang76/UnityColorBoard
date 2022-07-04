using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorHue : MonoBehaviour
{
    //绘制颜色的Texture
    Texture2D tex2d;
    //图片展示组件
    RawImage ri;
    //长宽
    int TexPixelWdith = 952;
    int TexPixelHeight = 16;
    //颜色的数组
    UnityEngine.Color[,] arrayColor;

    private void Awake()
    {
        ri = gameObject.GetComponent<RawImage>();
        
        //初始化颜色和Texture
        arrayColor = new UnityEngine.Color[TexPixelWdith, TexPixelHeight];
        tex2d = new Texture2D(TexPixelWdith, TexPixelHeight, TextureFormat.RGB24,true);
            
        //计算颜色
        UnityEngine.Color[] calcArray = CalcArrayColor();
        //展示出来
        tex2d.SetPixels(calcArray);
        tex2d.Apply();

        ri.texture = tex2d;
        ri.texture.wrapMode = TextureWrapMode.Clamp;
    }
    
    
    //计算色相条上面需要展示的颜色数组
    UnityEngine.Color[] CalcArrayColor()
    {
        //计算水平像素的等分增量
        int addValue = (TexPixelWdith - 1) / 3;
        //
        for (int i = 0; i < TexPixelHeight; i++)
        {
            arrayColor[0, i] = UnityEngine.Color.red;
            arrayColor[addValue, i] = UnityEngine.Color.green;
            arrayColor[addValue+addValue, i] = UnityEngine.Color.blue;
            arrayColor[TexPixelHeight - 1, i] = UnityEngine.Color.red;
        }
        UnityEngine.Color value = (UnityEngine.Color.green - UnityEngine.Color.red)/addValue;
        for (int i = 0; i < TexPixelHeight; i++)
        {
            for (int j = 0; j < addValue; j++)
            {
                arrayColor[j, i] = UnityEngine.Color.red + value * j;
            }
        }

        value = (UnityEngine.Color.blue - UnityEngine.Color.green)/ addValue;
        for (int i = 0; i < TexPixelHeight; i++)
        {
            for (int j = addValue; j < addValue*2; j++)
            {
                arrayColor[j, i] = UnityEngine.Color.green + value * (j-addValue);
            }
        }

        value = (UnityEngine.Color.red - UnityEngine.Color.blue) / ((TexPixelWdith - 1)-addValue-addValue);
        for (int i = 0; i < TexPixelHeight; i++)
        {
            for (int j = addValue*2; j < TexPixelWdith - 1; j++)
            {
                arrayColor[j, i] = UnityEngine.Color.blue + value * (j- addValue * 2);
            }
        }

        List<UnityEngine.Color> listColor = new List<UnityEngine.Color>();
        for (int i = 0; i < TexPixelHeight; i++)
        {
            for (int j = 0; j < TexPixelWdith; j++)
            {
                listColor.Add(arrayColor[j, i]);
            }
        }

        return listColor.ToArray();
    }
    
    /// <summary>
    /// 获取颜色 根据高度
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public Color GetColorBySliderValue(float value)
    {
        float clampValue = Mathf.Clamp(value, 0.001f, 0.999f);
        Color getColor=tex2d.GetPixel((int)((TexPixelWdith-1)*clampValue),0);
        return getColor;
    }
    
}
