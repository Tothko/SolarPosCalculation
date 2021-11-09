using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarPosCalculation.Interfaces
{
    public interface ISolarPositionService
    {
        DateTime SunRise { get; }
        DateTime SunSet { get;  }
        string MoonPHase { get;  }
        DateTime CalculateNoon(double longitude);
    }
}
