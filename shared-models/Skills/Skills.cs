using System;

namespace Skills
{
    public abstract class Skill
    {
        public String Name { get; set; }
        public abstract override String ToString();
        public abstract String Type { get; }

        public Skill(String name)
        {
            Name = name;
        }
        // Get string representation of the value regardless of underlying type
        public abstract String GetStringValue();
    }

    // Skill represented by a number range
    public class NumRangeSkill : Skill
    {
        public override String Type
        {
            get => "NumRangeSkill";
        }
        public int MinValue { get; set; }
        public int MaxValue { get; set; } = 5;
        int _value;
        public int Value
        {
            get => _value;
            set
            {
                _value = Math.Max(MinValue, value);
                _value = Math.Min(MaxValue, value);
            }
        }

        public NumRangeSkill(String name, int value) : base(name)
        {
            Value = value;
        }

        public NumRangeSkill(String name, int value, int minValue, int maxValue) : this(name, value)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public override String GetStringValue()
        {
            return _value.ToString();
        }

        public override String ToString()
        {
            return String.Format("{0}: {1} out of {2}", Name, Value, MaxValue);
        }
    }

}
