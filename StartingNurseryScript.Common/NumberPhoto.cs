using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingNurseryScript.Common {
    public class NumberPhoto {
        public Point[] Points { get; set; }
        public byte[] Photo { get; set; }
        public int Number { get; set; } = (int)ErrorNumber.Init;
        public Point Center { get => Utils.GetCenter(Points); }
    }
}
