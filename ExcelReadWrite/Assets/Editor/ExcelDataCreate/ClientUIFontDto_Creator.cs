using appDto;

namespace ExcelDataCreator
{
    public class ClientUIFontDto_Creator : BaseExcel_Creator
    {
        public ClientUIFontDto _clientUIFontDto = new ClientUIFontDto();

        public override object GetData()
        {
            return _clientUIFontDto;
        }

        public void set_id(string pId)
        {
            _clientUIFontDto.id = int.Parse(pId);
        }

        /** 字体简述 */
        public void set_shortDesc(string pShortDesc)
        {

        }

        /** 字体类型 */
        public void set_fontName(string pFontName)
        {

        }

        /** 字体颜色 */
        public void set_colorTint(string pColorTint)
        {

        }

        /** 字体大小 */
        public void set_fontSize(string pFontSize)
        {

        }

        /** 字体样式 */
        public void set_fontStyle(string pFontStyle)
        {

        }

        /** 是否渐变 */
        public void set_gradient(string pGradient)
        {

        }

        /** 顶部颜色 */
        public void set_gradientTop(string pGradientTop)
        {

        }

        /** 底部颜色 */
        public void set_gradientBottom(string pGradientBottom)
        {

        }

        /** 阴影效果 */
        public void set_effect(string pEffect)
        {

        }

        /** 阴影/描边效果 */
        public void set_effectColor(string pEffectColor)
        {

        }

        /** 阴影/描边偏移X */
        public void set_effectX(string pEffectX)
        {

        }

        /** 阴影/描边偏移Y */
        public void set_effectY(string pEffectY)
        {

        }

        /** 字间距 */
        public void set_spacingX(string pSpacingX)
        {

        }

        /** 行间距 */
        public void set_spacingY(string pSpacingY)
        {

        }

    }
}
