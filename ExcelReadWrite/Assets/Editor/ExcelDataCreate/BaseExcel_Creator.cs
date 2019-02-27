using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExcelDataCreator
{
    public class BaseExcel_Creator : IBaseExcel_Creator
    {
        public int id;

        public  void set_id(string pId)
        {

            id = int.Parse(pId);
        }

        public int Get_Id()
        {
            return id;
        }

        public virtual object GetData()
        {
            return null;
        }
    }
}
