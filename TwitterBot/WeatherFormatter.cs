using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ScottPlot;
using TootingMad.DataSources.LaCrosse;
using Tweetinvi;
using Tweetinvi.Parameters;

namespace TwitterBot
{
    public class StringFormatter {
        public static string WithEmoji(List<Tuple<Sensor, object>> res) {
            
            return $"🌡 {((decimal)res[2].Item2).ToString("F1")}°C 💦 {((decimal)res[3].Item2).ToString("F0")}% 🌬 {((decimal)res[1].Item2).ToString("F1")} kts {res[0].Item2} ㍱ {((decimal)res[4].Item2).ToString("F1")}  {res[5].Item2.ToString().ToLower()}, 🌧{elapsedRain:0.#}mm";
        }

        public static string InEnglish(List<Tuple<Sensor, object>> res) {
            return $"It's currently {((decimal)res[2].Item2).ToString("F1")}°C with {((decimal)res[3].Item2).ToString("F0")}% humidity.\nThe wind is {((decimal)res[1].Item2).ToString("F1")} kts from {res[0].Item2}\nAir pressure is {((decimal)res[4].Item2).ToString("F1")} hPa and {res[5].Item2.ToString().ToLower()}, {elapsedRain:0.#}mm rain";
            
        }
    }


}