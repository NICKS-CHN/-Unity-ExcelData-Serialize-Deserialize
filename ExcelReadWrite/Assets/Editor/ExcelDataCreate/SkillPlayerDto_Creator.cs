using appDto;

namespace ExcelDataCreator
{
    public class SkillPlayerDto_Creator : BaseExcel_Creator
    {
        public SkillPlayerDto _skillPlayerDto = new SkillPlayerDto();

        public override object GetData()
        {
            return _skillPlayerDto;
        }

        public void set_id(string pId)
        {
            _skillPlayerDto.id = int.Parse(pId);
        }

        /** 名字 */
        public void set_name(string pName)
        {
            _skillPlayerDto.name = pName;
        }

        /** 描述 */
        public void set_desc(string pDesc)
        {
            _skillPlayerDto.desc = pDesc;
        }

        /** 按键 */
        public void set_button(string pButton)
        {
            _skillPlayerDto.button = pButton;
        }

    }
}
