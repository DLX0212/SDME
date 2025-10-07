using SDME.Domain.Base;

namespace SDME.Domain.ValueObjects
{
    /// Value Object para representar dinero con moneda
    public class Dinero : ValueObject
    {
        public decimal Monto { get; private set; }
        public string Moneda { get; private set; }

        public Dinero(decimal monto, string moneda = "DOP")
        {
            if (monto < 0)
                throw new ArgumentException("El monto no puede ser negativo", nameof(monto));

            Monto = monto;
            Moneda = moneda;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Monto;
            yield return Moneda;
        }

        public static Dinero operator +(Dinero a, Dinero b)
        {
            if (a.Moneda != b.Moneda)
                throw new InvalidOperationException("No se pueden sumar montos de diferentes monedas");

            return new Dinero(a.Monto + b.Monto, a.Moneda);
        }

        public static Dinero operator -(Dinero a, Dinero b)
        {
            if (a.Moneda != b.Moneda)
                throw new InvalidOperationException("No se pueden restar montos de diferentes monedas");

            return new Dinero(a.Monto - b.Monto, a.Moneda);
        }

        public static Dinero operator *(Dinero dinero, decimal multiplicador)
        {
            return new Dinero(dinero.Monto * multiplicador, dinero.Moneda);
        }

        public override string ToString()
        {
            return $"{Moneda} {Monto:N2}";
        }
    }
}