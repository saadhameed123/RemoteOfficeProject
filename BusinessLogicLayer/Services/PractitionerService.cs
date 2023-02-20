using System.Collections.Concurrent;
using System.Drawing;
using System.Globalization;
using System.Security.Permissions;
using System.Text.Json;
using Coreplus.Sample.Api.Types;

namespace Coreplus.Sample.Api.Services;

public record PractitionerDto(long id, string name);

public class PractitionerService
{
    public async Task<IEnumerable<PractitionerDto>> GetPractitioners()
    {

        using var fileStream = File.OpenRead(@"./data/practitioners.json");
        var data = await JsonSerializer.DeserializeAsync<Practitioner[]>(fileStream);
        if (data == null)
        {
            throw new Exception("Data read error");
        }

        return data.Select(prac => new PractitionerDto(prac.id, prac.name)).ToList();
    }

    public List<Practitioner> GetPractitionersTest()
    {
        using var fileStream = File.OpenRead(@"./data/practitioners.json");
        var data = JsonSerializer.Deserialize<Practitioner[]>(fileStream);


        if (data == null)
        {
            throw new Exception("Data read error");
        }
        var s = data.Select(prac => new PractitionerMew { id = prac.id, name = prac.name, level = prac.level }).ToList();
        return data.ToList();
    }
    public List<CustomModelReportFinal> GetPractitionersReport(int PractitionerId, DateTime startDate, DateTime endDate)
    {
        using var fileStreamPractioner = File.OpenRead(@"./data/practitioners.json");
        var dataPractioner = JsonSerializer.Deserialize<Practitioner[]>(fileStreamPractioner);

        using var fileStreamAppointments = File.OpenRead(@"./data/appointments.json");
        var dataAppointments = JsonSerializer.Deserialize<Appoinments[]>(fileStreamAppointments);




        var practitionerFinal = from pract in dataPractioner
                           join appoin in dataAppointments on pract.id equals appoin.practitioner_id
                                where pract.id == PractitionerId
                           select
                           new Appoinments
                           {
                               id = appoin.id,
                               practitioner_id = appoin.practitioner_id,
                               date = appoin.date,
                               Practioner_name = pract.name,
                               client_name = appoin.client_name,
                               appointment_type = appoin.appointment_type,
                               duration = appoin.duration,
                               revenue = appoin.revenue,
                               cost = appoin.cost

                           };
        var practitioners = practitionerFinal.Select(appoin => new AppoinmentsFinal

        {
            id = appoin.id,
            PractitionerID = appoin.practitioner_id,
            date = Convert.ToDateTime(appoin.date),
            Practioner_name = appoin.Practioner_name,
            client_name = appoin.client_name,
            appointment_type = appoin.appointment_type,
            duration = appoin.duration,
            revenue = appoin.revenue,
            cost = appoin.cost

        });



        var practitionerData = from practitioner in practitioners
                               where practitioner.date >= startDate && practitioner.date <= endDate
                               group practitioner by new { practitioner.PractitionerID, practitioner.Practioner_name, practitioner.date.Month } into g
                               select new
                               {
                                   practionId = g.Key.PractitionerID,
                                     PractitionerName = g.Key.Practioner_name,
                                   Month = g.Key.Month,
                                   TotalCost = g.Sum(p => p.cost),
                                   TotalRevenue = g.Sum(p => p.revenue)
                               };

        var data = practitionerData.Select(p => new CustomModelReportFinal
        {
            practionId = p.practionId,
            PractitionerName = p.PractitionerName,
            Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(p.Month),
            TotalCost = p.TotalCost,
            TotalRevenue = p.TotalRevenue
        });
        return data.OrderBy(p=>p.Month).ToList();
    }

    public async Task<IEnumerable<PractitionerDto>> GetSupervisorPractitioners()
    {
        using var fileStream = File.OpenRead(@"./data/practitioners.json");
        var data = await JsonSerializer.DeserializeAsync<Practitioner[]>(fileStream);
        if (data == null)
        {
            throw new Exception("Data read error");
        }

        return data.Where(practitioner => (int)practitioner.level >= 2).Select(prac => new PractitionerDto(prac.id, prac.name));
    }
    public class PractitionerMew
    {
        public long id { get; set; }
        public string name { get; set; }
        public PractitionerLevel level { get; set; }
    }
    public class Appoinments
    {
        public long id { get; set; }
        public string date { get; set; }
        public string client_name { get; set; }
        public string Practioner_name { get; set; }
        public string appointment_type { get; set; }
        public int duration { get; set; }
        public float revenue { get; set; }
        public float cost { get; set; }
        public int practitioner_id { get; set; }

        //     public string PractionerName { get; set; }



    }
    public class AppoinmentsFinal
    {
        public long id { get; set; }
        public DateTime date { get; set; }
        public string client_name { get; set; }
        public string appointment_type { get; set; }
        public int duration { get; set; }
        public float revenue { get; set; }
        public float cost { get; set; }
        public int PractitionerID { get; set; }
        public string Practioner_name { get; set; }

        //     public string PractionerName { get; set; }



    }
    public class CustomModelReportFinal
    {
        public int practionId { get; set; }
        public string PractitionerName { get; set; }
        public string Month { get; set; }
        public float TotalCost { get; set; }
        public float TotalRevenue { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public List<CustomModelReportFinal> CustomModelReportFinalList { get; set; }
    }
  
}