using System.Collections;
using System.Collections.Generic;
using System.Linq;
using appDto;
using UnityEngine;

namespace ExcelDataCreator
{
    public class PetDto_Creator : BaseExcel_Creator
    {
        public PetDto petDto = new PetDto();

        public override object GetData()
        {
            return petDto;
        }

        public void set_id(string pId)
        {
            petDto.id = int.Parse(pId);
        }
        public void set_name(string pName)
        {
            petDto.name = pName;
        }
        public void set_desc(string pDesc)
        {
            petDto.desc = pDesc;
        }
        public void set_skillId(string pSkillId)
        {
            petDto.skillId = pSkillId.Split(',').ToList().ConvertAll<int>(item => int.Parse(item));
        }
    }
}
