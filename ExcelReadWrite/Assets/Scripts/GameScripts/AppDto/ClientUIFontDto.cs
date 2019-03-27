using System;
using System.Collections.Generic;
namespace appDto
{
    [Serializable]
    public class ClientUIFontDto
    {
        /** 编号 */
        public int id;

        /** 字体简述 */
        public string shortDesc;

        /** 字体类型 */
        public string fontName;

        /** 字体颜色 */
        public string colorTint;

        /** 字体大小 */
        public int fontSize;

        /** 字体样式 */
        public int fontStyle;

        /** 是否渐变 */
        public bool gradient;

        /** 顶部颜色 */
        public string gradientTop;

        /** 底部颜色 */
        public string gradientBottom;

        /** 阴影效果 */
        public int effect;

        /** 阴影/描边效果 */
        public string effectColor;

        /** 阴影/描边偏移X */
        public int effectX;

        /** 阴影/描边偏移Y */
        public int effectY;

        /** 字间距 */
        public int spacingX;

        /** 行间距 */
        public int spacingY;

    }
}
