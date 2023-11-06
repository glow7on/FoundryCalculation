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
            Alloys[] aluminiumAlloys = new Alloys[] {
               new Alloys(909, 763, 1244, 2200, 83, 15071.64, 0.0000303, 6, 0.86, 865.2) {name = "АЛ1"},
               new Alloys(864, 850, 1286, 2200, 83, 15323.96, 0.0000293, 6, 0.86, 859.8) {name = "АК12 (АЛ2)"},
               new Alloys(888, 821, 1194, 2200, 83, 14765.65, 0.0000316, 6, 0.86, 867.9) {name = "АЛ3 (АК5М2Мг)"},
               new Alloys(867, 840, 1274, 2200, 83, 15252.29, 0.0000296, 6, 0.86, 858.9) {name = "АК9ч (АЛ4)"},
               new Alloys(831, 884, 1261, 2200, 83, 15174.27, 0.0000299, 6, 0.86, 846.9) {name = "АЛ6"},
               new Alloys(911, 784, 1249, 2200, 83, 15101.9,  0.0000302, 6, 0.86, 872.9) {name = "АЛ7"},
               new Alloys(878, 793, 1295, 2200, 83, 15377.48, 0.0000291, 6, 0.86, 852.5) {name = "АЛ8"},
               new Alloys(889, 843, 1282, 2200, 83, 15300.1,  0.0000294, 6, 0.86, 875.2) {name = "АК7ч (АЛ9)"},
               new Alloys(873, 773, 1265, 2200, 83, 15198.32, 0.0000298, 6, 0.86, 843) {name = "АЛ10"},
               new Alloys(858, 824, 1182, 2200, 83, 14691.26, 0.0000319, 6, 0.86, 847.8) {name = "АК7Ц9 (АЛ11)"},
               new Alloys(914, 781, 1198, 2200, 83, 14790.36, 0.0000315, 6, 0.86, 874.1) {name = "АЛ12"},
               new Alloys(895, 858, 1286, 2200, 83, 15323.96, 0.0000293, 6, 0.86, 883.9) {name = "АМг5К (АЛ13)"},
               new Alloys(903, 773, 1244, 2200, 83, 15071.64, 0.0000303, 6, 0.86, 864) {name = "АЛ18"},
               new Alloys(917, 808, 1249, 2200, 83, 15101.9,  0.0000302, 6, 0.86, 884.3) {name = "АМ5 (АЛ19)"},
               new Alloys(843, 718, 1290, 2200, 83, 15347.77, 0.0000292, 6, 0.86, 805.5) {name = "АМг11 (АЛ22)"}
            };
            Alloys[] magnesiumAlloys = new Alloys[]
            {
                new Alloys(923, 918, 1254, 1600, 84, 12982.2, 0.00004186603, 7, 0.529, 921.5) {name = "Мл2"},
                new Alloys(901, 834, 1254, 1600, 84, 12982.2, 0.00004186603, 7, 0.529, 880.9) {name = "Мл3"},
                new Alloys(883, 673, 1254, 1600, 84, 12982.2, 0.00004186603, 7, 0.529, 820) {name = "Мл4"},
                new Alloys(880, 703, 1254, 1600, 84, 12982.2, 0.00004186603, 7, 0.529, 826.9) {name = "Мл5"},
                new Alloys(873, 713, 1254, 1600, 84, 12982.2, 0.00004186603, 7, 0.529, 825) {name = "Мл6"},
                new Alloys(913, 823, 1254, 1600, 84, 12982.2, 0.00004186603, 7, 0.529, 886) {name = "Мл10"},
                new Alloys(921, 866, 1254, 1600, 84, 12982.2, 0.00004186603, 7, 0.529, 904.5) {name = "Мл11"},
                new Alloys(908, 823, 1254, 1600, 84, 12982.2, 0.00004186603, 7, 0.529, 882.5) {name = "Мл12"},
                new Alloys(904, 812, 1254, 1600, 84, 12982.2, 0.00004186603, 7, 0.529, 876.4) {name = "Мл15"}
            };

            AlloySelection.ItemsSource = aluminiumAlloys;
        }
    }

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

        }
    }
}
