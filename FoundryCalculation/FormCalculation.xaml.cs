using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FoundryCalculation
{
    /// <summary>
    /// Логика взаимодействия для FormCalculation.xaml
    /// </summary>
    public partial class FormCalculation : Window
    {
        const double OXIDEFOAM = 0.000005; //Толщина оксидной пены
        const double millimetersInMeters = 1000;
        const double G = 9.81;
        const int S = 1;
        const double PI = 3.14;
        //string imagePath; //Путь до файла с изображением

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

        double meltHeatTransfer; //Теплоотдача расплава 
        double nusseltCriterion; //Критерий Нуссельта
        double pecleCriterion; //Критерий Пекле
        double slugFormationCriteria; //Критерий шлакообразования
        double meltThermalConductivity; //Температуропроводность расплава 

        double meltFlowFrontTemperature1_2; //Температура фронта потока расплава участка 1-2
        double meltFlowFrontTemperature1_3; //Температура фронта потока расплава участка 1-3
        double meltFlowFrontTemperature3_4; //Температура фронта потока расплава участка 3-4
        double meltFlowFrontTemperature4_5; //Температура фронта потока расплава учаска 4-5

        double meltPressure1_2; //Напор расплава на участке 1-2
        double meltPressure1_3; //Напор расплава на участке 1-3
        double meltPressure3_4; //Напор расплава на участке 3-4
        double meltPressure4_5; //Напор расплава на участке 4-5

        //Для расчета исполняемых размеров вертикально-щелевой литниковой системы

        double estimatedSpreadingLength; //Расчетная длина растекания L (l+h0)
        double permissibleMeltFlowRate; //Допустимый расход расплава (металла)
        double thicknessGap; //Толщина щели 
        double spreadingAngle; //Угол растекания расплава
        double transverseSpreadingRate; //скорость поперечного растекания расплава в полости литейной формы
        double permissibleFlowHeight; //высоту потока расплава при допустимом расходе, м
        double reducedSize; //приведенный размер растекающегося потока расплава
        double spreadingMeltTransfer; //теплоотдача расплава при проточно-поперечном растекании,  Вт/(м2К)
        double maxSpreadingLength; //максимальную длину растекания расплава, м
        double riserSquare; //Площадь стояка
        double riserSize; //Размер стояка
        double pitDiameter; //Диаметр колодца

        public FormCalculation(bool pattern)
        {
            InitializeComponent();            
//            var sections = new List<Section>
//              {
//                  new Section { Name = "1-2", FlowRate = fillingRateLimit, SpeedInArrowLabel = speedInArrowLabelSecond, FlowRateLabel = FlowRateLabelSecond, MeltFrontTemperatureLabel = meltFrontTemperatureLabelFirst },
//                  new Section { Name = "1-3", SpeedInArrowLabel = speedInArrowLabelSecond, FlowRateLabel = FlowRateLabelSecond, MeltFrontTemperatureLabel = meltFrontTemperatureLabelSecond },
//                  new Section { Name = "3-4", SpeedInArrowLabel = speedInArrowLabelThird, FlowRateLabel = FlowRateLabelThird, MeltFrontTemperatureLabel = meltFrontTemperatureLabelThird },
//                  new Section { Name = "4-5", SpeedInArrowLabel = speedInArrowLabelFourth, FlowRateLabel = FlowRateLabelFourth, MeltFrontTemperatureLabel = meltFrontTemperatureLabelFourth }
//              };

//            foreach (var section in sections)
//            {
//                if (section.Name == "1-2")
//                {
//                    MeltFlowFrontTemperatureCalculation(ref section.MeltFlowFrontTemperature, section.FlowRate, section.MeltFrontTemperatureLabel);
//                }
//                else
//                {
//                    MeltPressureCalculation(section.Name);
//                    SpeedInArrowCalculation(ref section.SpeedInArrow, this.GetMeltPressure(section.Name), section.SpeedInArrowLabel);
//                    FlowRateCalculation(ref section.FlowRate, this.GetMeltPressure(section.Name), section.FlowRateLabel);
//                    MeltFlowFrontTemperatureCalculation(ref section.MeltFlowFrontTemperature, section.FlowRate, section.MeltFrontTemperatureLabel);
//                }

//                if (section.MeltFlowFrontTemperature < currentAlloys.liquidusTemperature)
//                {
//                    MessageBox.Show($"Температура на участке {section.Name} меньше температуры Ликвидус");
//                }
//            }

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
                new Alloys(923, 918, 1254, 1600, 84, 12982.2, 0.00004186603, 0.0000007, 0.529, 921.5) {name = "Мл2"},
                new Alloys(901, 834, 1254, 1600, 84, 12982.2, 0.00004186603, 0.0000007, 0.529, 880.9) {name = "Мл3"},
                new Alloys(883, 673, 1254, 1600, 84, 12982.2, 0.00004186603, 0.0000007, 0.529, 820) {name = "Мл4"},
                new Alloys(880, 703, 1254, 1600, 84, 12982.2, 0.00004186603, 0.0000007, 0.529, 826.9) {name = "Мл5"},
                new Alloys(873, 713, 1254, 1600, 84, 12982.2, 0.00004186603, 0.0000007, 0.529, 825) {name = "Мл6"},
                new Alloys(913, 823, 1254, 1600, 84, 12982.2, 0.00004186603, 0.0000007, 0.529, 886) {name = "Мл10"},
                new Alloys(921, 866, 1254, 1600, 84, 12982.2, 0.00004186603, 0.0000007, 0.529, 904.5) {name = "Мл11"},
                new Alloys(908, 823, 1254, 1600, 84, 12982.2, 0.00004186603, 0.0000007, 0.529, 882.5) {name = "Мл12"},
                new Alloys(904, 812, 1254, 1600, 84, 12982.2, 0.00004186603, 0.0000007, 0.529, 876.4) {name = "Мл15"}
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

            if (pattern == true)
            {
                formHeightBox.Text = "300";
                formWidthBox.Text = "250";
                formThickBox.Text = "5";
                formLengthBox.Text = "400";
                meltPressureBox.Text = "400";
                AlloySelection.SelectedIndex = 7;
                fillingTemperatureBox.IsEnabled = true;
                fillingTemperatureBox.Text = "1053";
                MixtureSelection.SelectedIndex = 0;
                CoverageSelection.SelectedIndex = 1;
                meltSupplySchemesSelection.SelectedIndex = 4;
                ComplexitySelection.SelectedIndex = 0;
            }
        }

        void StartCalculation(object sender, RoutedEventArgs e)
        {
            if (RefreshInputData())
            {
                ChangeCurrentElements(); //Изменяет значения в выделенных объектах для упрощенного чтения кода и вызова

                halfWallThickness = formThick / 2;
                SquareSectionsCalculation();
                pathLength = currentMeltSupplyScheme.GetPathLength(formLength);
                ReducedCastingSizeCalculation();
                SlugFormationCriteriaRefresh();
                FillingRateLimitCalculation();

                if (currentMeltSupplyScheme.type == "Вертикально-щелевая")
                {
                    EstimatedSpreadingLengthCalculation();
                    permissibleMeltFlowRateСalculation();
                    thicknessGapCalculation();
                    spreadingAngleCalculation();
                    transverseSpreadingRateCalculation();
                    permissibleFlowHeightCalculation();
                    reducedSizeCalculation();
                    PecleCriterionCalculation();
                    NusseltCriterionCalculation();
                    spreadingMeltTransferCalculation();
                    maxSpreadingLengthCalculation();
                    expenseRatioCalculation();
                }
                else
                {
                    meltFlowRate1_2 = fillingRateLimit;
                    FlowRateLabelFirst.Content = meltFlowRate1_2;

                    MeltPressureFirstToSecondCalculation();
                    SpeedInArrowCalculation(ref speedInArrow1_2, this.meltPressure1_2, speedInArrowLabelFirst);
                    SquareInArrowCalculation();
                    MeltThermalConductivityCalculation();
                    PecleCriterionCalculation();
                    NusseltCriterionCalculation();
                    MeltHeatTransferCaclculation();
                    MeltFlowFrontTemperatureCalculation(ref meltFlowFrontTemperature1_2, fillingRateLimit, meltFrontTemperatureLabelFirst);
                    if (meltFlowFrontTemperature1_2 < currentAlloys.liquidusTemperature) { MessageBox.Show("Температура на участке 1-2 меньше температуры Ликвидус"); }

                    MeltPressureFirstToThirdCalculation();
                    SpeedInArrowCalculation(ref speedInArrow1_3, this.meltPressure1_3, speedInArrowLabelSecond);
                    FlowRateCalculation(ref meltFlowRate1_3, this.meltPressure1_3, FlowRateLabelSecond);
                    MeltFlowFrontTemperatureCalculation(ref meltFlowFrontTemperature1_3, meltFlowRate1_3, meltFrontTemperatureLabelSecond);
                    if (meltFlowFrontTemperature1_3 < currentAlloys.liquidusTemperature) { MessageBox.Show("Температура на участке 1-3 меньше температуры Ликвидус"); }

                    MeltPressureThirdToFourthCalculation();
                    SpeedInArrowCalculation(ref speedInArrow3_4, this.meltPressure3_4, speedInArrowLabelThird);
                    FlowRateCalculation(ref meltFlowRate3_4, this.meltPressure3_4, FlowRateLabelThird);
                    MeltFlowFrontTemperatureCalculation(ref meltFlowFrontTemperature3_4, meltFlowRate3_4, meltFrontTemperatureLabelThird);
                    if (meltFlowFrontTemperature3_4 < currentAlloys.liquidusTemperature) { MessageBox.Show("Температура на участке 3-4 меньше температуры Ликвидус"); }

                    MeltPressureFourthToFifthCalculation();
                    SpeedInArrowCalculation(ref speedInArrow4_5, this.meltPressure4_5, speedInArrowLabelFourth);
                    FlowRateCalculation(ref meltFlowRate4_5, this.meltPressure4_5, FlowRateLabelFourth);
                    MeltFlowFrontTemperatureCalculation(ref meltFlowFrontTemperature4_5, meltFlowRate4_5, meltFrontTemperatureLabelFourth);
                    if (meltFlowFrontTemperature4_5 < currentAlloys.liquidusTemperature) { MessageBox.Show("Температура на участке 4-5 меньше температуры Ликвидус"); }
                }
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
            catch
            {
                MessageBox.Show("Недопустимый формат входных данных");
                check = false;
            }
            if (AlloySelection.SelectedItem == null | MixtureSelection.SelectedItem == null |
                meltSupplySchemesSelection.SelectedItem == null | ComplexitySelection.SelectedItem == null) { check = false; MessageBox.Show("Выберите сплав, схему и сложность формы"); }
            return check;
        }

        void ChangeCurrentElements() //Обновляет введенные значения пользователя, записывая их в используемый* объект, для удобства расчетов и чтения кода
        {
            if (AluminiumRadioBtn.IsChecked == true) { currentAlloys = aluminiumAlloys[AlloySelection.SelectedIndex]; }
            else { currentAlloys = magniumAlloys[AlloySelection.SelectedIndex]; }
            currentMixture = mixtures[MixtureSelection.SelectedIndex];
            if (CoverageSelection.SelectedItem != null) { currentCoverage = coverages[CoverageSelection.SelectedIndex]; }          
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
            squareSection = Math.Round(formWidth * formThick, 5);
            squareFirstToSecondLabel.Content = squareSection;
        }

        void ReducedCastingSizeCalculation() //Приведенный размер отливки 
        {
            reducedCastingSize = Math.Round(formThick / 2.0, 4);
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
            meltPressure1_2 = Math.Round(currentMeltSupplyScheme.GetMeltPressure(formHeight, meltPressure), 5);
            meltPressureLabelFirst.Content = meltPressure1_2;
        }
        void SpeedInArrowCalculation(ref double speedInArrow, double meltPressure, Label speedInArrowLabel) //Скорость течения расплава в узком месте, м/с (1.1.5) (ω уз.)
        {
            speedInArrow = Math.Round(currentMeltSupplyScheme.flowCoefficient * Math.Sqrt(2.0 * G * meltPressure), 4);
            speedInArrowLabel.Content = speedInArrow;
        }
        void SquareInArrowCalculation() //Площадь поперечного сечения узкого места литниковой системы Fуз(1.1.4)
        {
            squareInArrow = Math.Round((squareSection * fillingRateLimit) / speedInArrow1_2, 7);
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
        void MeltFlowFrontTemperatureCalculation(ref double meltFlowFrontTemperature, double flowSpeed, Label meltFrontTemperatureLabel) //Температура фронта потока расплава T(n-n+1)
        {
            if (CoverageSelection.SelectedItem == null)
            {
                meltFlowFrontTemperature = Math.Round(Math.Abs(fillingTemperature - currentMixture.initialTemperature) *
                Math.Exp((-pathLength * (meltHeatTransfer / 2.0)) /
                (currentAlloys.heatCapacity * currentAlloys.liquidMeltDensity * reducedCastingSize * flowSpeed *
                (1 + (currentAlloys.heatStorageCapacity / currentMixture.heatStorageCapacity)))) + currentMixture.initialTemperature, 0);
            }
            else
            {
                meltFlowFrontTemperature = Math.Round(Math.Abs(fillingTemperature - currentMixture.initialTemperature) *
                Math.Exp((-pathLength * meltHeatTransfer / 2.0 * Math.Sqrt(currentCoverage.thermalConductivity)) /
                (currentAlloys.heatCapacity * currentAlloys.liquidMeltDensity * reducedCastingSize * Math.Sqrt(currentMixture.thermalConductivity) * flowSpeed *
                (1 + currentAlloys.heatStorageCapacity / currentMixture.heatStorageCapacity))) + currentMixture.initialTemperature, 0);
            }

            meltFrontTemperatureLabel.Content = meltFlowFrontTemperature;
        }

        void MeltPressureFirstToThirdCalculation() //Напор расплава на участке 1-3
        {
            meltPressure1_3 = Math.Round(meltPressure1_2 - (0.5 * 0.5 * formHeight), 5);
            meltPressureLabelSecond.Content = meltPressure1_3;
        }
        void FlowRateCalculation(ref double meltFlowRate, double meltPressure, Label FlowRateLabel) //скорость расплава при движении на участке ДОБАВИТЬ ВЫВОД
        {
            meltFlowRate = Math.Round(squareInArrow * currentMeltSupplyScheme.flowCoefficient * Math.Sqrt(2 * G * meltPressure) / squareSection, 4);
            FlowRateLabel.Content = meltFlowRate;
        }
        void MeltPressureThirdToFourthCalculation() //Напор расплава на участке 3-4
        {
            meltPressure3_4 = Math.Round(meltPressure1_2 - (0.5 * formHeight) - (0.25 * formHeight), 5);
            meltPressureLabelThird.Content = meltPressure3_4;
        }
        void MeltPressureFourthToFifthCalculation() //Напор расплава на участке 4-5
        {
            meltPressure4_5 = Math.Round(meltPressure1_2 - formHeight, 5);
            meltPressureLabelFourth.Content = meltPressure4_5;
        }


        //Расчет Вертикально-щелевой 
        void EstimatedSpreadingLengthCalculation()
        {
            estimatedSpreadingLength = formLength + formHeight;
        }
        void permissibleMeltFlowRateСalculation() //допустимый расход металла
        {
            permissibleMeltFlowRate = Math.Round(estimatedSpreadingLength * fillingRateLimit * formThick, 5); // L = (l + ho)
            permissibleMeltFlowRateLabel.Content = permissibleMeltFlowRate;
        }
        void thicknessGapCalculation() //Толщина щели
        {
            thicknessGap = Math.Round(formThick * 0.7, 5);
            thicknessGapLabel.Content = thicknessGap;
        }
        void spreadingAngleCalculation() //угол растекания расплава исходя из  максимального расхода
        {
            spreadingAngle = Math.Round(0.4 * Math.Pow(permissibleMeltFlowRate, 0.195) * Math.Pow(thicknessGap, -1.09), 3);
            spreadingAngleLabel.Content = spreadingAngle;
        }
        void transverseSpreadingRateCalculation() //скорость поперечного растекания расплава в полости литейной форму
        {
            transverseSpreadingRate = Math.Round(Math.Pow(Math.Pow(S, 2) * (permissibleMeltFlowRate / (2 * reducedCastingSize) * Math.Sin(spreadingAngle)), 1.0 / 3), 3);
            transverseSpreadingRateLabel.Content = transverseSpreadingRate;
        }
        void permissibleFlowHeightCalculation() //высотa потока расплава при допустимом расходе
        {
            permissibleFlowHeight = Math.Round(Math.Pow(Math.Pow(permissibleMeltFlowRate / (S * 2 * reducedCastingSize), 2) * (1 / Math.Sin(spreadingAngle)), 1.0 / 3), 3);
            permissibleFlowHeightLabel.Content = permissibleFlowHeight;
        }
        void reducedSizeCalculation() //приведенный размер растекающегося потока
        {
            reducedSize = Math.Round(2 * permissibleFlowHeight * 2 * reducedCastingSize / (2 * permissibleFlowHeight + 2 * reducedCastingSize), 3);
            reducedSizeLabel.Content = reducedSize;
        }
        void spreadingMeltTransferCalculation() //Теплоотдача расплава
        {
            spreadingMeltTransfer = Math.Round((nusseltCriterion * currentAlloys.heatOutput) / reducedCastingSize, 3);
            spreadingMeltTransferLabel.Content = spreadingMeltTransfer;
        }
        void maxSpreadingLengthCalculation() //Определяем максимальную длину растекания расплава
        {
            maxSpreadingLength = Math.Round(Math.Log((fillingTemperature - currentMixture.initialTemperature)
                / (currentAlloys.flowStopTemperature - currentMixture.initialTemperature)) * currentAlloys.heatCapacity *
                currentAlloys.liquidMeltDensity * reducedCastingSize * transverseSpreadingRate *
                (1 + currentAlloys.heatStorageCapacity / currentMixture.heatStorageCapacity) / spreadingMeltTransfer, 4);
            maxSpreadingLengthLabel.Content = maxSpreadingLength;
        }
        void expenseRatioCalculation()
        {
            if (maxSpreadingLength >= (1.2 * estimatedSpreadingLength)) 
            {
                pitDiameterCalculation();
                riserSquareCalculation();
                riserSizeCalculation();
            }
            else { MessageBox.Show("Применение одного колодца невозможно"); }
        }
        void pitDiameterCalculation()
        {
            pitDiameter = 4 * thicknessGap;
            pitDiameterLabel.Content = pitDiameter;
        }
        void riserSquareCalculation()
        {
            riserSquare = Math.Round(permissibleMeltFlowRate / (currentMeltSupplyScheme.flowCoefficient * Math.Sqrt(2 * G * meltPressure)), 3);
            riserSquareLabel.Content = riserSquare;
        }
        void riserSizeCalculation()
        {
            riserSize = Math.Round(Math.Sqrt(riserSquare / PI), 3);
            riserSizeLabel.Content = riserSize;
        }
        //Расчет Вертикально-щелевой /

        private void AlloySelectionChange(object sender, SelectionChangedEventArgs e)
        {
            fillingTemperatureBox.IsEnabled = true;
            try
            {
                if (AluminiumRadioBtn.IsChecked == true) 
                { 
                    fillingTemperatureLabel.Content = "Рек.: " + (aluminiumAlloys[AlloySelection.SelectedIndex].liquidusTemperature + 50);
                    currentAlloys = aluminiumAlloys[AlloySelection.SelectedIndex];
                }
                else 
                { 
                    fillingTemperatureLabel.Content = "Рек.: " + (magniumAlloys[AlloySelection.SelectedIndex].liquidusTemperature + 50);
                    currentAlloys = magniumAlloys[AlloySelection.SelectedIndex];
                }
            }
            catch{ }

            alloyNameLabel.Content = currentAlloys.name;
            liquidusTemperatureLabel.Content = currentAlloys.liquidusTemperature.ToString();
            solidusTemperatureLabel.Content = currentAlloys.solidusTemperature.ToString();
            alloyHeatCapacityLabel.Content = currentAlloys.heatCapacity.ToString();
            liquidMeltDensityLabel.Content = currentAlloys.liquidMeltDensity.ToString();
            heatOutputLabel.Content = currentAlloys.heatOutput.ToString();
            alloyHeatStorageCapacityLabel.Content = currentAlloys.heatStorageCapacity.ToString();
            alloyThermalConductivityLabel.Content = currentAlloys.thermalConductivity.ToString();
            kineticViscosityLabel.Content = currentAlloys.kineticViscosity.ToString();
            surfaceTensionLabel.Content = currentAlloys.surfaceTension.ToString();
            flowStopTemperatureLabel.Content = currentAlloys.flowStopTemperature.ToString();
        }

        private void MixtureSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentMixture = mixtures[MixtureSelection.SelectedIndex];            
            initialTemperatureLabel.Content = currentMixture.initialTemperature.ToString();
            mixtureThermalConductivityLabel.Content = currentMixture.thermalConductivity.ToString();
            mixtureHeatCapacityLabel.Content = currentMixture.heatCapacity.ToString();
            dencityLabel.Content = currentMixture.dencity.ToString();
            mixtureHeatStorageCapacityLabel.Content = currentMixture.heatStorageCapacity.ToString();
        }
        private void CoverageSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CoverageSelection.SelectedIndex != -1)
            {
                currentCoverage = coverages[CoverageSelection.SelectedIndex];
                coverageNameLabel.Content = currentCoverage.name;
                thermalConductivityLabel.Content = currentCoverage.thermalConductivity;
            }
            else
            {
                coverageNameLabel.Content = "null";
                thermalConductivityLabel.Content = "null";
            }
            
        }
        void MeltSupplySchemeChange(object sender, SelectionChangedEventArgs e)
        {
            currentMeltSupplyScheme = meltSupplySchemes[meltSupplySchemesSelection.SelectedIndex];
            if (currentMeltSupplyScheme.type == "Вертикально-щелевая")
            {
                VericallyForm.Visibility = Visibility.Visible;
            }
            else
            {
                VericallyForm.Visibility = Visibility.Hidden;
            }
            SupplySchemeImage.Source = currentMeltSupplyScheme.bitmapImage;
            SelectedSchemeLabel.Content = "Выбранная схема подвода: " + currentMeltSupplyScheme.name;
            meltSupplySchemeTypeLabel.Content = currentMeltSupplyScheme.type;
            PathLengthLabel.Content = currentMeltSupplyScheme.GetPathLength(formLength);
            MeltPressureLabel.Content = currentMeltSupplyScheme.GetMeltPressure(formHeight, meltPressure);
        }
        private void DeleteCoverageSelection(object sender, RoutedEventArgs e)
        {
            CoverageSelection.SelectedIndex = -1;
        }

        private void ShowCalculatedValues(object sender, RoutedEventArgs e)
        {
            initialValues.Visibility = Visibility.Hidden;
        }

        private void ShowInitialValues(object sender, RoutedEventArgs e)
        {
            initialValues.Visibility = Visibility.Visible;
        }
    }
}