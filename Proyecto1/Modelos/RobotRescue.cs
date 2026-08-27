namespace Proyecto1.Modelos
{
    public class RobotRescue : Robot
    {
        public RobotRescue(string nombre)
            : base(nombre, "ChapinRescue")
        {
        }

        public override string ToString()
        {
            return Nombre + " (ChapinRescue)";
        }
    }
}
