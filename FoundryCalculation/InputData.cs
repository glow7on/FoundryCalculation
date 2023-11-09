using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FoundryCalculation
{
    //Сплавы
   public class Alloys
    {
        public string name;
        public int liquidusTemperature; //Температура ликвидус Тл, К
        public int solidusTemperature; //Температура солидус Тс, К
        public int heatCapacity; //Теплоемкость Сж, Дж/Кг*К
        public int liquidMeltDensity; //Плотность жидкого расплава p(ж), Кг/м^3
        public int heatOutput; // Теплопроводность λж, Вт/м·К
        public double heatStorageCapacity;//Теплоаккумулирующая способность bж, Вт·с1/2/(м2·К)
        public double thermalConductivity; //Температуропроводность a(ж), м^2/с
        public int kineticViscosity; //Кинетическая вязкость v, 10^-7 * м^2/с
        public double surfaceTension; //Поверхностное натяжение σ, H/м
        public double flowStopTemperature; //Температура остановки потока Т0, К
        public override string ToString() => $"{name}";

        public Alloys(int liquidusTemperature, int solidusTemperature, int heatCapacity, int liquidMeltDensity,
            int heatOutput, double heatStorageCapacity, double thermalConductivity, int kineticViscosity,
            double surfaceTension, double flowStopTemperature)
        {
            this.liquidusTemperature = liquidusTemperature;
            this.solidusTemperature = solidusTemperature;
            this.heatCapacity = heatCapacity;
            this.liquidMeltDensity = liquidMeltDensity;
            this.heatOutput = heatOutput;
            this.heatStorageCapacity = heatStorageCapacity;
            this.thermalConductivity = thermalConductivity;
            this.kineticViscosity = kineticViscosity;
            this.surfaceTension = surfaceTension;
            this.flowStopTemperature = flowStopTemperature;
        }
    }

    //Состав смеси
    public class Mixture
    {
        public string name;
        public int initialTemperature; //Температура формы начальная Тфн, К
        public double thermalConductivity; //Теплопровод-ность формы λф, Вт/м·К
        public int heatCapacity; //Теплоемкость сф, Дж/кг·К
        public int dencity; //Плотность ρф, кг/м3
        public int heatStorageCapacity;//Теплоаккумулирующая способность bф, Вт·с1/2/(м2·К)
        public Mixture(int initialTemperature, double thermalConductivity, int heatCapacity, int dencity, int heatStorageCapacity)
        {
            this.initialTemperature = initialTemperature;
            this.thermalConductivity = thermalConductivity;
            this.heatCapacity = heatCapacity;
            this.dencity = dencity;
            this.heatStorageCapacity = heatStorageCapacity;
        }
        public override string ToString() => $"{name}";
    }

    //Свойства покрытий и припылов
    public class Coverage
    {
        public string name;
        public double thermalConductivity; //Теплопроводность
        public Coverage(double thermalConductivity)
        {
            this.thermalConductivity = thermalConductivity;
        }
        public override string ToString() => $"{name}";
    }

    public class Complexity
    {
        public string name;
        public Complexity() { }
        public override string ToString() => $"{name}";
    }
}
