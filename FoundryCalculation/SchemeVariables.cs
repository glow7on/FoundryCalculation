using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoundryCalculation
{
    public class SideScheme : MeltSupplyScheme
    {
        public SideScheme(string type, int[] criteriaValues, string path) : base(type, criteriaValues, path)
        {

        }
    }

    public class SideTwoScheme : MeltSupplyScheme
    {
        public SideTwoScheme(string type, int[] criteriaValues, string path) : base(type, criteriaValues, path)
        {

        }
        public override double GetPathLength(double pathLength) => pathLength / 2;
    }

    public class SiphonScheme : MeltSupplyScheme
    {
        public SiphonScheme(string type, int[] criteriaValues, string path) : base(type, criteriaValues, path)
        {

        }
        public override double GetMeltPressure(double formHeight, double meltPressure) => meltPressure + (formHeight/2);

    }

    public class SiphonTwoScheme : MeltSupplyScheme
    {
        public SiphonTwoScheme(string type, int[] criteriaValues, string path) : base(type, criteriaValues, path)
        {

        }
        public override double GetMeltPressure(double formHeight, double meltPressure) => meltPressure + (formHeight / 2);
        public override double GetPathLength(double pathLength) => pathLength / 2;
    }

    public class TieredScheme : MeltSupplyScheme
    {
        public TieredScheme(string type, int[] criteriaValues, string path) : base(type, criteriaValues, path)
        {

        }
        public override double GetMeltPressure(double formHeight, double meltPressure) => meltPressure + (formHeight / 2);
        public override double GetPathLength(double pathLength) => pathLength / 2;
    }

    public class VerticallySlottedScheme : MeltSupplyScheme
    {
        public VerticallySlottedScheme(string type, int[] criteriaValues, string path) : base(type, criteriaValues, path)
        {

        }
    }
}
