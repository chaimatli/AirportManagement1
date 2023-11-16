using AM.ApplicationCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AM.Infrastructure.Configurations
{
    public class ReservationTicketConfiguration : IEntityTypeConfiguration<ReservationTicket>
    {
        public void Configure(EntityTypeBuilder<ReservationTicket> builder)
        {
            //config de la clé composée

            builder.HasKey(p => new
            {
                p.PassengerFk, // <=>   p.Passenger.PassportNumber,
                p.TicketFk,
                p.DateReservation
            });






        }
    }
}
