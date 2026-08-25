using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto1.Modelo
{
    public class RobotFire : Robot
    {
        public int CapacidadCombate { get; set; }

        public RobotFire(string nombre, int capacidadCombate)
            : base(nombre, "Chapin Fire")
        {
            CapacidadCombate = capacidadCombate;
        }
    }
}