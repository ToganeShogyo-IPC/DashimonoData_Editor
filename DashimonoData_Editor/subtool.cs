using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashimonoData_Editor
{
    public static class subtools
    {
        public static int convertsuuzi(string mozi)
        {
            if (mozi == null) return 0;
            int result;
            try 
            {
                int.TryParse(mozi,out result);
            }
            catch 
            {
                result = -1;
            }
            return result;
        }
    }
}
