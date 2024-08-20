using Newtonsoft.Json;
using ZaposleniRadniSati.Models;

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
    }
}
