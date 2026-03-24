using System;
using UnityEngine;

namespace ClawbearGames
{
    public class Utilities
    {



        /// <summary>
        /// Convert the given float number to meters format.
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static string FloatToMeters(float distance)
        {
            if (distance < 1000f)
            {
                return Mathf.RoundToInt(distance).ToString() + " M";
            }
            else
            {
                float distanceTemp = distance / 1000f;
                return string.Format("{0:0.00}", distanceTemp) + " KM";
            }
        }


        /// <summary>
        /// Covert the given seconds to time format.
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string SecondsToTimeFormat(double seconds)
        {
            int hours = (int)seconds / 3600;
            int mins = ((int)seconds % 3600) / 60;
            seconds = Math.Round(seconds % 60, 0);
            return hours + ":" + mins + ":" + seconds;
        }

        /// <summary>
        /// Covert the given seconds to minutes format.
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string SecondsToMinutesFormat(double seconds)
        {
            int mins = ((int)seconds % 3600) / 60;
            seconds = Math.Round(seconds % 60, 0);
            return mins + ":" + seconds;
        }


        /// <summary>
        /// Convert color to hex
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ColorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        /// <summary>
        /// Convert hex to color
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color HexToColor(string hex)
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
    }
}
