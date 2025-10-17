using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.NuevaSimlationconQwen.Equipments;

namespace Simulator.Client.HCPages.SimulationPlanneds.ProcessFlowDiagram
{
    // Clase base abstracta para nodos de equipo
    public abstract class EquipmentNodeBase
    {
        public Guid Id => Equipment.Id; 
        public ProccesEquipmentType Type => Equipment.EquipmentType; 
        public IEquipment Equipment { get; set; } = null!;
        public double X { get; set; }
        public double Y { get; set; }
        public string Name { get; set; } = string.Empty;

        public abstract string GetShapeSvg();
        public string GetFillColor()
        {
            return "#d3d3d3"; // Gris tenue (LightGray)
        }

        public string GetStrokeColor()
        {
            return "#a9a9a9"; // Gris más oscuro (DarkGray)
        }
        public virtual double GetConnectionPointX(bool isInput = false)
        {
            return isInput ? X - 40 : X + 40;
        }
        public virtual double GetConnectionPointY(bool isInput = false)
        {
            return Y;
        }
    }

    // Nodo para tanques de materia prima
    public class RawTankNode : EquipmentNodeBase
    {
        public override string GetShapeSvg()
        {
            // Tanque cilíndrico fondo plano, tapa cónica
            return $@"<path d=""M -35 20 L -35 -15 L 0 -25 L 35 -15 L 35 20 Z"" 
                  fill=""{GetFillColor()}"" stroke=""{GetStrokeColor()}"" stroke-width=""2"" />";
        }

        
    }

    // Nodo para tanques WIP
    public class WipTankNode : EquipmentNodeBase
    {
        public override string GetShapeSvg()
        {
            // Igual al MixerNode pero sin agitador
            return $@"<!-- Cuerpo cilíndrico (más delgado) -->
                  <rect x=""-30"" y=""-15"" width=""60"" height=""35"" 
                        fill=""{GetFillColor()}"" stroke=""{GetStrokeColor()}"" stroke-width=""2"" />
                  
                  <!-- Tapa torisférica (abombada hacia arriba - hacia afuera) -->
                  <path d=""M -30 -15 
                           Q 0 -35 30 -15"" 
                        fill=""{GetFillColor()}"" stroke=""{GetStrokeColor()}"" stroke-width=""2"" />
                  
                  <!-- Fondo torisférico -->
                  <path d=""M -30 20 
                           Q 0 40 30 20"" 
                        fill=""{GetFillColor()}"" stroke=""{GetStrokeColor()}"" stroke-width=""2"" />";
        }

       
        public override double GetConnectionPointX(bool isInput = false)
        {
            return isInput ? X - 30 : X + 30;
        }

        
    }

    // Nodo para mezcladores
    public class MixerNode : EquipmentNodeBase
    {
        public override string GetShapeSvg()
        {
            // Reducir ancho en 15%: 70 → 60, 35 → 30
            // Ancho original: 70, Nuevo ancho: 70 * 0.85 = 59.5 ≈ 60
            return $@"<!-- Cuerpo cilíndrico (más delgado) -->
                  <rect x=""-30"" y=""-15"" width=""60"" height=""35"" 
                        fill=""{GetFillColor()}"" stroke=""{GetStrokeColor()}"" stroke-width=""2"" />
                  
                  <!-- Tapa torisférica (ajustada al nuevo ancho) -->
                  <path d=""M -30 -15 
                           Q 0 -35 30 -15"" 
                        fill=""{GetFillColor()}"" stroke=""{GetStrokeColor()}"" stroke-width=""2"" />
                  
                  <!-- Fondo torisférico (ajustado al nuevo ancho) -->
                  <path d=""M -30 20 
                           Q 0 40 30 20"" 
                        fill=""{GetFillColor()}"" stroke=""{GetStrokeColor()}"" stroke-width=""2"" />
                  
                  <!-- Agitador -->
                  <line x1=""-10"" y1=""-5"" x2=""10"" y2=""-5"" 
                        stroke=""black"" stroke-width=""2"" />
                  <line x1=""0"" y1=""-5"" x2=""0"" y2=""5"" 
                        stroke=""black"" stroke-width=""2"" />
                  <circle cx=""0"" cy=""5"" r=""3"" fill=""black"" />";
        }

        //public override string GetFillColor()
        //{
        //    return "#e6ffe6"; // Verde tenue (Light Green)
        //}

        //public override string GetStrokeColor()
        //{
        //    return "#666666"; // Gris oscuro
        //}

        public override double GetConnectionPointX(bool isInput = false)
        {
            return isInput ? X - 30 : X + 30; // Ajustar puntos de conexión al nuevo ancho
        }

        
    }
  
    // Nodo para bombas
    public class PumpNode : EquipmentNodeBase
    {
        public override string GetShapeSvg()
        {
            // Círculo principal de la bomba (radio = 20, diámetro = 40)
            return $@"
                  
                  <!-- Triángulo relleno: conectando todos los puntos -->
                  <polygon points=""-16.5,13 -22,24 22,24 16.5,13"" 
                           fill=""{GetFillColor()}"" stroke=""{GetStrokeColor()}"" stroke-width=""2"" />
                
                  
                  <!-- Línea de salida: desde centro superior (como polígono relleno) -->
                  <!-- Puntos: (0,-20) → (23,-20) → (23,-10) → (17,-10) → (0,-10) → cerrar -->
                  <polygon points=""0 -20 23 -20 23 -10 17 -10 0 -10"" 
                           fill=""{GetFillColor()}"" stroke=""{GetStrokeColor()}"" stroke-width=""2"" />


                    <!-- Círculo principal de la bomba -->
                  <circle cx=""0"" cy=""0"" r=""20"" 
                         fill=""{GetFillColor()}"" stroke=""{GetStrokeColor()}"" stroke-width=""2"" />

  
                  <!-- Círculo pequeño central -->
                  <circle cx=""0"" cy=""0"" r=""3"" 
                         fill=""{GetStrokeColor()}"" />
";
        }

        //public override string GetFillColor()
        //{
        //    return "#d3d3d3"; // Gris tenue (LightGray)
        //}

        //public override string GetStrokeColor()
        //{
        //    return "#a9a9a9"; // Gris más oscuro (DarkGray)
        //}
        public override double GetConnectionPointX(bool isInput = false)
        {
            if (isInput)
            {
                return X - 20; // Entrada: borde izquierdo del círculo
            }
            else
            {
                return X + 23; // Salida: lado derecho del polígono de salida
            }
        }

       

        public override double GetConnectionPointY(bool isInput = false)
        {
            if (isInput)
            {
                return Y; // Entrada: centro vertical del círculo
            }
            else
            {
                return Y - 15; // Salida: punto medio del polígono (-15)
            }
        }
    }

    // Nodo para líneas
    public class LineNode : EquipmentNodeBase
    {
        public override string GetShapeSvg()
        {
            // Línea con nombre en el centro en lugar de "P"
            var displayName = Name.Length > 10 ? Name.Substring(0, 10) + "..." : Name;
            var encodedName = System.Web.HttpUtility.HtmlEncode(displayName);

            return $@"<rect x=""-40"" y=""-20"" width=""80"" height=""40"" rx=""5"" 
                        fill=""{GetFillColor()}"" stroke=""{GetStrokeColor()}"" stroke-width=""2"" />
                  <!-- Nombre de la línea en el centro -->
                 ";
        }

     

        public override double GetConnectionPointX(bool isInput = false)
        {
            return isInput ? X - 40 : X + 40;
        }

       
    }

    // Nodo por defecto
    public class DefaultEquipmentNode : EquipmentNodeBase
    {
        public override string GetShapeSvg()
        {
            return $@"<rect x=""-40"" y=""-25"" width=""80"" height=""50"" rx=""5"" 
                        fill=""{GetFillColor()}"" stroke=""{GetStrokeColor()}"" stroke-width=""2"" />";
        }

    }
    public class OperatorNode : EquipmentNodeBase
    {
        public override string GetShapeSvg()
        {
            // Versión aún más compacta
            return $@"<!-- Cabeza del operario -->
                  <circle cx=""0"" cy=""-12"" r=""8"" 
                         fill=""{GetFillColor()}"" stroke=""{GetStrokeColor()}"" stroke-width=""2"" />
                  
                  <!-- Cuerpo del operario -->
                  <rect x=""-6"" y=""-4"" width=""12"" height=""20"" 
                        fill=""{GetFillColor()}"" stroke=""{GetStrokeColor()}"" stroke-width=""2"" />";
        }

       

        public override double GetConnectionPointX(bool isInput = false)
        {
            return isInput ? X - 15 : X + 15; // Conectar desde los lados
        }

       
    }
    public class StreamJoinerNode : EquipmentNodeBase
    {
        public override string GetShapeSvg()
        {
            // Triángulo con base vertical izquierda (entradas) y punta derecha (salida)
            return $@"<polygon points=""-15,-20  -15,20  25,0"" 
                        fill=""{GetFillColor()}"" stroke=""{GetStrokeColor()}"" stroke-width=""2"" />";
        }

        

        // Punto de entrada: centro de la base izquierda (x = -15)
        public override double GetConnectionPointX(bool isInput = false)
        {
            return isInput ? X - 15 : X + 25; // Entrada: base izq | Salida: punta derecha
        }

        public override double GetConnectionPointY(bool isInput = false)
        {
            return Y; // Todas las conexiones centradas verticalmente
        }
    }
}
