using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace naxokit.Helpers
{
    public class Converters
    {
        //Handling PlayerPrefs
        public static int boolToInt(bool value)
        {
            if (value)
                return 1;
            else
                return 0;
        }
        public static bool intToBool(int value)
        {
            if (value != 0)
                return true;
            else
                return false;
        }
        //End
    }
}
