using CoordinateSharp;
using SolarPosCalculation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarPosCalculation.Services
{
    public class SolarPositionService : ISolarPositionService

    {
        private DateTime _sunRise;
        private DateTime _sunSet;
        private string _moonPhase;
        double _fractionalYear;
        double _eqTime;
        double _solDeclineAngle;
        double _trueSolTimeOffset;
        double _trueSolarTime;
        double _solarHourAngle;
        double _solarZenithAngle;
        double _solarAzimuth;
        double _newSolarHourAngle;
        readonly int _leapYear = 366;
        readonly int _year = 365;

        /// <summary>
        ///  Calculation of FractionalYear whatever that is.
        /// </summary>
        /// <returns>double representing radians</returns>
        private double CalculateFractionalYear()
        {
            int currentYear;
            if (DateTime.IsLeapYear(DateTime.Now.Year)) currentYear = _leapYear;
            else currentYear = _year;


            return ((2 * Math.PI) / currentYear) * (DateTime.Now.DayOfYear - 1 + (DateTime.Now.Hour - 12) / 24);
        }

        private double CalculateEquationOfTime(double fractionalYear)
        {
            return 229.18 * (0.000075 + 0.001868 * Math.Cos(fractionalYear) - 0.032077 * Math.Sin(fractionalYear) - 0.014615 * Math.Cos(2 * fractionalYear) - 0.040849 * Math.Sin(2 * fractionalYear));

        }
        private double CalculateSolarDeclinationAngle(double fractionalYear)
        {
            return 0.006918 - 0.399912 * Math.Cos(fractionalYear) + 0.070257 * Math.Sin(fractionalYear) - 0.006758 * Math.Cos(2 * fractionalYear) + 0.000907 * Math.Sin(2 * fractionalYear) - 0.002697 * Math.Cos(3 * fractionalYear) + 0.00148 * Math.Sin(3 * fractionalYear);

        }

        /// <summary>
        /// Time offset in minutes
        /// </summary>
        /// <param name="eqTime">Equation of time in minutes</param>
        /// <param name="longitude"> Your current longitude positive to the east of the Prime Meridian</param>
        /// <param name="timezone"> For example (U.S. Mountain Standard Time = –7 hours </param>
        /// <returns>Time offset in minutes</returns>
        private double CalculateTrueSolarTimeTimeOffset(double eqTime, double longitude, int timezone)
        {
            return eqTime + 4 * longitude - 60 * timezone;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeOffset"> Value taken from CalculateTrueSolarTimeTimeOffset method </param>
        /// <returns>True solar time</returns>
        private double CalculateTrueSolarTime(double timeOffset)
        {
            return (DateTime.Now.Hour * 60) + DateTime.Now.Minute + (DateTime.Now.Second / 60) + timeOffset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trueSolarTime">Value taken from CalculateTrueSolarTime method</param>
        /// <returns></returns>
        private double CalculateSolarHourAngle(double trueSolarTime)
        {
            return trueSolarTime / 4 - 180;
        }
        private double CalculateSolarZenithAngle(double hourAngle, double latitude, double solarDeclinationAngle)
        {
            return Math.Sin(latitude) * Math.Sin(180 / Math.PI * solarDeclinationAngle) +  Math.Cos(latitude) * Math.Cos(180 / Math.PI * solarDeclinationAngle) * Math.Cos(hourAngle);
        }
        /// <summary>
        /// Calculates whatever sun AZEROTH
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="solarZenithAngle"></param>
        /// <param name="solarDeclinationAngle"></param>
        /// <returns>At this point random number</returns>
        private double CalculateSolarAzimuth(double latitude, double solarZenithAngle, double solarDeclinationAngle)
        {
            return (Math.Sin(latitude) * solarZenithAngle - Math.Sin(180 / Math.PI * solarDeclinationAngle)) / Math.Cos(latitude) * Math.Sin(180/Math.PI*solarZenithAngle);
        }


        public double Whatever(double latitude, double longitude, int timezone)
        {
            _fractionalYear = CalculateFractionalYear();
            _eqTime = CalculateEquationOfTime(_fractionalYear);
            _solDeclineAngle = CalculateSolarDeclinationAngle(_fractionalYear);
            _trueSolTimeOffset = CalculateTrueSolarTimeTimeOffset(_eqTime, longitude, timezone);
            _trueSolarTime = CalculateTrueSolarTime(_trueSolTimeOffset);
            _solarHourAngle = CalculateSolarHourAngle(_trueSolarTime);
            _solarZenithAngle = CalculateSolarZenithAngle(_solarHourAngle, latitude, _solDeclineAngle);
            _solarAzimuth = CalculateSolarAzimuth(latitude, _solarZenithAngle, _solDeclineAngle);
            _newSolarHourAngle = CalculateNewSolarHourAngle(latitude);
            return _solarAzimuth;
        }
        private double CalculateNewSolarHourAngle(double latitude)
        {
            return Math.Acos((Math.Cos(90.833) / (Math.Cos(latitude) * Math.Cos(180 / Math.PI * _solDeclineAngle))) - (Math.Tan(latitude) * Math.Tan(180 / Math.PI * _solDeclineAngle)));
        }

        
        public DateTime CalculateNoon(double longitude)
        {

            /*TimeSpan ts = TimeSpan.FromMinutes(720 - 4 * (longitude + _newSolarHourAngle) - _eqTime);
            DateTime dt = DateTime.Now.Date;
            return dt = dt.AddMinutes(ts.TotalMinutes);*/
            TimeSpan ts = TimeSpan.FromMinutes(720 - 4 * longitude  - _eqTime);
            DateTime dt = DateTime.Now.Date;
            return dt = dt.AddMinutes(ts.TotalMinutes);
        }
        

        public DateTime SunRise
        {
            get { return _sunRise; }
           
        }
        public DateTime SunSet
        {
            get { return _sunSet; }

        }
        public string MoonPHase
        {
            get { return _moonPhase; }

        }


        /// <summary>
        /// Used to fill our variables with information for current day day
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public void CalculateInformation(double latitude, double longitude, bool isDaylightSavingTime)
        {
            
            
            Celestial cel = Celestial.CalculateCelestialTimes(latitude, longitude, DateTime.Now);
            _sunRise = cel.SunRise.Value;
            _sunSet = cel.SunSet.Value;
            if (!isDaylightSavingTime)
            {
                _sunRise =_sunRise.AddHours(1);
                _sunSet = _sunSet.AddHours(1);
            }
            _moonPhase = cel.MoonIllum.PhaseName;

        }
        

    }
}
