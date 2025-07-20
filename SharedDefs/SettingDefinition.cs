using System;
using System.Collections.Generic;

namespace SharedDefs
{
    public enum SettingType
    {
        Toggle,
        Slider,
        Dropdown,
        Description
    }

    public enum SettingCategory
    {
        Always,
        Early,
        Mid,
        Late
    }

    public enum ParentSection
    {
        Core,
        Royalty,
        Ideology,
        Biotech,
        Anomaly,
        Odyssey
    }

    public class SettingDefinition
    {
        public string Key;
        public string Label;
        public string Description;
        public SettingType Type;

        public Func<bool> Getter;
        public Action<bool> Setter;

        public Func<float> SliderGetter;
        public Action<float> SliderSetter;
        public float SliderMin;
        public float SliderMax;

        public Func<int> DropdownGetter;
        public Action<int> DropdownSetter;
        public List<string> DropdownOptions;

        public ParentSection Section;
        public SettingCategory Category;
    }

    public class SettingGroup
    {
        public string GroupKey;
        public string GroupLabel;
        public string GroupDescription;
        public ParentSection Section;
        public SettingCategory Category;
        public List<SettingDefinition> Items;
    }
}
