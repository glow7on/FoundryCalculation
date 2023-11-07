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
using static System.Collections.Specialized.BitVector32;

namespace FoundryCalculation
{
    /// <summary>
    /// Логика взаимодействия для FormCalculation.xaml
    /// </summary>
    public partial class FormCalculation : Window
    {
        int crossSectionalArea; //Площадь поперечного сечения
        double reducedCastingSize; //Приведенный размер отливки
        double fillingRateLimit; //Предельно допустимая скорость заполнения
        double squareFirstToSecond; //Площадь поперечного сечения на участке 1-2
        double speedInArrow; //Скорость течения в узком месте
        double squareInArrow; //Площадь поперечного сечения в узком месте
        double meltFlowRate; // Коэффициент расхода расплава
        double calculationPlessure; //Расчетный напор
        double meltHeatTransfer; //Теплоотдача расплава 
        double meltFrontTemperature; //Температура фронта потока расплава T1-2 в узле 2.э
        double nusseltCriterion; //Критерий Нуссельта
        double pecleCriterion; //Критерий Пекле
        double meltThermalConductivity; //Температуропроводность расплава 
        double meltFlowFrontTemperature; //Температура фронта потока расплава
        double flowStopTemperature; //Температура остановки потока расплава T0

        //Для расчета исполняемых размеров вертикально-щелевой литниковой системы
        double permissibleMeltFlowRate; //Допустимый расход расплава
        double thicknessGap; //Толщина щели 
        double spreadingAngle; //Угол растекания расплава
        double transverseSpreadingRate; //скорость поперечного растекания расплава в полости литейной формы
        double permissibleFlowHeight; //высоту потока расплава при допустимом расходе, м
        double reducedSize; //приведенный размер растекающегося потока расплава
        double spreadingMeltTransfer; //теплоотдача расплава при проточно-поперечном растекании,  Вт/(м2К)
        double maxSpreadingLength; //максимальную длину растекания расплава, м
        double expenseRatio; //Коэффициент расхода
        double riserSquare; //Площадь стояка
        double riserSize; //Размер стояка
        double pitDiameter; //Диаметр колодца

        public FormCalculation()
        {
            InitializeComponent();

            //Алюминиевый сплав
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

            //Магниевый сплав
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

            //Состав смеси
            Mixture[] mixtures = new Mixture[]
            {
                new Mixture(293, 0.510, 1100, 1600, 950) {name = "Типовая смесь для алюминиевых и магниевых отливок"},
                new Mixture(290, 1.28,  1080, 1650, 1600) {name = "Формовочная песчано-глинистая сухая с 10% глины"},//290-1790
                new Mixture(290, 0.705, 1650, 1600, 1377) {name = "Стержневая с 0,5 % сульфидной барды и 19% древесных опилок, сухая"},//290-1570
                new Mixture(290, 2.560, 1980, 2700, 3700) {name = "Хромомагнезитовая жидкостекольная с 6% жидкого стекла"},//290-1850
                new Mixture(290, 0.326, 795,  1500, 620) {name = "Кварцевый песок, сухой"},
                new Mixture(290, 1.130, 2100, 1650, 1970) {name = "Кварцевый песок, влажный"}
            };

            Coverage[] coverages = new Coverage[]
            {
                new Coverage(0.4) {name = "Графит"},
                new Coverage(0.207) {name = "Тальк"},
                new Coverage(0.29) {name = "Гипс"},
                new Coverage(0.174) {name = "Мел"},
                new Coverage(0.17) {name = "Маршалит"},
                new Coverage(0.41) {name = "Прокаленный тальк"},
                new Coverage(0.09) {name = "Сажа"}
            };


            AlloySelection.ItemsSource = aluminiumAlloys;
            MixtureSelection.ItemsSource = mixtures;
            CoverageSelection.ItemsSource = coverages;
        }
    }
}

