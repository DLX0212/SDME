using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SDME.Domain.Enums
{
    /// Tipos de notificación que puede recibir el usuario (Email y Push)

    public enum TipoNotificacion
    {
        Email = 1,
        SMS = 2,
        Push = 3
    }
}
