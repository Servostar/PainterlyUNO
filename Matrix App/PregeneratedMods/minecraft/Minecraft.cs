using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Matrix_App.Properties;

namespace Matrix_App.minecraft
{
    public class Minecraft : MatrixGifGenerator
    {
        private Bitmap? texture = Resources.Pumpkin;

        protected override void CreateUi(FlowLayoutPanel anchor, update invokeGenerator)
        {
            CreateButton(Resources.CreeperHead, "Creeper", anchor, invokeGenerator);
            CreateButton(Resources.EndermanHead, "Enderman", anchor, invokeGenerator);
            CreateButton(Resources.EmeraldBlock, "Emerald", anchor, invokeGenerator);
            CreateButton(Resources.CommandBlock, "Command Block", anchor, invokeGenerator);
            CreateButton(Resources.DiamondOre, "Diamond ore", anchor, invokeGenerator);
            CreateButton(Resources.GrassBlock, "Grass", anchor, invokeGenerator);
            CreateButton(Resources.Pumpkin, "Pumpkin", anchor, invokeGenerator);
            CreateButton(Resources.RedstoneLamp, "Redstone lamp", anchor, invokeGenerator);
            CreateButton(Resources.TNT, "TNT", anchor, invokeGenerator);
            CreateButton(Resources.BlueWool, "Blue wool", anchor, invokeGenerator);
        }

        private void CreateButton(Bitmap bitmap, string title, FlowLayoutPanel anchor, update invokeGenerator)
        {
            var button = new Button()
            {
                Text = title,
                AutoSize = true,
                Image = bitmap,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight
            };
            button.Width = anchor.ClientSize.Width - button.Margin.Left - button.Margin.Right;
            button.Click += (a, b) =>
            {
                texture = bitmap;
                invokeGenerator();
            };
            anchor.Controls.Add(button);
        }

        protected override void ColorFragment(in int x, in int y, in float u, in float v, in int frame, out float r, out float g, out float b)
        {
            var color = texture!.GetPixel((int) (u * texture.Width), (int) (v * texture.Height));

            r = color.R / 255.0f;
            g = color.G / 255.0f;
            b = color.B / 255.0f;
        }
    }
}