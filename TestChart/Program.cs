using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using ScottPlot;

namespace TestChart
{
    class Program
    {
        static void Main(string[] args)
        {
            var customCulture = new System.Globalization.CultureInfo("en-NZ");
            customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            int width = 800;
            int height = 600;
            var plot = new Plot(width, height);

            DateTime[] myDates = new DateTime[60];
            var startdate = DateTime.Now;
            for (int i = 0; i < myDates.Length; i++)
            {
                myDates[i] = startdate.AddMinutes(i);
            }

            double[] dataX = myDates.Select(x => x.ToOADate()).ToArray();
            //plot.PlotScatter(dataX, dataY);

            plot.XAxis.DateTimeFormat(true);

            plot.YAxis.Label("°C");
            plot.YAxis.Color(Color.Red);

            plot.SetAxisLimitsY(0, 50);
            

            //double[] dataX = DataGen.Consecutive(60);
            double[] dataY = DataGen.Sin(60, 2, 25, 2);
            //plot.PlotScatter(dataX, dataY);

            var scatter = plot.AddScatter(dataX, dataY);

            scatter.LineWidth = 2;
            scatter.Color = Color.Red;

            var baroScatter = plot.AddScatter(dataX, DataGen.Sin(60, 6, 1013, 4));

            var baroLimits = baroScatter.GetAxisLimits();
            

            var yAxis3 = plot.AddAxis(ScottPlot.Renderable.Edge.Left, axisIndex: 2);
            baroScatter.YAxisIndex = 2;
            yAxis3.Label("hPa");
            yAxis3.Color(baroScatter.Color);

            
            
            plot.SaveFig("out.png");
            var bitmap = plot.Render();
            var png = ImageToPngByte(bitmap);


            Console.WriteLine($"PNG byte size is {png.Length}");
        }

        public static byte[] ImageToPngByte(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}
