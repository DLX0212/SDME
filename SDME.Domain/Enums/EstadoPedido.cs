using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SDME.Domain.Enums
{
    public enum EstadoPedido
    {
        Recibido = 1,
        EnPreparacion = 2,
        EnCamino = 3,
        Entregado = 4,
        Cancelado = 5
    }
}
