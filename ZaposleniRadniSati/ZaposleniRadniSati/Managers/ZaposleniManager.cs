using Newtonsoft.Json;
using System.Drawing;
using ZaposleniRadniSati.Models;
using static System.Net.Mime.MediaTypeNames;
using Font = System.Drawing.Font;

namespace ZaposleniRadniSati.Managers
{
    public class ZaposleniManager
    {
        private readonly string apiUrl = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";

        public class ZaposleniRadniSati
        {
            public string Ime { get; set; }
            public double RadniSati { get; set; }
        }
        public async Task<List<ZaposleniRadniSati>> GetZaposleniAsync()
        {
            using (HttpClient klijent = new HttpClient())
            {
                HttpResponseMessage response = await klijent.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var vreme = JsonConvert.DeserializeObject<List<Zaposleni>>(jsonResponse);

                var employeeTotalHours = vreme
                    .GroupBy(e => e.EmployeeName)
                    .Select(group => new ZaposleniRadniSati
                    {
                        Ime = group.Key,
                        RadniSati = group.Sum(e => e.TotalHoursWorked)
                    })
                    .OrderByDescending(e => e.RadniSati)
                    .ToList();

                return employeeTotalHours;
            }
        }
        public void GeneratePieChart(Dictionary<string, double> employeeTotalHours, string outputPath)
        {
            double totalHours = employeeTotalHours.Values.Sum();

            const int width = 900;
            const int height = 600;

            using (var bitmap = new Bitmap(width, height))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);

                Rectangle rect = new Rectangle(50, 100, 500, 500);

                Color[] colors = new Color[]
                {
                    Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Purple,
                    Color.Cyan, Color.Magenta, Color.YellowGreen, Color.Brown,
                    Color.Pink, Color.Gray, Color.Olive, Color.Maroon,
                    Color.Teal, Color.Lime
                };

                float startAngle = 0f;
                int colorIndex = 0;

                foreach (var entry in employeeTotalHours)
                {
                    float sweepAngle = (float)(entry.Value / totalHours * 360);

                    using (var brush = new SolidBrush(colors[colorIndex % colors.Length]))
                    {
                        graphics.FillPie(brush, rect, startAngle, sweepAngle);
                    }

                    float midAngle = startAngle + sweepAngle / 2;
                    float labelX = (float)(rect.X + rect.Width / 2 + Math.Cos(midAngle * Math.PI / 180) * rect.Width / 3);
                    float labelY = (float)(rect.Y + rect.Height / 2 + Math.Sin(midAngle * Math.PI / 180) * rect.Height / 3);

                    string percentage = $"{entry.Value / totalHours:P0}";

                    using (var font = new Font("Arial", 12, FontStyle.Bold))
                    using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        graphics.DrawString(percentage, font, Brushes.Black, new PointF(labelX, labelY), format);
                    }

                    startAngle += sweepAngle;
                    colorIndex++;
                }

                int legendX = 600;
                int legendY = 100;

                colorIndex = 0;
                foreach (var entry in employeeTotalHours)
                {
                    using (var brush = new SolidBrush(colors[colorIndex % colors.Length]))
                    {
                        graphics.FillRectangle(brush, legendX, legendY, 20, 20);
                    }

                    graphics.DrawString(entry.Key, new Font("Arial", 12), Brushes.Black, new PointF(legendX + 30, legendY));

                    legendY += 30;
                    colorIndex++;
                }

                bitmap.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
    }
}
