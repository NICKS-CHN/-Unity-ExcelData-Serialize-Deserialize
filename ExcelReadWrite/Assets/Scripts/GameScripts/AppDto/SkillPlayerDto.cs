using System;
using System.Collections.Generic;
namespace appDto
{
    [Serializable]
    public class SkillPlayerDto : SkillDto
    {
        /** 编号 */
        public int id;

        /** 名字 */
        public string name;

        /** 描述 */
        public string desc;

        /** 按键 */
        public string button;

    }
}
