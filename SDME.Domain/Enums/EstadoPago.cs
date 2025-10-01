using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SDME.Domain.Enums
{
    public enum EstadoPago
    {
        Pendiente = 1,
        Procesando = 2,
        Aprobado = 3,
        Rechazado = 4,
        Reembolsado = 5
    }
}
