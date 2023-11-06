using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FoundryCalculation
{
    /// <summary>
    /// Логика взаимодействия для FormCalculation.xaml
    /// </summary>
    public partial class FormCalculation : Window
    {
        public FormCalculation()
        {
            InitializeComponent();
            AlloySelection.ItemsSource = new AluminiumAlloys[]
            {
                new AluminiumAlloys(909, 763, 1244, 2200, 83, 15071.64, 0.0000303, 6, 0.86, 865.2) {name = "АЛ1"},
                new AluminiumAlloys() {name = "АК12 (АЛ2)"},
                new AluminiumAlloys() {name = "АЛ3 (АК5М2Мг)"},
                new AluminiumAlloys() {name = "АК9ч (АЛ4)"},
                new AluminiumAlloys() {name = "АЛ6"},
                new AluminiumAlloys() {name = "АЛ7"},
                new AluminiumAlloys() {name = "АЛ8"},
                new AluminiumAlloys() {name = "АК7ч (АЛ9)"},
                new AluminiumAlloys() {name = "АЛ10"},
                new AluminiumAlloys() {name = "АК7Ц9 (АЛ11)"},
                new AluminiumAlloys() {name = "АЛ12"},
                new AluminiumAlloys() {name = "АМг5К (АЛ13)"},
                new AluminiumAlloys() {name = "АЛ18"},
                new AluminiumAlloys() {name = "АМ5 (АЛ19)"},
                new AluminiumAlloys() {name = "АМг11 (АЛ22)"}
            };
        }
    }

    public class AluminiumAlloys
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
        public override string ToString() => $"{name};";

        public AluminiumAlloys(int liquidusTemperature, int solidusTemperature, int heatCapacity, int liquidMeltDensity,
            int heatOutput, double heatStorageCapacity, double thermalConductivity, int kineticViscosity,
            double surfaceTension, double flowStopTemperature)
        {

        }
    }
}
