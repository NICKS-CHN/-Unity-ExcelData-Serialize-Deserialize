using appDto;

namespace ExcelDataCreator
{
    public class SkillDto_Creator : BaseExcel_Creator
    {
        public SkillDto _skillDto = new SkillDto();

        public override object GetData()
        {
            return _skillDto;
        }

        public void set_id(string pId)
        {
            _skillDto.id = int.Parse(pId);
        }

        /** 名字 */
        public void set_name(string pName)
        {
            _skillDto.name = pName;
        }

        /** 描述 */
        public void set_desc(string pDesc)
        {
            _skillDto.desc = pDesc;
        }

    }
}
