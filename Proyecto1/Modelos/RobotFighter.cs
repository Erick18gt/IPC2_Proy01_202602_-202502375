namespace Proyecto1.Modelos
{
    public class RobotFighter : Robot
    {
        public int CapacidadCombate { get; set; }

        public RobotFighter(string nombre, int capacidadCombate)
            : base(nombre, "ChapinFighter")
        {
            CapacidadCombate = capacidadCombate;
        }

        public override string ToString()
        {
            return Nombre + " (ChapinFighter - capacidad " + CapacidadCombate + ")";
        }
    }
}
