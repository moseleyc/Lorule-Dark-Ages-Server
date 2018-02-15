namespace Darkages.Types
{
    public class ElementManager
    {
        public enum Element
        {
            None = 0x00,
            Fire = 0x01,
            Water = 0x02,
            Wind = 0x03,
            Earth = 0x04,
            Light = 0x05,
            Dark = 0x06
        }

        public static readonly double[,] ElementTable = new double[7, 7]
        {
            //none fire  water wind  earth light dark  
            {1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00}, // none
            {1.00, 1.00, 0.05, 1.50, 1.00, 0.10, 0.10}, // fire
            {1.00, 1.50, 1.00, 1.00, 0.05, 0.10, 0.10}, // water
            {1.00, 0.05, 1.00, 1.00, 1.50, 0.10, 0.10}, // wind
            {1.00, 1.00, 1.50, 0.05, 1.00, 0.10, 0.10}, // earth
            {1.00, 1.00, 1.00, 1.00, 1.00, 0.05, 1.50}, // light
            {1.00, 1.00, 1.00, 1.00, 1.00, 1.50, 0.05}  // dark
        };
    }
}