using System;

namespace Matrix_App.PregeneratedMods.reflection
{
    [AttributeUsage(AttributeTargets.Field)] 
    public class UiDescriptionAttribute : Attribute
    {
        public string title;
        public string description;

        public UiDescriptionAttribute(string title, string description)
        {
            this.title = title;
            this.description = description;
        }
    }
}