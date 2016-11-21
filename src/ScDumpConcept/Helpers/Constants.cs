using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScDumpConcept
{
    public static class Constants
    {
        //Purpose, change return value to correct SQL type
        public static string TYPE_STRING { get { return typeof(string).ToString(); } }
        public static string TYPE_INT { get { return typeof(int).ToString(); } }
        public static string TYPE_DOUBLE { get { return typeof(double).ToString(); } }
        public static string TYPE_FLOAT { get { return typeof(float).ToString(); } }
        public static string TYPE_OBJECT { get { return typeof(object).ToString(); } }
        public static string TYPE_CLASS_REFERENCE { get { return "REFERENCE"; } }
    }
}
