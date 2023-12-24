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
using System.Xml.Serialization;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace FoundryCalculation
{
    /// <summary>
    /// Логика взаимодействия для FormCalculation.xaml
    /// </summary>
    public partial class FormCalculation : Window
    {
        const double OXIDEFOAM = 0.000005; //Толщина оксидной пены
        const double millimetersInMeters = 1000;
        const double g = 9.81;
        const double S = 1;
        const double PI = 3.14;
        string imagePath; //Путь до файла с изображением

        //Входные данные (вводимые вручную)
        double formHeight; //Высота формы h0 (отливка)
        double formWidth; //Ширина формы b
        double formThick; //Толщина формы 
        double formLength; //Длина формы l
        double meltPressure; //Напор расплава Н
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

        int fillingTemperature; //Температура заливки

        //Общие выходные данные
        double pathLength; //Длина пути участка
        double crossSectionalArea; //Площадь поперечного сечения
        double halfWallThickness; //double castingWallThickness
        double reducedCastingSize; //Приведенный размер отливки
        double fillingRateLimit; //Предельно допустимая скорость заполнения
        double squareSection; //Площадь поперечного сечения (на участке 1-2)
        double squareInArrow; //Площадь поперечного сечения в узком месте

        double speedInArrow1_2; //Скорость течения в узком месте на участке 1-2
        double speedInArrow1_3; //Скорость течения в узком месте на участке 1-3
        double speedInArrow3_4; //Скорость течения в узком месте на участке 3-4
        double speedInArrow4_5; //Скорость течения в узком месте на участке 4-5

        double meltFlowRate1_2; // Скорость течения расплава на участке 1-2
        double meltFlowRate1_3; // Скорость течения расплава на участке 1-3
        double meltFlowRate3_4; // Скорость течения расплава на участке 3-4
        double meltFlowRate4_5; // Скорость течения расплава на участке 4-5

        double calculationPlessure; //Расчетный напор
        double meltHeatTransfer; //Теплоотдача расплава 
        double nusseltCriterion; //Критерий Нуссельта
        double pecleCriterion; //Критерий Пекле
        double slugFormationCriteria; //Критерий шлакообразования
        double meltThermalConductivity; //Температуропроводность расплава 

        double meltFlowFrontTemperature1_2; //Температура фронта потока расплава участка 1-2
        double meltFlowFrontTemperature1_3; //Температура фронта потока расплава участка 1-3
        double meltFlowFrontTemperature3_4; //Температура фронта потока расплава участка 3-4
        double meltFlowFrontTemperature4_5; //Температура фронта потока расплава учаска 4-5

        double flowStopTemperature; //Температура остановки потока расплава T0

        double meltPressure1_2; //Напор расплава на участке 1-2
        double meltPressure1_3; //Напор расплава на участке 1-3
        double meltPressure3_4; //Напор расплава на участке 3-4
        double meltPressure4_5; //Напор расплава на участке 4-5


        //Для расчета исполняемых размеров вертикально-щелевой литниковой системы
        double permissibleMeltFlowRate; //Допустимый расход расплава (металла)
        double thicknessGap; //Толщина щели 
        double spreadingAngle; //Угол растекания расплава
        double transverseSpreadingRate; //скорость поперечного растекания расплава в полости литейной формы
        double permissibleFlowHeight; //высоту потока расплава при допустимом расходе, м
        double reducedSize; //приведенный размер растекающегося потока расплава
        double spreadingMeltTransfer; //теплоотдача расплава при проточно-поперечном растекании,  Вт/(м2К)
        double maxSpreadingLength; //максимальную длину растекания расплава, м
        double expenseRatio; //Определяем возможность применения одного колодца
        double pitDiameter; //Диаметр колодца
        double riserSquare; //Площадь стояка
        double riserSize; //Размер стояка

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
                { "Вертикально-щелевая", 0.4 }
            };

            //Алюминиевый сплав
            Alloys[] aluminiumArray = new Alloys[] {
               new Alloys(909, 763, 1244, 2200, 83, 15071.64, 0.0000303, 0.0000006, 0.86, 865.2) {name = "АЛ1"},
               new Alloys(864, 850, 1286, 2200, 83, 15323.96, 0.0000293, 0.0000006, 0.86, 859.8) {name = "АК12 (АЛ2)"},
               new Alloys(888, 821, 1194, 2200, 83, 14765.65, 0.0000316, 0.0000006, 0.86, 867.9) {name = "АЛ3 (АК5М2Мг)"},
               new Alloys(867, 840, 1274, 2200, 83, 15252.29, 0.0000296, 0.0000006, 0.86, 858.9) {name = "АК9ч (АЛ4)"},
               new Alloys(831, 884, 1261, 2200, 83, 15174.27, 0.0000299, 0.0000006, 0.86, 846.9) {name = "АЛ6"},
               new Alloys(911, 784, 1249, 2200, 83, 15101.9,  0.0000302, 0.0000006, 0.86, 872.9) {name = "АЛ7"},
               new Alloys(878, 793, 1295, 2200, 83, 15377.48, 0.0000291, 0.0000006, 0.86, 852.5) {name = "АЛ8"},
               new Alloys(889, 843, 1282, 2200, 83, 15300.1,  0.0000294, 0.0000006, 0.86, 875.2) {name = "АК7ч (АЛ9)"},
               new Alloys(873, 773, 1265, 2200, 83, 15198.32, 0.0000298, 0.0000006, 0.86, 843) {name = "АЛ10"},
               new Alloys(858, 824, 1182, 2200, 83, 14691.26, 0.0000319, 0.0000006, 0.86, 847.8) {name = "АК7Ц9 (АЛ11)"},
               new Alloys(914, 781, 1198, 2200, 83, 14790.36, 0.0000315, 0.0000006, 0.86, 874.1) {name = "АЛ12"},
               new Alloys(895, 858, 1286, 2200, 83, 15323.96, 0.0000293, 0.0000006, 0.86, 883.9) {name = "АМг5К (АЛ13)"},
               new Alloys(903, 773, 1244, 2200, 83, 15071.64, 0.0000303, 0.0000006, 0.86, 864) {name = "АЛ18"},
               new Alloys(917, 808, 1249, 2200, 83, 15101.9,  0.0000302, 0.0000006, 0.86, 884.3) {name = "АМ5 (АЛ19)"},
               new Alloys(843, 718, 1290, 2200, 83, 15347.77, 0.0000292, 0.0000006, 0.86, 805.5) {name = "АМг11 (АЛ22)"}
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
                new Mixture(1, 1, 1, 1, 1) { name = "Отсутствие покрытия" },
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
                new SideScheme("Боковая", sideArray, "/Resources/Side.png") {name = "Боковой подвод"},
                new SideTwoScheme("Боковая", sideArray, "/Resources/SideTwo.png") {name = "Боковой подвод с двух сторон"},
                new SiphonScheme("Сифонная", siphonArray, "/Resources/Siphon.png") {name = "Сифонный подвод"},
                new SiphonTwoScheme("Сифонная", siphonArray, "/Resources/SiphonTwo.png") {name = "Сифонный подвод с двух сторон"},
                new TieredScheme("Сифонная", siphonArray, "/Resources/Tiered.png") {name = "Ярусный подвод с двух сторон"},
                new VerticallySlottedScheme("Вертикально-щелевая", verticallySlottedArray, "/Resources/VerticallySlotted.png") {name = "Вертикально-щелевой подвод"}
            };

            //Сложность конфигурации формы
            string[] ComplexityArray = new string[] { "Простая", "Средняя", "Сложная" };

            //Добавление массива объектов в листы для видимости в коде
            aluminiumAlloys.AddRange(aluminiumArray);
            magniumAlloys.AddRange(magnesiumArray);
            mixtures.AddRange(mixtureArray);
            coverages.AddRange(coverageArray);
            meltSupplySchemes.AddRange(meltSuppliesArray);

            //Добавление в UI блоки выбор из созданных объектов
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
                ChangeCurrentElements(); //Изменяет значения в выделенных объектах для упрощенного чтения кода и вызова

                halfWallThickness = formThick / 2;
                SquareSectionsCalculation();
                pathLength = formLength; //Вероятно изменить
                ReducedCastingSizeCalculation();
                SlugFormationCriteriaRefresh();
                FillingRateLimitCalculation();
                MeltPressureFirstToSecondCalculation();
                SpeedInArrowCalculation(this.speedInArrow1_2, this.meltPressure1_2, speedInArrowLabelFirst);
                SquareInArrowCalculation();
                MeltThermalConductivityCalculation();
                PecleCriterionCalculation();
                NusseltCriterionCalculation();
                MeltHeatTransferCaclculation();
                MeltFlowFrontTemperatureCalculationFirst(this.meltFlowFrontTemperature1_2, this.speedInArrow1_2);
                if (meltFlowFrontTemperature1_2 < currentAlloys.liquidusTemperature) { MessageBox.Show("Температура на участке 1-2 меньше температуры Ликвидус"); }

                MeltPressureFirstToThirdCalculation();
                SpeedInArrowCalculation(this.speedInArrow1_3, this.meltPressure1_3, speedInArrowLabelSecond);
                FlowRateCalculation(this.meltFlowRate1_3, this.meltPressure1_3); //Добавить вывод
                MeltFlowFrontTemperatureCalculationSecond(this.meltFlowFrontTemperature1_3, meltFlowRate1_3); //Добавить вывод
                if (meltFlowFrontTemperature1_3 < currentAlloys.liquidusTemperature) { MessageBox.Show("Температура на участке 1-3 меньше температуры Ликвидус"); }

                MeltPressureThirdToFourthCalculation();
                SpeedInArrowCalculation(this.speedInArrow3_4, this.meltPressure3_4, speedInArrowLabelThird);
                FlowRateCalculation(this.meltFlowRate3_4, this.meltPressure3_4);
                MeltFlowFrontTemperatureCalculation(this.meltFlowFrontTemperature3_4, meltFlowRate3_4, meltFrontTemperatureLabelThird);
                if (meltFlowFrontTemperature3_4 < currentAlloys.liquidusTemperature) { MessageBox.Show("Температура на участке 3-4 меньше температуры Ликвидус"); }

                MeltPressureFourthToFifthCalculation();
                SpeedInArrowCalculation(this.speedInArrow4_5, this.meltPressure4_5, speedInArrowLabelFourth);
                FlowRateCalculation(this.meltFlowRate4_5, this.meltPressure4_5);
                MeltFlowFrontTemperatureCalculation(this.meltFlowFrontTemperature4_5, meltFlowRate4_5, meltFrontTemperatureLabelFourth);
                if (meltFlowFrontTemperature4_5 < currentAlloys.liquidusTemperature) { MessageBox.Show("Температура на участке 4-5 меньше температуры Ликвидус"); }
            }
        }

        bool RefreshInputData() //Валидация данных (Проверка на корректность ввода)
        {
            bool check = true;
            try
            {
                formHeight = Double.Parse(formHeightBox.Text) / millimetersInMeters;
                formWidth = Double.Parse(formWidthBox.Text) / millimetersInMeters;
                formThick = Double.Parse(formThickBox.Text) / millimetersInMeters;
                formLength = Double.Parse(formLengthBox.Text) / millimetersInMeters;
                meltPressure = Double.Parse(meltPressureBox.Text) / millimetersInMeters;
                fillingTemperature = Int32.Parse(fillingTemperatureBox.Text);
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка входных данных: " + e);
                check = false;
            }
            if (AlloySelection.SelectedItem == null | MixtureSelection.SelectedItem == null | CoverageSelection.SelectedItem == null |
                meltSupplySchemesSelection.SelectedItem == null | ComplexitySelection.SelectedItem == null) { check = false; }
            return check;
        }

        void ChangeCurrentElements() //Обновляет введенные значения пользователя, записывая их в используемый* объект, для удобства расчетов и чтения кода
        {
            if (AluminiumRadioBtn.IsChecked == true) { currentAlloys = aluminiumAlloys[AlloySelection.SelectedIndex]; }
            else { currentAlloys = magniumAlloys[AlloySelection.SelectedIndex]; }
            currentMixture = mixtures[MixtureSelection.SelectedIndex];
            currentCoverage = coverages[CoverageSelection.SelectedIndex];
            currentMeltSupplyScheme = meltSupplySchemes[meltSupplySchemesSelection.SelectedIndex];
        }

        private void SwitchToAluminium(object sender, RoutedEventArgs e) //Смена выборки для сплавов на Алюминиевые
        {
            AlloySelection.ItemsSource = aluminiumAlloys;
            fillingTemperatureBox.IsEnabled = false;
            fillingTemperatureLabel.Content = "Рек.: ";
        }
        private void SwitchToMagnium(object sender, RoutedEventArgs e) //Смена выборки для сплавов на Магниевые
        {
            AlloySelection.ItemsSource = magniumAlloys;
            fillingTemperatureBox.IsEnabled = false;
            fillingTemperatureLabel.Content = "Рек.: ";
        }

        void SquareSectionsCalculation() //Площадь поперечного сечения участок 1-2
        {
            squareSection = Math.Round(formWidth * formThick, 3);
            squareFirstToSecondLabel.Content = squareSection;
        }

        void ReducedCastingSizeCalculation() //Приведенный размер отливки 
        {
            reducedCastingSize = Math.Round(formThick / 2, 4);
            reducedCastingSizeLabel.Content = reducedCastingSize;
        }
        void SlugFormationCriteriaRefresh() //Критерий шлакообразования (выбирается по сложности и типу системы)
        {
            slugFormationCriteria = currentMeltSupplyScheme.criteriaValues[ComplexitySelection.SelectedIndex];
        }
        void FillingRateLimitCalculation() //Предельно допустимая скорость заполнения полости литейной формы w(шл.)
        {
            fillingRateLimit = (slugFormationCriteria * currentAlloys.kineticViscosity * OXIDEFOAM * currentAlloys.surfaceTension)
                / (currentAlloys.liquidMeltDensity * Math.Pow(reducedCastingSize, 3));
            fillingRateLimit = Math.Round(Math.Pow(fillingRateLimit, (double)1 / 3), 3);

            fillingRateLimitLabel.Content = fillingRateLimit;
        }
        void MeltPressureFirstToSecondCalculation() //Определение напора расплава на участке 1-2
        {
            meltPressure1_2 = currentMeltSupplyScheme.GetMeltPressure(formHeight, meltPressure);
            meltPressureLabelFirst.Content = meltPressure1_2;
        }
        void SpeedInArrowCalculation(double speedInArrow, double meltPressure, Label speedInArrowLabel) //Скорость течения расплава в узком месте (стояке), м/с (1.1.5) (ω уз.)
        {
            speedInArrow = Math.Round(currentMeltSupplyScheme.flowCoefficient * Math.Sqrt(2 * g * meltPressure), 3);
            speedInArrowLabel.Content = speedInArrow;
        }
        void SquareInArrowCalculation() //Площадь поперечного сечения узкого места литниковой системы Fуз(1.1.4)
        {
            squareInArrow = Math.Round((squareSection * fillingRateLimit) / speedInArrow1_2, 5);
            squareInArrowLabel.Content = squareInArrow;
        }
        void MeltThermalConductivityCalculation() //Температуропроводность расплава аж (1.1.11)
        {
            meltThermalConductivity = Math.Round(currentAlloys.heatOutput / (currentAlloys.heatCapacity * currentAlloys.liquidMeltDensity), 6);
            meltThermalConductivityLabel.Content = meltThermalConductivity;
        }
        void PecleCriterionCalculation() //Критерий Пекле
        {
            pecleCriterion = Math.Round((fillingRateLimit * halfWallThickness) / meltThermalConductivity, 3);
            pecleCriterionLabel.Content = pecleCriterion;
        }
        void NusseltCriterionCalculation()//критерий Нуссельта
        {
            if (pecleCriterion < 50) { nusseltCriterion = 1; }
            else { nusseltCriterion = Math.Round(0.033 * Math.Pow(pecleCriterion, 0.82), 4); }
            nusseltCriterionLabel.Content = nusseltCriterion;
        }
        void MeltHeatTransferCaclculation() //Теплоотдача расплава α
        {
            meltHeatTransfer = Math.Round((currentAlloys.heatOutput * nusseltCriterion) / halfWallThickness, 3);
            meltHeatTransferLabel.Content = meltHeatTransfer;
        }

        void MeltFlowFrontTemperatureCalculationFirst(double meltFlowFrontTemperature, double flowSpeed) //Температура фронта потока расплава T(n-n+1)
        {
            meltFlowFrontTemperature = Math.Round(Math.Abs(fillingTemperature - currentMixture.initialTemperature) *
                Math.Exp((-currentMeltSupplyScheme.GetPathLengthFirst(pathLength) * meltHeatTransfer / 2) / 
                (currentAlloys.heatCapacity * currentAlloys.liquidMeltDensity * reducedCastingSize * flowSpeed *
                (1 + currentAlloys.heatStorageCapacity / currentMixture.heatStorageCapacity))) + currentMixture.initialTemperature, 3);

            meltFrontTemperatureLabelFirst.Content = meltFlowFrontTemperature;
        }
        void MeltFlowFrontTemperatureCalculationSecond(double meltFlowFrontTemperature, double flowSpeed) //Температура фронта потока расплава T(n-n+1)
        {
            meltFlowFrontTemperature = Math.Round(Math.Abs(fillingTemperature - currentMixture.initialTemperature) *
                Math.Exp((-currentMeltSupplyScheme.GetPathLengthSecond(pathLength) * meltHeatTransfer / 2) /
                (currentAlloys.heatCapacity * currentAlloys.liquidMeltDensity * reducedCastingSize * flowSpeed *
                (1 + currentAlloys.heatStorageCapacity / currentMixture.heatStorageCapacity))) + currentMixture.initialTemperature, 3);

            meltFrontTemperatureLabelSecond.Content = meltFlowFrontTemperature;
        }
        void MeltFlowFrontTemperatureCalculation(double meltFlowFrontTemperature, double flowSpeed, Label meltFrontTemperatureLabel) //Температура фронта потока расплава T(n-n+1)
        {
            meltFlowFrontTemperature = Math.Round(Math.Abs(fillingTemperature - currentMixture.initialTemperature) *
                Math.Exp((-pathLength * meltHeatTransfer / 2) /
                (currentAlloys.heatCapacity * currentAlloys.liquidMeltDensity * reducedCastingSize * flowSpeed *
                (1 + currentAlloys.heatStorageCapacity / currentMixture.heatStorageCapacity))) + currentMixture.initialTemperature, 3);

            meltFrontTemperatureLabel.Content = meltFlowFrontTemperature;
        }

        void MeltPressureFirstToThirdCalculation() //Напор расплава на участке 1-3 ДОБАВИТЬ ВЫВОД
        {
            meltPressure1_3 = currentMeltSupplyScheme.GetMeltPressure(formHeight, meltPressure) - 1 / 2 * (1 / 2 * formHeight);
            meltPressureLabelSecond.Content = meltPressure1_3;
        }
        void FlowRateCalculation(double meltFlowRate, double meltPressure) //скорость расплава при движении на участке ДОБАВИТЬ ВЫВОД
        {
            meltFlowRate = (squareInArrow * currentMeltSupplyScheme.flowCoefficient * Math.Sqrt(2 * g * meltPressure) / squareSection);
        }
        void MeltPressureThirdToFourthCalculation() //Напор расплава на участке 3-4 ДОБАВИТЬ ВЫВОД
        {
            meltPressure3_4 = (meltPressure1_3 - 1/2 * formHeight) - (1/4 * formHeight);
            meltPressureLabelThird.Content = meltPressure3_4;
        }
        void MeltPressureFourthToFifthCalculation() //Напор расплава на участке 4-5 ДОБАВИТЬ ВЫВОД
        {
            meltPressure4_5 = meltPressure3_4 - formHeight;
            meltPressureLabelFourth.Content = meltPressure4_5;
        }


        //Расчет Вертикально-щелевой 
        void permissibleMeltFlowRateСalculation() //допустимый расход металла
        {
            permissibleMeltFlowRate = (formLength + formHeight) * fillingRateLimit * formThick; // L = (l + ho)
        } 
        void thicknessGapCalculation() //Толщина щели
        {
            thicknessGap = formThick * 0.7;
        }
        void spreadingAngleCalculation() //угол растекания расплава исходя из  максимального расхода
        {
            spreadingAngle = 0.4 * Math.Pow(permissibleMeltFlowRate, 0.195) * Math.Pow(thicknessGap, -1.09);
        }
        void transverseSpreadingRateCalculation() //скорость поперечного растекания
        {
            transverseSpreadingRate = Math.Pow(Math.Pow(S,2) * (permissibleMeltFlowRate / (2 * reducedCastingSize) * Math.Sin(spreadingAngle)), 1.0 / 3);
        }
        void permissibleFlowHeightCalculation() //высотa потока расплава при допустимом расходе
        {
            permissibleFlowHeight = Math.Pow(Math.Pow(permissibleMeltFlowRate / (S * 2 * reducedCastingSize), 2) * (1 / Math.Sin(spreadingAngle)), 1.0 / 3);
        }
        void reducedSizeCalculation() //приведенный размер растекающегося потока
        {
            reducedSize = 2 * permissibleFlowHeight * 2 * reducedCastingSize / (2 * permissibleFlowHeight + 2 * reducedCastingSize);
        }
        void spreadingMeltTransferCalculation() //Теплоотдача расплава
        {
            spreadingMeltTransfer = (nusseltCriterion * currentAlloys.heatOutput) / reducedCastingSize;
        }
        void maxSpreadingLengthCalculation() //Определяем максимальную длину растекания расплава
        {   
            maxSpreadingLength = Math.Log((fillingTemperature - currentMixture.initialTemperature)
                / (currentAlloys.flowStopTemperature - currentMixture.initialTemperature)) * currentAlloys.heatCapacity *
                currentAlloys.liquidMeltDensity * reducedCastingSize * transverseSpreadingRate *
                (1 + currentAlloys.heatStorageCapacity / currentAlloys.heatStorageCapacity) / spreadingMeltTransfer;
        }
        void expenseRatioCalculation() 
        {
            if(maxSpreadingLength >= 1.2 * (formLength + formHeight))
            {

            }
        }
        void pitDiameterCalculation() 
        {
            pitDiameter = 4 * thicknessGap;
        }
        void riserSquareCalculation() 
        {
            riserSquare = permissibleMeltFlowRate / (currentMeltSupplyScheme.flowCoefficient * Math.Sqrt(2 * g * meltPressure));
        }
        void riserSizeCalculation() 
        {
            riserSize = Math.Sqrt(riserSquare / PI);
        }
        //Расчет Вертикально-щелевой /

        void MeltSupplySchemeChange(object sender, SelectionChangedEventArgs e)
        {
            MeltSupplyScheme selectedItem = meltSupplySchemes[meltSupplySchemesSelection.SelectedIndex];
            SupplySchemeImage.Source = selectedItem.bitmapImage;
            SelectedSchemeLabel.Content = "Выбранная схема подвода: " + selectedItem.name;
        }

        private void AlloySelectionChange(object sender, SelectionChangedEventArgs e)
        {
            fillingTemperatureBox.IsEnabled = true;
            try
            {
                if (AluminiumRadioBtn.IsChecked == true) { fillingTemperatureLabel.Content = "Рек.: " + (aluminiumAlloys[AlloySelection.SelectedIndex].liquidusTemperature + 50); }
                else { fillingTemperatureLabel.Content = "Рек.: " + (magniumAlloys[AlloySelection.SelectedIndex].liquidusTemperature + 50); }
            }
            catch{ }
        }
    }
}

