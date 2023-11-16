using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AM.ApplicationCore.Domain
{
    public class ReservationTicket
    {
        public double Prix { get; set; }
        public DateTime DateReservation { get; set; }

        [ForeignKey("TicketFk")]
        public virtual Ticket Ticket { get; set; }//relation
      // 1 ere methode
        [ForeignKey("PassengerFk")]
        public virtual Passenger Passenger { get; set; }//relation

        public string PassengerFk { get; set; }
        public int TicketFk { get; set; }
    }
}
