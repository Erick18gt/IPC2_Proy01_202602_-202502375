namespace Proyecto1.Modelos
{
    public abstract class Robot
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }

        protected Robot(string nombre, string tipo)
        {
            Nombre = nombre;
            Tipo = tipo;
        }
    }
}
