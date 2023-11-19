using System;
using System.IO;
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
using System.Xml.Linq;

namespace FoundryCalculation
{
    /// <summary>
    /// Логика взаимодействия для FormCalculation.xaml
    /// </summary>
    public partial class FormCalculation : Window
    {
        const double oxideFoam = 0.000005; //Толщина оксидной пены
        const int millimetersInMeters = 100;
        const double g = 9.8;
        string path;

        //Входные данные (вводимые вручную)
        int formHeight; //Высота формы h0
        int formWidth; //Ширина формы b
        int formThick; //Толщина формы 
        int formLength; //Длина формы l
        int meltPressure; //Напор расплава Н
        List<Alloys> aluminiumAlloys = new List<Alloys>();
        List<Alloys> magniumAlloys = new List<Alloys>();
        List<Mixture> mixtures = new List<Mixture>();
        List<Coverage> coverages = new List<Coverage>();
        List<MeltSupplyScheme> meltSupplySchemes = new List<MeltSupplyScheme>();

        public static Dictionary<string, double> flowCoefficientDictionary;

        //Выбираемые через "ComboBox" элементы
        Alloys currentAlloys;
        Mixture currentMixture;
        Coverage currentCoverage;
        MeltSupplyScheme currentMeltSupplyScheme;

        //Общие выходные данные ??
        int pathLength; //Длина пути участка
        int crossSectionalArea; //Площадь поперечного сечения
        double halfWallThickness; //double castingWallThickness
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
        double slugFormationCriteria; //Критерий шлакообразования
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
            //Табличные значения критерия шлакообразования для различных конфигураций по типу литниковой системы: простой, средней и сложной соответственно
            int[] verticallySlottedArray = new int[] { 150000, 18500, 6500 };
            int[] siphonArray = new int[] { 75000, 9000, 3100 };
            int[] sideArray = new int[] { 45000, 5500, 1800 };
            BitmapImage bitmapImage = new BitmapImage();

            flowCoefficientDictionary = new Dictionary<string, double>
            {
                { "Боковая", 0.45 },
                { "Сифонная", 0.4 },
                { "Вертикально-щелевая", 0.5 }
            };

            //Алюминиевый сплав
            Alloys[] aluminiumArray = new Alloys[] {
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
            Alloys[] magnesiumArray = new Alloys[]
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
            Mixture[] mixtureArray = new Mixture[]
            {
                new Mixture(293, 0.510, 1100, 1600, 950) {name = "Типовая смесь для алюминиевых и магниевых отливок"},
                new Mixture(290, 1.28,  1080, 1650, 1600) {name = "Формовочная песчано-глинистая сухая с 10% глины"},//290-1790
                new Mixture(290, 0.705, 1650, 1600, 1377) {name = "Стержневая с 0,5 % сульфидной барды и 19% древесных опилок, сухая"},//290-1570
                new Mixture(290, 2.560, 1980, 2700, 3700) {name = "Хромомагнезитовая жидкостекольная с 6% жидкого стекла"},//290-1850
                new Mixture(290, 0.326, 795,  1500, 620) {name = "Кварцевый песок, сухой"},
                new Mixture(290, 1.130, 2100, 1650, 1970) {name = "Кварцевый песок, влажный"}
            };
            //Покрытие
            Coverage[] coverageArray = new Coverage[]
            {
                new Coverage(0.4) {name = "Графит"},
                new Coverage(0.207) {name = "Тальк"},
                new Coverage(0.29) {name = "Гипс"},
                new Coverage(0.174) {name = "Мел"},
                new Coverage(0.17) {name = "Маршалит"},
                new Coverage(0.41) {name = "Прокаленный тальк"},
                new Coverage(0.09) {name = "Сажа"}
            };
            //Литниковые системы
            MeltSupplyScheme[] meltSuppliesArray = new MeltSupplyScheme[]
            {
                new MeltSupplyScheme("Боковая", sideArray, "/Resources/Side.png") {name = "Боковой подвод"},
                new MeltSupplyScheme("Боковая", sideArray, "/Resources/SideTwo.png") {name = "Боковой подвод с двух сторон"},
                new MeltSupplyScheme("Сифонная", siphonArray, "/Resources/Siphon.png") {name = "Сифонный подвод"},
                new MeltSupplyScheme("Сифонная", siphonArray, "/Resources/SiphonTwo.png") {name = "Сифонный подвод с двух сторон"},
                new MeltSupplyScheme("Сифонная", siphonArray, "/Resources/Tiered.png") {name = "Ярусный подвод с двух сторон"},
                new MeltSupplyScheme("Вертикально-щелевая", verticallySlottedArray, "/Resources/VerticallySlotted.png") {name = "Вертикально-щелевой подвод"}
            };
            //Сложность конфмгурации формы
            string[] ComplexityArray = new string[] { "Простая", "Средняя", "Сложная" };

            aluminiumAlloys.AddRange(aluminiumArray);
            magniumAlloys.AddRange(magnesiumArray);
            mixtures.AddRange(mixtureArray);
            coverages.AddRange(coverageArray);
            meltSupplySchemes.AddRange(meltSuppliesArray);

            AlloySelection.ItemsSource = aluminiumAlloys;
            MixtureSelection.ItemsSource = mixtures;
            CoverageSelection.ItemsSource = coverages;
            meltSupplySchemesSelection.ItemsSource = meltSupplySchemes;
            ComplexitySelection.ItemsSource = ComplexityArray;
        }

        void StartCalculation(object sender, RoutedEventArgs e)
        {
            if (RefreshInputData())
            {
                ChangeCurrentElements();

                halfWallThickness = formThick / 2;
                SquareSectionsCalculation();
                pathLength = formLength;
                ReducedCastingSizeCalculation();
                slugFormationCriteriaCalculation();
                fillingRateLimitCalculation();

            }
        }

        bool RefreshInputData()
        {
            bool check = true;
            try
            {
                formHeight = Int32.Parse(formHeightBox.Text);
                formWidth = Int32.Parse(formWidthBox.Text);
                formThick = Int32.Parse(formThickBox.Text);
                formLength = Int32.Parse(formLengthBox.Text);
                meltPressure = Int32.Parse(meltPressureBox.Text);
            }
            catch
            {
                MessageBox.Show("Ошибка входных данных");
                check = false;
            }
            if (AlloySelection.SelectedItem == null | MixtureSelection.SelectedItem == null | CoverageSelection.SelectedItem == null |
                meltSupplySchemesSelection.SelectedItem == null | ComplexitySelection.SelectedItem == null) { check = false; }
            return check;
        }

        void ChangeCurrentElements()
        {
            if (AluminiumRadioBtn.IsChecked == true) { currentAlloys = aluminiumAlloys[AlloySelection.SelectedIndex]; }
            else { currentAlloys = magniumAlloys[AlloySelection.SelectedIndex]; }
            currentMixture = mixtures[MixtureSelection.SelectedIndex];
            currentCoverage = coverages[CoverageSelection.SelectedIndex];
            currentMeltSupplyScheme = meltSupplySchemes[meltSupplySchemesSelection.SelectedIndex];
        }


        private void SwitchToAluminium(object sender, RoutedEventArgs e)
        {
            AlloySelection.ItemsSource = aluminiumAlloys;
        }

        private void SwitchToMagnium(object sender, RoutedEventArgs e)
        {
            AlloySelection.ItemsSource = magniumAlloys;
        }

        void SquareSectionsCalculation()
        {
            squareFirstToSecond = (formWidth / millimetersInMeters) * (formThick / millimetersInMeters);
            squareFirstToSecondLabel.Content = "Площадь поперечного сечения на участке 1-2: " + squareFirstToSecond;
        }

        void ReducedCastingSizeCalculation()
        {
            reducedCastingSize = (formThick / millimetersInMeters) / 2;
            reducedCastingSizeLabel.Content = "Приведенный размер отливки: " + reducedCastingSize;
        }


        void slugFormationCriteriaCalculation() //Добавить вывод?
        {
            slugFormationCriteria = currentMeltSupplyScheme.criteriaValues[ComplexitySelection.SelectedIndex];
        }

        void fillingRateLimitCalculation() //Добавить вывод
        {
            fillingRateLimit = Math.Pow((slugFormationCriteria * currentAlloys.kineticViscosity * oxideFoam * currentAlloys.surfaceTension) / (currentAlloys.liquidMeltDensity * reducedCastingSize), (double)1 / 3);
        }
        void speedInArrowCalculation()//скорость течения расплава в узком месте (стояке), м/с (1.1.5)
        {
            speedInArrow = currentMeltSupplyScheme.flowCoefficient * Math.Sqrt(2 * g * meltPressure);
        }
        void squareInArrowCalculation()//площадь поперечного сечения узкого места литниковой системы Fуз(1.1.4)
        {
            squareInArrow = (squareFirstToSecond * fillingRateLimit) / speedInArrow;
        }
        void meltThermalConductivityCalculation()//Температуропроводность расплава (1.1.11)
        {
            meltThermalConductivity = currentAlloys.heatOutput / (currentAlloys.heatCapacity * currentAlloys.liquidMeltDensity);
        }
        void pecleCriterionCalculation()//критерий Пекле
        {
            pecleCriterion = (fillingRateLimit * halfWallThickness) / meltThermalConductivity;
        }
        void nusseltCriterionCalculation()//критерий Нуссельта
        {
            if (pecleCriterion < 50)
            {
                nusseltCriterion = 1;
            }
            else
            {
                nusseltCriterion = 0.33 * Math.Pow(pecleCriterion, 0.82);
            }
        }
        void meltHeatTransferCaclculation()//Теплоотдача расплава 
        {
            meltHeatTransfer = (currentAlloys.heatOutput * nusseltCriterion) / halfWallThickness;
        }

        void MeltSupplySchemeChange(object sender, SelectionChangedEventArgs e)
        {
            MeltSupplyScheme selectedItem = meltSupplySchemes[meltSupplySchemesSelection.SelectedIndex];
            SupplySchemeImage.Source = selectedItem.bitmapImage;
            SelectedSchemeLabel.Content = "Выбранная схема подвода: " + selectedItem.name;
        }
    }
}

