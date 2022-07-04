using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CSharp.UI.ColorBoard
{
    public class ColorBoard : MonoBehaviour, IPointerClickHandler, IDragHandler
    {
        //显示颜色的Texture
        Texture2D tex2d;
        //RawImage组件
        RawImage ri;
        //像素宽度高度256(默认值)
        int TexPixelLength = 256;
        int TexPixelHeight = 256;
        //公共组件
        public Slider sliderCRGB;

        public ColorHue colorHue;
        //颜色数组
        UnityEngine.Color[,] arrayColor;
        //自身的Transform
        RectTransform rt;
        //颜色聚焦点的圆圈
        public RectTransform circleRect;

        public delegate void ColorChangeDelegate(Color color);

        public event ColorChangeDelegate OnColorChanged;

        private void Awake()
        {
            ri = GetComponent<RawImage>();
            rt = GetComponent<RectTransform>();
            circleRect = transform.Find("img_cursor").GetComponent<RectTransform>();

            TexPixelLength = (int)rt.sizeDelta.x;
            TexPixelHeight = (int)rt.sizeDelta.y;
            
            //初始化颜色数组
            arrayColor = new UnityEngine.Color[TexPixelLength, TexPixelHeight];
            //创建一个固定长宽的Texture
            tex2d = new Texture2D(TexPixelLength, TexPixelHeight, TextureFormat.RGB24, true);
            //组件赋值图片
            ri.texture = tex2d;
            ri.texture.wrapMode = TextureWrapMode.Clamp;
            //初始化设置板子的颜色为红色
            SetColorPanel(UnityEngine.Color.red);
            
            sliderCRGB.onValueChanged.AddListener(OnCRGBValueChanged);
        }
        
        
        //颜色放生变化的监听
        void OnCRGBValueChanged(float value)
        {
            UnityEngine.Color endColor=colorHue.GetColorBySliderValue(value);
            SetColorPanel(endColor);

            var color = GetColorByPosition(circleRect.anchoredPosition);
            OnColorChanged?.Invoke(color);
        }

//设置板子的颜色
        public void SetColorPanel(UnityEngine.Color endColor)
        {
            UnityEngine.Color[] CalcArray = CalcArrayColor(endColor);
            //给颜色板子填入颜色，并且应用
            tex2d.SetPixels(CalcArray);
            tex2d.Apply();
        }


        //通过一个最终颜色值，计算板子上所有像素点应该的颜色，并返回一个数组
        UnityEngine.Color[] CalcArrayColor(UnityEngine.Color endColor)
        {
            //计算最终值和白色的差值在水平方向的平均值，用于计算水平每个像素点的色值
            UnityEngine.Color value = (endColor - UnityEngine.Color.white) / (TexPixelLength - 1);
            for (int i = 0; i < TexPixelLength; i++)
            {
                arrayColor[i, TexPixelHeight - 1] = UnityEngine.Color.white + value * i;
            }
            // 同理，垂直方向
            for (int i = 0; i < TexPixelLength; i++)
            {
                value = (arrayColor[i, TexPixelHeight - 1] - UnityEngine.Color.black) / (TexPixelHeight - 1);
                for (int j = 0; j < TexPixelHeight; j++)
                {
                    arrayColor[i, j] = UnityEngine.Color.black + value * j;
                }
            }
            //返回一个数组，保存了所有颜色色值
            List<UnityEngine.Color> listColor = new List<UnityEngine.Color>();
            for (int i = 0; i < TexPixelHeight; i++)
            {
                for (int j = 0; j < TexPixelLength; j++)
                {
                    listColor.Add(arrayColor[j, i]);
                }
            }

            return listColor.ToArray();
        }


        /// <summary>
        /// 获取颜色by坐标，坐标是Texture上面的二维坐标
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public UnityEngine.Color GetColorByPosition(Vector2 pos)
        {
            Texture2D tempTex2d = (Texture2D)ri.texture;
            UnityEngine.Color getColor = tempTex2d.GetPixel((int)pos.x, (int)pos.y);
            return getColor;
        }

        public Vector2 GetClampPosition(Vector2 touchPos)
        {
            Vector2 vector2 = new Vector2(touchPos.x, touchPos.y);
            vector2.x = Mathf.Clamp(vector2.x, 0.001f, rt.sizeDelta.x);
            vector2.y = Mathf.Clamp(vector2.y, 0.001f, rt.sizeDelta.y);
            return vector2;
        }
        //点击事件
        public void OnPointerClick(PointerEventData eventData)
        {
            Vector3 wordPos;
            //将UGUI的坐标转为世界坐标  
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out wordPos))
                circleRect.position = wordPos;
            circleRect.anchoredPosition = GetClampPosition(circleRect.anchoredPosition);
            var color = GetColorByPosition(circleRect.anchoredPosition);
            OnColorChanged?.Invoke(color);
        }

        //拖拽事件
        public void OnDrag(PointerEventData eventData)
        {
            Vector3 wordPos;
            //将UGUI的坐标转为世界坐标  
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out wordPos))
                circleRect.position = wordPos;
            circleRect.anchoredPosition = GetClampPosition(circleRect.anchoredPosition);

            var color = GetColorByPosition(circleRect.anchoredPosition);
            OnColorChanged?.Invoke(color);
        }
    }
}
