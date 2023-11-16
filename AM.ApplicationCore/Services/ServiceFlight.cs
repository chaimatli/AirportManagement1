using AM.ApplicationCore.Domain;
using AM.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
//using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AM.ApplicationCore.Services
{
    public class ServiceFlight : Service<Flight> , IServiceFlight 
    {

        public IList<Flight> Flights = new List<Flight>();

        public ServiceFlight(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        //Retourner les voyageurs d’un avion passé en paramètre.
        public IEnumerable<Passenger> GetPassengersByPlane(Plane plane)
        {
            return Get(f => f.Plane == plane)?.Passengers;
        }

        //Retourner les vols ordonnés par date de départ des n derniers avions.
        public IList<Flight> GetFlightsOrderedByDepartureDate(int n)
        {
            var flights = GetAll().OrderByDescending(f => f.FlightDate).Take(n);
            return flights.ToList();
        }

        //Retourner true si on peut réserver n place à un vol passé en paramètre.

        public bool CanReserveSeats(int n, Flight flight)
        {

            var capacity = Get(f => f.FlightId == flight.FlightId)?.Plane?.Capacity;

            if (capacity != null)
            {
                int remainingCapacity = (int)(capacity - (flight.Passengers?.Count ?? 0));
                return remainingCapacity >= n;
            }

            return false;
        }

        //Retourner la liste des staffs d’un vol dont son identifiant est passé en paramètre.
        public IList<Passenger> GetStaffs(int flightId)
        {
            var staffs = Get(f => f.FlightId == flightId)?.Passengers.OfType<Staff>();
            return (IList<Passenger>)staffs.ToList();
        }

        //Retourner la liste des voyageurs qui ont voyagé dans un avion donné à une date donnée.
        public IList<Passenger> GetTravellersByDate(DateTime date, Plane plane)
        {
            var travellers = Get(f => (f.Plane == plane) && (f.FlightDate == date))?.Passengers.OfType<Traveller>();
            return (IList<Passenger>)travellers.ToList();
        }

        //Afficher le nombre de voyageurs par date de vol.Cette dernière doit être comprise entre deux dates données.
        public int GetTravellersNumber(DateTime date1, DateTime date2)
        {
            var travellers = Get(f => (f.FlightDate > date1) && (f.FlightDate < date2))?.Passengers.OfType<Traveller>();
            if (travellers != null)
            {

                return travellers.ToList().Count;
            }
            else { 
            Console.WriteLine("Pas de vol entre les 2 dates entrées");
            return 0;
            }
        }

        //---------------------------------------------------------//
        public IList<DateTime> GetFlightDates(string destination)
        {
            List<DateTime> ls = new List<DateTime>();
            //With for structure
            //for (int j = 0; j < Flights.Count; j++)
            //    if (Flights[j].Destination.Equals(destination))
            //        ls.Add(Flights[j].FlightDate);

            //With foreach structure
            //foreach(Flight f in Flights)
            //    if (f.Destination.Equals(destination))
            //        ls.Add(f.FlightDate);
            //return ls;

            //with LINQ language
            var query = from f in Flights
                      where
                      f.Destination.Equals(destination)
                      select f.FlightDate;
            return query.ToList();

            //with Lambda expressions
            // IEnumerable<DateTime> reqLambda = Flights.Where(f => f.Destination.Equals(destination)).Select(f => f.FlightDate);
        }

        public void GetFlights(string filterType, string filterValue)
        {
            switch (filterType)
            {
                case "Destination":
                    foreach (Flight f in Flights)
                    {
                        if (f.Destination.Equals(filterValue))
                            Console.WriteLine(f);
                    }
                    break;
                case "FlightDate":
                    foreach (Flight f in Flights)
                    {
                        if (f.FlightDate == DateTime.Parse(filterValue))

                            Console.WriteLine(f);

                    }
                    break;
                case "EffectiveArrival":
                    foreach (Flight f in Flights)
                    {
                        if (f.EffectiveArrival == DateTime.Parse(filterValue))
                            Console.WriteLine(f);
                    }
                    break;
            }

        }

        public void ShowFlightDetails(Plane plane)
        {
            var req = from f in Flights
                      where f.Plane == plane
                      select new { f.FlightDate, f.Destination };
            //  var reqLambda = Flights.Where(f => f.Plane == plane).Select(p => new { f.FlightDate, f.Destination });
            foreach (var v in req)
                Console.WriteLine("Flight Date; " + v.FlightDate + " Flight destination: " + v.Destination);
        }

        public int ProgrammedFlightNumber(DateTime startDate)
        {
            var req = from f in Flights
                      where (f.FlightDate).CompareTo(startDate) > 0 && (f.FlightDate - startDate).TotalDays < 7
                      select f;
            // var reqLambda = Flights.Where(f => DateTime.Compare(f.FlightDate, startDate) > 0 && (f.FlightDate - startDate).TotalDays < 7);
            return req.Count();

        }

        public double DurationAverage(string destination)
        {
            return (from f in Flights
                    where f.Destination.Equals(destination)
                    select f.EstimatedDuration).Average();
            // return Flights.Where(f=>f.Destination.Equals(destination)).Select(f=> f.EstimatedDuration).Average();
        }

        public IEnumerable<Flight> OrderedDurationFlights()
        {
            //var req = from f in Flights
            //          orderby f.EstimatedDuration descending
            //          select f;
            
            //return req;

            //lambda expression

            return Flights.OrderByDescending(f => f.EstimatedDuration);

        }
        public IEnumerable<Traveller> SeniorTravellers(Flight f)
        {

            var oldTravellers = from p in f.Passengers.OfType<Traveller>()
                                orderby p.BirthDate
                                select p;

            // var reqLambda = f.Passengers.OfType<Traveller>().OrderBy(p => p.BirthDate).Take(3);


            return oldTravellers.Take(3);
            //if we want to skip 3
            //return oldTravellers.Skip(3);

        }

        public IEnumerable<IGrouping <string, Flight>> DestinationGroupedFlights()
        {
            var req = from f in Flights
                      group f by f.Destination;

            //  var reqLambda = Flights.GroupBy(f => f.Destination);

            foreach (var g in req)
            {
                Console.WriteLine("Destination: " + g.Key);
                foreach (var f in g)
                    Console.WriteLine("Décollage: " + f.FlightDate);

            }
            return req;
        }

    }
}
