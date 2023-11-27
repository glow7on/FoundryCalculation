using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        /// <summary> Температура формы начальная Тфн, К </summary>
        public int initialTemperature;

        /// <summary> Теплопроводность формы λф, Вт/м·К </summary>
        public double thermalConductivity;

        /// <summary> Теплоемкость сф, Дж/кг·К </summary>
        public int heatCapacity;

        /// <summary> Плотность ρф, кг/м3 </summary>
        public int dencity;

        /// <summary> Теплоаккумулирующая способность bф, Вт·с1/2/(м2·К) </summary>
        public int heatStorageCapacity;

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

    //Сложность формы литниковой системы
    public class Complexity
    {
        public string name;
        public Complexity() { }
        public override string ToString() => $"{name}";
    }

    //Литниковая система
    public class MeltSupplyScheme
    {
        public string name;
        public string type; //Тип литниковой системы: Вертикально-щелевая, Сифонная, Боковая

        //Критерии шлакообразования для: простой, средней и тяжелой полости формы соответственно
        public int[] criteriaValues = new int[3];
        private Uri uri;
        public BitmapImage bitmapImage;
        public double flowCoefficient;

        public MeltSupplyScheme(string type, int[] criteriaValues, string path)
        {
            this.criteriaValues = criteriaValues;
            uri = new Uri(path, UriKind.Relative);
            bitmapImage = new BitmapImage(uri);
            this.type = type;
            flowCoefficient = FormCalculation.flowCoefficientDictionary[type];
        }
        public virtual double GetPathLengthFirst(double pathLength) => pathLength; //Возвращает длину пути для участка 1-2
        public virtual double GetPathLengthSecond(double pathLength) => pathLength; //Возвращает длину пути для участка 1-3
        public virtual double GetMeltPressure(double formHeight, double meltPressure) => meltPressure; //Возвращает напор расплава Н
        public override string ToString() => $"{name}";
    }
}
