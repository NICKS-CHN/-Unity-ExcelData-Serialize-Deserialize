using appDto;

namespace ExcelDataCreator
{
    public class TitleDto_Creator : BaseExcel_Creator
    {
        public TitleDto _titleDto = new TitleDto();

        public override object GetData()
        {
            return _titleDto;
        }

        public void set_id(string pId)
        {
            _titleDto.id = int.Parse(pId);
        }

        public void set_name(string pName)
        {

        }

        public void set_skillId(string pSkillId)
        {

        }

    }
}
