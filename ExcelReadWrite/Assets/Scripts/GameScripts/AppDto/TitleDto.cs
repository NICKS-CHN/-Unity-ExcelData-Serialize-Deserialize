using System;
using System.Collections.Generic;
namespace appDto

{
    [Serializable]
    public class TitleDto
    {
        public int id;
        public string name;
        public List<int> skillId;
    }
}
