using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Skills
{
    public abstract class SkillFactory
    {
        // Note: calling this method won't actually increase your ELO/MMR
        public static Skill GetSkill(JToken skill)
        {
            var name = skill.Value<String>("name");
            if (name is null) throw new ArgumentException();

            switch (skill.Value<String>("type").ToLower())
            {
                case "numrangeskill":
                    var minValue = skill.Value<int?>("minValue");
                    var maxValue = skill.Value<int?>("maxValue");
                    if (minValue is null || maxValue is null)
                    {
                        throw new ArgumentException();
                    }
                    return new NumRangeSkill(name, skill.Value<int>("value"), (int)minValue, (int)maxValue);
                default:
                    throw new Exception();
            }
        }
    }
}
