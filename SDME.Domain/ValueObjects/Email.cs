using SDME.Domain.Base;
using System.Text.RegularExpressions;

namespace SDME.Domain.ValueObjects
{
    /// <summary>
    /// Value Object para Email con validación
    /// </summary>
    public class Email : ValueObject
    {
        public string Valor { get; private set; }

        public Email(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("El email no puede estar vacío", nameof(valor));

            if (!EsValido(valor))
                throw new ArgumentException("El formato del email no es válido", nameof(valor));

            Valor = valor.ToLower().Trim();
        }

        private static bool EsValido(string email)
        {
            var patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, patron, RegexOptions.IgnoreCase);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Valor;
        }

        public override string ToString() => Valor;

        // Conversión implícita desde string
        public static implicit operator string(Email email) => email.Valor;
    }
}