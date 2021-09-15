using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;

namespace Microsoft.Msagl.GraphViewerGdi 
{
    public static class GraphViewerGDIExtensions 
    {
        public static Image SaveImageToMemory(this GViewer viewer, double imageScale)
        {
            int w = (int)Math.Ceiling(viewer.Graph.Width * imageScale);
            int h = (int)Math.Ceiling(viewer.Graph.Height * imageScale);

            var bitmap = new Bitmap(w, h, PixelFormat.Format32bppPArgb);
            using (Graphics graphics = Graphics.FromImage(bitmap)) 
            {
                DrawGeneral(viewer, w, h, graphics, imageScale);
            }

            return bitmap;
        }

        private static void DrawGeneral(GViewer viewer, int w, int h, Graphics graphics, double imageScale) {
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            DrawAll(viewer, w, h, graphics, imageScale);
        }

        private static void DrawAll(GViewer viewer, int w, int h, Graphics graphics, double imageScale) {
            if (viewer is null) {
                throw new ArgumentNullException(nameof(viewer));
            }

            if (graphics is null) {
                throw new ArgumentNullException(nameof(graphics));
            }
            //fill the whole image
            graphics.FillRectangle(new SolidBrush(Draw.MsaglColorToDrawingColor(viewer.Graph.Attr.BackgroundColor)),
                                   new RectangleF(0, 0, w, h));

            //calculate the transform
            double s = imageScale;
            Graph g = viewer.Graph;
            double x = 0.5 * w - s * (g.Left + 0.5 * g.Width);
            double y = 0.5 * h + s * (g.Bottom + 0.5 * g.Height);

            graphics.Transform = new Matrix((float)s, 0, 0, (float)-s, (float)x, (float)y);
            Draw.DrawPrecalculatedLayoutObject(graphics, viewer.DGraph);
        }

        
    }
}
