using System;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Matrix_App.PregeneratedMods.reflection
{
    public static class FieldWidgets
    {
        public delegate void UiEvent();
        
        public static Control[]? GetFieldWidget(FieldInfo field, object instance, UiEvent eventTask)
        {
            if (field.IsStatic || !field.IsDefined(typeof(UiWidget))) 
                return null;
            
            // fallback
            return GetDefaultFieldUi(field, field.GetValue(instance), (instance as MatrixGifGenerator)!, eventTask);
        }
        
        private static Control[] GetDefaultFieldUi(FieldInfo field, object? fieldValue, MatrixGifGenerator generator, UiEvent eventTask)
        {
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                AutoSize = true
            };
            
            var title = GetBetterFieldName(field.Name);

            var description = new Label();
            
            if (Attribute.GetCustomAttribute(field, typeof(UiDescriptionAttribute)) is UiDescriptionAttribute desc)
            {
                title = desc.title;
                description.Text = desc.description;
                description.ForeColor = Color.Gray;
                description.AutoSize = true;
            }  
            
            panel.Controls.Add(new Label
            {
                TextAlign = ContentAlignment.MiddleLeft,
                Text = title,
                Dock = DockStyle.Left,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
            });

            switch (fieldValue)
            {
                case int value:
                {
                    var upDown = new NumericUpDown
                    {
                        Width = 360,
                        Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                        Value = value,
                        Maximum = 1000
                    };
                    upDown.ValueChanged += (a, b) =>
                    {
                        field.SetValue(generator, (int) upDown.Value);
                        eventTask();
                    };

                    panel.Controls.Add(upDown);
                    break;
                }
                case bool value1:
                {
                    var upDown = new CheckBox
                    {
                        Dock = DockStyle.Fill, Anchor = AnchorStyles.Top | AnchorStyles.Right, Checked = value1
                    };
                    upDown.CheckedChanged += (a, b) =>
                    {
                        field.SetValue(generator, upDown.Checked);
                        eventTask();
                    };

                    panel.Controls.Add(upDown);
                    break;
                }
                case float floatValue:
                {
                    var upDown = new TrackBar
                    {
                        Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                        Maximum = 100,
                        Minimum = 0,
                        Value = (int) (floatValue * 100.0f),
                        Width = 360,
                        TickFrequency = 10,
                    };
                    upDown.ValueChanged += (a, b) =>
                    {
                        field.SetValue(generator, upDown.Value / 1e2f);
                        eventTask();
                    };

                    panel.Controls.Add(upDown);
                    break;
                }
            }

            return new Control[] {description, panel};
        }

        /// <summary>
        /// Generates a new name from standard class names
        /// Example: SomeClassA --> some class a
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetBetterFieldName(string name)
        {
            var groups = Regex.Match(name, @"([A-Z]*[a-z]+)([A-Z]+[a-z]*)|(.*)").Groups;

            var newName = "";

            for (var c = 1; c < groups.Count; c++)
            {
                newName += groups[c].Value.ToLower() + " ";
            }

            return newName;
        }
    }
}