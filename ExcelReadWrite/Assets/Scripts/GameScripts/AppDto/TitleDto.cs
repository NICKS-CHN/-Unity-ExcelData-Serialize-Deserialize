using System;
using System.Collections.Generic;
namespace appDto
{
    [Serializable]
    public class TitleDto
    {
        /** id */
        public int id;

        /** 名字 */
        public string name;

        /** 描述 */
        public string desc;

        /** 技能列表 */
        public List<int> skillId;

    }
}
