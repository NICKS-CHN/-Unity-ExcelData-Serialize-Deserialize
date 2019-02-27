using System;
using System.Collections.Generic;

namespace appDto
{
    [Serializable]
    public class PetDto
    {
        public int id;
        public string name;
        public string desc;
        public List<int> skillId;
    }
}
