using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelinerNet
{
    public static class Stuff
    {
        public static double ToPixcel(this TimeSpan time, TimeSpan spanDt, double spanPx) => time / spanDt * spanPx;

    }
}
