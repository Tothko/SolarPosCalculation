using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarPosCalculation.Interfaces
{
    public interface ISolarPositionService
    {
        double Whatever(double latitude, double longitude, int timezone);
    }
}
