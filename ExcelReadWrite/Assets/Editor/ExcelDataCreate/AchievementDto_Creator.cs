using System.Collections;
using System.Collections.Generic;
using appDto;
using UnityEngine;

namespace ExcelDataCreator
{
    public class AchievementDto_Creator : BaseExcel_Creator
    {
        public AchievementDto achievement = new AchievementDto();

        public override object GetData()
        {
            return achievement;
        }

        public void set_startCount(string pStartCount)
        {
            int result = -1;
            bool b = int.TryParse(pStartCount, out result);
            if (b)
                achievement.startCount = result;
        }

        public void set_desc(string pDesc)
        {
            achievement.desc = pDesc;
        }
        public void set_power(string pPower)
        {
            int result = -1;
            bool b = int.TryParse(pPower, out result);
            if (b)
                achievement.power = result;
        }
        public void set_name(string pName)
        {
            achievement.name = pName;
        }

        public void set_attribute(string pAttribute)
        {
            List<int> tAttribute = new List<int>();
            if (string.IsNullOrEmpty(pAttribute))
            {
                achievement.attribute = tAttribute;
                return;
            }
            else
            {
                string[] attStr = pAttribute.Split(',');
                foreach (var item in attStr)
                {
                    int result = -1;
                    bool b = int.TryParse(item, out result);
                    if (b)
                        tAttribute.Add(result);
                }
            }
            achievement.attribute = tAttribute;
        }
    }

}
