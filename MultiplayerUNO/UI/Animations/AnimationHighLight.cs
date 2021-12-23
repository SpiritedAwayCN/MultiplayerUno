using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiplayerUNO.UI.Animations {
    public class AnimationHighLight {
        public const int SLEEP_TIME = 10;
        public readonly Form MainForm;
        public readonly Control LableControlled;
        private int Steps = 200;
        // alpha
        public readonly bool CanDoAnimation;
        private bool Zero2One = true;
        // scale
        private float Scale = 2.0f;
        private int Turns = 2;
        private float StartScale = 1.0f;

        public AnimationHighLight(Form form, Label lbl) {
            this.MainForm = form;
            this.LableControlled = lbl;
            Bitmap[] bmp = lbl.Tag as Bitmap[];
            this.CanDoAnimation = (bmp != null) && (bmp.Length >= 2);
        }

        /// <param name="zero2One">
        /// true: 从 Label.Tag 的第一张图片变为第二张图片
        /// </param>
        public bool SetDirection(bool zero2One = true) {
            if (!CanDoAnimation) { return false; }
            Zero2One = zero2One;
            return true;
        }

        public void SetScale(
            float scale = 2.0f, int turns = 2, float startScale = 1.0f) {
            Scale = scale;
            Turns = turns;
            StartScale = startScale;
        }

        public bool SetSteps(int steps = 200) {
            if (!CanDoAnimation) { return false; }
            Steps = steps;
            return true;
        }

        public Task Run() {
            return Run(new CancellationTokenSource());
        }


        public Task Run(CancellationTokenSource token) {
            return Task.Factory.StartNew(() => {
                Point pos = LableControlled.Location;
                Size size = LableControlled.Size;
                // alpha
                float delta = 2.0f / Steps;
                float val = 1.0f;
                int idx = Zero2One ? 0 : 1;
                float eps = delta / 2;
                // scale
                float valScale = 0;
                float intervalScale = (Scale - 1.0f);
                float deltaScale = (Turns * 2 * intervalScale) / Steps;
                float twiceIntervalScale = intervalScale * 2;
                Bitmap bmp = ((Bitmap[])this.LableControlled.Tag)[idx];
                for (int i = 0; i < Steps; ++i) {
                    Bitmap bmpNew = null;
                    if (CanDoAnimation) {
                        val -= delta;
                        if (Math.Abs(val) < eps) {
                            bmp = ((Bitmap[])this.LableControlled.Tag)[1 - idx];
                        }
                        // 设置 alpha 值
                        float opacity = Math.Abs(val);
                        bmpNew = new Bitmap(bmp.Width, bmp.Height);
                        using (var g = Graphics.FromImage(bmpNew)) {
                            var matrix = new ColorMatrix { Matrix33 = opacity };
                            var attributes = new ImageAttributes();
                            attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                            var rectangle = new Rectangle(0, 0, bmpNew.Width, bmpNew.Height);
                            g.DrawImage(bmp, rectangle, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                        }
                    }
                    valScale += deltaScale;
                    // 余数
                    float mod = valScale -
                        (float)Math.Floor(valScale / twiceIntervalScale) * twiceIntervalScale;
                    float scale = StartScale + intervalScale - Math.Abs(intervalScale - mod);
                    Size sizeNew = new Size(
                            (int)(size.Width * scale), (int)(size.Height * scale));
                    Point posNew = new Point(
                            pos.X - (sizeNew.Width - size.Width) / 2,
                            pos.Y - (sizeNew.Height - size.Height) / 2);
                    MainForm.BeginInvoke(new Action(() => {
                        if (CanDoAnimation) {
                            LableControlled.BackgroundImage = bmpNew;
                        }
                        LableControlled.Size = sizeNew;
                        LableControlled.Location = posNew;
                    }));
                    // 运行时也能取消, 单单使用 Cancel 无法取消
                    if (token.IsCancellationRequested) {
                        // 里面存储的 m_state 变量是 volitile 的
                        return;
                    }
                    Thread.Sleep(SLEEP_TIME);
                }
            }, token.Token);
        }
    }
}
