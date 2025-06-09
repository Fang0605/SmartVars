using UnityEngine;
namespace SmartVars.Attributes
{

    public class SmartRangeAttribute : PropertyAttribute
    {
        public float Min;
        public float Max;

        public SmartRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }

}