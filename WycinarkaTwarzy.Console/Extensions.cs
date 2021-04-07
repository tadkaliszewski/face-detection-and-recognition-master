using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WycinarkaTwarzy.Console
{
    public static class Extensions
    {
        public static System.Drawing.Rectangle Scale
            (this System.Drawing.Rectangle value, decimal scale,
                decimal shiftX = 0, decimal shiftY = 0)
        {
            var newWidth = (int)(value.Width + (value.Width * scale) / 100);

            var newHeight = (int)(value.Height + (value.Height * scale) / 100);

            var newX = value.X - ((newWidth - value.Width) / 2);

            if (shiftX != 0)
            {
                newX = (int)(newX + (newX * shiftX) / 100);
            }

            var newY = value.Y - ((newHeight - value.Height) / 2);

            if (shiftY != 0)
            {
                newY = (int)(newY + (newY * shiftY) / 100);
            }

            return new System.Drawing.Rectangle(newX, newY, newWidth, newHeight);
        }
    }
}
