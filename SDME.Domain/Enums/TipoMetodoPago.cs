using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SDME.Domain.Enums
{
    /// Metodos de pago disponibles: Stripe, PayPal, Azul, Contra Entrega
    public enum TipoMetodoPago
    {
        Stripe = 1,
        PayPal = 2,
        Azul = 3,
        ContraEntrega = 4
    }
}