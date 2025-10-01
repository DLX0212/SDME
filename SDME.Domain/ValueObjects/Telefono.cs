using SDME.Domain.Base;
using System.Text.RegularExpressions;

namespace SDME.Domain.ValueObjects
{
    /// <summary>
    /// Value Object para Teléfono con validación (República Dominicana)
    /// </summary>
    public class Telefono : ValueObject
    {
        public string Valor { get; private set; }

        public Telefono(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("El teléfono no puede estar vacío", nameof(valor));

            // Limpiar el valor (eliminar espacios, guiones, paréntesis)
            var limpio = Regex.Replace(valor, @"[\s\-\(\)]", "");

            if (!EsValido(limpio))
                throw new ArgumentException("El formato del teléfono no es válido", nameof(valor));

            Valor = limpio;
        }

        private static bool EsValido(string telefono)
        {
            // Validar formato RD: 809/829/849 + 7 dígitos
            var patron = @"^(809|829|849)\d{7}$";
            return Regex.IsMatch(telefono, patron);
        }

        public string Formateado => $"({Valor.Substring(0, 3)}) {Valor.Substring(3, 3)}-{Valor.Substring(6)}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Valor;
        }

        public override string ToString() => Formateado;

        public static implicit operator string(Telefono telefono) => telefono.Valor;
    }
}
