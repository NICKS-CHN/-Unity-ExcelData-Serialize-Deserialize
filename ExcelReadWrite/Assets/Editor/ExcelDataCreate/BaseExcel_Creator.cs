using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExcelDataCreator
{
    public class BaseExcel_Creator : IBaseExcel_Creator
    {

        public virtual object GetData()
        {
            return null;
        }
    }
}
