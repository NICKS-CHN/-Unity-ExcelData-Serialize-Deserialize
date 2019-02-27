using System;
using System.Collections.Generic;

namespace appDto
{
    [Serializable]
    public class AchievementDto
    {
        public int id;
        public int startCount;
        public string desc;
        public int power;
        public string name;
        public List<int> attribute;
    }
}
