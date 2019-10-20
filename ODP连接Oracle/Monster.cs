using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP连接Oracle
{
    class Monster
    {
        public static string name1 = "Mno";
        public static string name2 = "Mname";
        public static string name3 = "Mrace";
        public static string name4 = "Mlevel";
        public static string name5 = "Mblood";
        public static string name6 = "Mattack";
        public static string name7 = "Mdefense";
        public static string name8 = "Mgrade";

        private string mno;
        private string mname;
        private string mrace;
        private int mlevel;
        private int Mblood;
        private int mattack;
        private int mdefense;
        private int mgrade;

        public static int count;

        public string Mno { get => mno; set => mno = value; }
        public string Mname { get => mname; set => mname = value; }
        public string Mrace { get => mrace; set => mrace = value; }
        public int Mlevel { get => mlevel; set => mlevel = value; }
        public int Mattack { get => mattack; set => mattack = value; }
        public int Mdefense { get => mdefense; set => mdefense = value; }
        public int Mgrade { get => mgrade; set => mgrade = value; }
        public int Mblood1 { get => Mblood; set => Mblood = value; }
    }



}
