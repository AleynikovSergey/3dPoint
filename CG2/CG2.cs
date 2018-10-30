using System;
using System.Drawing;
using System.Windows.Forms;
using MatrixLibrary;

namespace CG2
{
    public partial class CG2 : Form
    {
        private Input _input = new Input();
        private Calc _calc = new Calc();
        private Draw _draw = new Draw();
        bool proj = true;

        public CG2()
        {
            InitializeComponent();
        }

        private void refresh_label()
        {
            labelCurrentX.Text = trackBarX.Value.ToString();
            labelCurrentY.Text = trackBarY.Value.ToString();
            labelCurrentZ.Text = trackBarZ.Value.ToString();

            label12.Text = trackBar3.Value.ToString();
            label11.Text = trackBar2.Value.ToString();
            label10.Text = trackBar1.Value.ToString();
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            
            mainFunc();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CG2_Load(object sender, EventArgs e)
        {
           
            _calc.calc_X0Y0(Viev3D.Width, Viev3D.Height);
            mainFunc();
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                proj = true;
            else if (radioButton2.Checked)
                proj = false;
            
            mainFunc();
        }

        private void show_error(string error)
        {
            Bitmap bitmap = new Bitmap(Viev3D.Width, Viev3D.Height); ;
            Graphics graphics = Graphics.FromImage(bitmap);
            SolidBrush TextBrush = new SolidBrush(Color.Black);
            Font TextFont = new Font("Times New Roman", 14);

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.DrawString(error, TextFont, TextBrush, Viev3D.Width / 4, Viev3D.Height / 2);
            Viev3D.Image = bitmap;
        }

        private bool check_distance(Input inp)//проверка cos
        {
            double cosa = ((inp.xt - inp.xc) * (-inp.xc) + (inp.yt - inp.yc) * (-inp.yc) + (inp.zt - inp.zc) * (-inp.zc)) / (Math.Sqrt(inp.xc * inp.xc + inp.yc * inp.yc + inp.zc * inp.zc) * Math.Sqrt((inp.xt - inp.xc) * (inp.xt - inp.xc) + (inp.yt - inp.yc) * (inp.yt - inp.yc) + (inp.zt - inp.zc) * (inp.zt - inp.zc)));
            if (cosa < 0 && proj == false)
                return false;
            else
                return true;
        }

        private bool check_camera(Input inp)
        {
            if (inp.xc == 0 && inp.yc == 0 && inp.zc == 0)
                return false;
            else
                return true;
        }

        private bool check_borders(Calc calc)//выходит ли точка за пределы экрана?
        {
            if ((calc._pointsO[7].X > Viev3D.Width || calc._pointsO[7].X < 0) || (calc._pointsO[7].Y > Viev3D.Height || calc._pointsO[7].Y < 0) || (calc._pointsO[11].X > Viev3D.Width || calc._pointsO[11].X < 0) || (calc._pointsO[11].Y > Viev3D.Height || calc._pointsO[11].Y < 0) || (calc._pointsO[12].X > Viev3D.Width || calc._pointsO[12].X < 0) || (calc._pointsO[12].Y > Viev3D.Height || calc._pointsO[12].Y < 0) || (calc._pointsO[13].X > Viev3D.Width || calc._pointsO[13].X < 0) || (calc._pointsO[13].Y > Viev3D.Height || calc._pointsO[13].Y < 0))
                return false;
            else
                return true;
        }

        private bool check_points(Input inp)
        {
            if (inp.xc == inp.xt && inp.yc == inp.yt && inp.zc == inp.zt && proj == false)
                return false;
            else
                return true;
        }

        void mainFunc()
        {
            refresh_label();
            _input.InpPoints(trackBarX.Value, trackBarY.Value, trackBarZ.Value, trackBar3.Value, trackBar2.Value, trackBar1.Value, Viev3D.Width);

            _calc.transf(_input.xt, _input.yt, _input.zt, _input.xc, _input.yc, _input.zc);
            if (proj == true)
                _calc.calc_Orthog(_input._points3D);
            else if (proj == false)
                _calc.calc_Central(_input._points3D);
            _calc.calc_C(_input._points3D, _input._points3C);

            bool flag = true;
            if (check_points(_input))
            {
                flag = check_distance(_input);
                if (check_distance(_input))
                {
                    if (check_borders(_calc) && flag)
                    {
                        if (check_camera(_input))
                            Viev3D.Image = _draw.draw_O(ComplexViev.Width, ComplexViev.Height, _calc._pointsO);
                        else
                            show_error("Камера находится\n в начале координат");
                    }

                    else
                        show_error("Точка за пределами экрана");
                }
                else
                    show_error("Чертеж не может быть построен");
            }
            else
                show_error("Чертеж не может быть построен");

            ComplexViev.Image = _draw.draw_C(ComplexViev.Width, ComplexViev.Height, _calc._pointsC);
        }
    }

    public class Draw
    {
        Bitmap _bitmap;
        Graphics _graphics;

        public Bitmap draw_O(int width, int height, PointF[] points)
        {
            _bitmap = new Bitmap(width, height);
            _graphics = Graphics.FromImage(_bitmap);
            _graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Pen PenBlack = new Pen(Brushes.Black);
            Pen PenBlue = new Pen(Brushes.Blue);
            Pen PenGrey = new Pen(Brushes.Gray);
            SolidBrush TextBrush = new SolidBrush(Color.Black);
            Font TextFont = new Font("Times New Roman", 8);
            int margin1 = 5, margin2 = 2;

            PenBlack.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            _graphics.DrawLine(PenBlack, points[0], points[1]);
            _graphics.DrawLine(PenBlack, points[0], points[3]);
            _graphics.DrawLine(PenBlack, points[0], points[5]);
            PenGrey.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            _graphics.DrawLine(PenGrey, points[8], points[0]);
            _graphics.DrawLine(PenGrey, points[9], points[0]);
            _graphics.DrawLine(PenGrey, points[10], points[0]);
            PenGrey.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _graphics.DrawLine(PenGrey, points[13], points[9]);
            _graphics.DrawLine(PenGrey, points[13], points[10]);
            _graphics.DrawLine(PenGrey, points[12], points[8]);
            _graphics.DrawLine(PenGrey, points[12], points[10]);
            _graphics.DrawLine(PenGrey, points[11], points[8]);
            _graphics.DrawLine(PenGrey, points[11], points[9]);
            _graphics.DrawLine(PenBlue, points[7], points[13]);
            _graphics.DrawLine(PenBlue, points[7], points[12]);
            _graphics.DrawLine(PenBlue, points[7], points[11]);

            _graphics.FillEllipse(TextBrush, points[11].X - margin2, points[11].Y - margin2, 4, 4);
            _graphics.FillEllipse(TextBrush, points[12].X - margin2, points[12].Y - margin2, 4, 4);
            _graphics.FillEllipse(TextBrush, points[13].X - margin2, points[13].Y - margin2, 4, 4);
            _graphics.FillEllipse(TextBrush, points[8].X - margin2, points[8].Y - margin2, 4, 4);
            _graphics.FillEllipse(TextBrush, points[9].X - margin2, points[9].Y - margin2, 4, 4);
            _graphics.FillEllipse(TextBrush, points[10].X - margin2, points[10].Y - margin2, 4, 4);
            _graphics.FillEllipse(TextBrush, points[0].X - margin2, points[0].Y - margin2, 4, 4);
            _graphics.FillEllipse(Brushes.Red, points[7].X - margin2, points[7].Y - margin2, 5, 5);

            _graphics.DrawString("X", TextFont, TextBrush, points[1].X + margin1, points[1].Y);
            _graphics.DrawString("Y", TextFont, TextBrush, points[3].X, points[3].Y - margin2);
            _graphics.DrawString("Z", TextFont, TextBrush, points[5].X + margin1, points[5].Y);
            _graphics.DrawString("O", TextFont, TextBrush, points[0].X + margin1, points[0].Y);
            _graphics.DrawString("T", TextFont, TextBrush, points[7].X + margin1, points[7].Y - margin1 * 3);
            _graphics.DrawString("T1", TextFont, TextBrush, points[11].X + margin1, points[11].Y - margin1 * 3);
            _graphics.DrawString("T2", TextFont, TextBrush, points[12].X + margin1, points[12].Y - margin1 * 3);
            _graphics.DrawString("T3", TextFont, TextBrush, points[13].X + margin1, points[13].Y - margin1 * 3);

            return _bitmap;
        }

        public Bitmap draw_C(int width, int height, PointF[] points)
        {
            _bitmap = new Bitmap(width, height);
            _graphics = Graphics.FromImage(_bitmap);
            _graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Pen PenBlack = new Pen(Brushes.Black);
            Pen PenBlue = new Pen(Brushes.Blue);
            Pen PenGrey = new Pen(Brushes.Gray);
            SolidBrush TextBrush = new SolidBrush(Color.Black);
            Font TextFont = new Font("Times New Roman", 8);
            int margin1 = 5;
            int r = 4;

            _graphics.DrawString("X", TextFont, TextBrush, margin1, points[1].Y + margin1);
            _graphics.DrawString("Z", TextFont, TextBrush, points[3].X - margin1 * 3, 5);
            _graphics.DrawString("Y", TextFont, TextBrush, width - margin1 * 3, points[5].Y + margin1);
            _graphics.DrawString("-Y", TextFont, TextBrush, points[3].X + margin1, 5);
            _graphics.DrawString("-Y", TextFont, TextBrush, points[4].X + margin1, height - 5 - margin1 * 3);
            _graphics.DrawString("-Z", TextFont, TextBrush, points[5].X + margin1 * 3, height - 5 - margin1 * 3);
            _graphics.DrawString("-Y", TextFont, TextBrush, 0, points[2].Y - margin1 * 3);
            _graphics.DrawString("-X", TextFont, TextBrush, width - margin1 * 3, points[2].Y - margin1 * 3);

            PenBlack.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            _graphics.DrawLine(PenBlack, points[1], points[2]);
            _graphics.DrawLine(PenBlack, points[3], points[4]);

            _graphics.DrawLine(PenGrey, points[10], points[8]);
            _graphics.DrawLine(PenGrey, points[8], points[11]);

            _graphics.DrawLine(PenGrey, points[6], points[11]);
            _graphics.DrawLine(PenGrey, points[10], points[5]);

            _graphics.DrawLine(PenGrey, points[9], points[5]);
            _graphics.DrawLine(PenGrey, points[9], points[7]);

            _graphics.DrawLine(PenBlue, points[17], points[15]);
            _graphics.DrawLine(PenBlue, points[15], points[18]);

            _graphics.DrawLine(PenBlue, points[13], points[18]);
            _graphics.DrawLine(PenBlue, points[17], points[12]);

            _graphics.DrawLine(PenBlue, points[16], points[12]);
            _graphics.DrawLine(PenBlue, points[16], points[14]);

            _graphics.DrawString("C1", TextFont, TextBrush, points[16].X, points[16].Y + margin1);
            _graphics.DrawString("C2", TextFont, TextBrush, points[17].X, points[17].Y - margin1 * 4);
            _graphics.DrawString("C3", TextFont, TextBrush, points[18].X, points[18].Y - margin1 * 4);

            _graphics.DrawString("T1", TextFont, TextBrush, points[9].X + margin1, points[9].Y - margin1 * 4);
            _graphics.DrawString("T2", TextFont, TextBrush, points[10].X + margin1, points[10].Y + margin1);
            _graphics.DrawString("T3", TextFont, TextBrush, points[11].X - margin1 * 4, points[11].Y + margin1);

            _graphics.FillEllipse(TextBrush, points[16].X - r / 2, points[16].Y - r / 2, r, r);
            _graphics.FillEllipse(TextBrush, points[17].X - r / 2, points[17].Y - r / 2, r, r);
            _graphics.FillEllipse(TextBrush, points[18].X - r / 2, points[18].Y - r / 2, r, r);

            _graphics.FillEllipse(TextBrush, points[9].X - r / 2, points[9].Y - r / 2, r, r);
            _graphics.FillEllipse(TextBrush, points[10].X - r / 2, points[10].Y - r / 2, r, r);
            _graphics.FillEllipse(TextBrush, points[11].X - r / 2, points[11].Y - r / 2, r, r);

            if (points[13].X - width / 2 == 0)
                width -= 2;

            if ((points[6].X - width / 2) < 0)
                _graphics.DrawArc(PenGrey, points[6].X, points[6].X + 16, -2 * (points[6].X - width / 2), -2 * (points[6].X - width / 2), 180, 90);
            else if ((points[6].X - width / 2) > 0)
                _graphics.DrawArc(PenGrey, width - points[6].X, height - points[6].X - 18, 2 * (points[6].X - width / 2), 2 * (points[6].X - width / 2), 0, 90);
            if ((points[13].X - width / 2) < 0)
                _graphics.DrawArc(PenGrey, points[13].X, points[13].X + 16, -2 * (points[13].X - width / 2), -2 * (points[13].X - width / 2), 180, 90);
            else if ((points[13].X - width / 2) > 0)
                _graphics.DrawArc(PenBlue, width - points[13].X, height - points[13].X - 18, 2 * (points[13].X - width / 2), 2 * (points[13].X - width / 2), 0, 90);
            
            return _bitmap;
        }
    }

    public class Calc
    {
        int width, height;
        public double sinA, cosA, sinB, cosB, d, length;
        public float x0, y0;
        public float xt, yt, zt;
        public float xc, yc, zc;

        Matrix Rz = new Matrix(4, 4);
        Matrix Rx = new Matrix(4, 4);
        Matrix Pxy = new Matrix(4, 4);
        Matrix pT = new Matrix(4, 4);
        Matrix M = new Matrix(4, 4);
        Matrix P = new Matrix(4, 4);
        Matrix A = new Matrix(4, 4);

        Matrix Ox = new Matrix(1, 4);
        Matrix Oy = new Matrix(1, 4);
        Matrix Oz = new Matrix(1, 4);

        Matrix tT = new Matrix(1, 4);
        Matrix tT1 = new Matrix(1, 4);
        Matrix tT2 = new Matrix(1, 4);
        Matrix tT3 = new Matrix(1, 4);
        Matrix tTx = new Matrix(1, 4);
        Matrix tTy = new Matrix(1, 4);
        Matrix tTz = new Matrix(1, 4);

        static int countO = 14, countC = 19;

        public PointF[] _pointsO = new PointF[countO], _pointsC = new PointF[countC];

        public PointF O = new PointF();

        public Calc()
        {
            for (int i = 0; i < countO; ++i)
                _pointsO[i] = new PointF();

            for (int i = 0; i < countC; ++i)
                _pointsC[i] = new PointF();
        }

        public void transf(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            xt = x1;
            yt = y1;
            zt = z1;
            xc = x2;
            yc = y2;
            zc = z2;
        }

        public void calc_X0Y0(int lwidth, int lheight)
        {
            width = lwidth;
            height = lheight;

            x0 = lwidth / 2;
            y0 = lheight / 2;
            O.X = x0;
            O.Y = y0;
            length = lwidth / 2 - 10;
        }

        public void calc()
        {
            //calc_Orthog();
            //calc_Central();
            //calc_C();
        }

        public void calc_Orthog(Point4d[] points3d)
        {
            calc_OA();
            calc_XYZ(points3d);
            calc_tT(points3d);

            _pointsO[0] = O;
            _pointsO[1].X = (float)Ox[0, 0]; _pointsO[1].Y = (float)Ox[0, 1];
            _pointsO[2].X = -(float)Ox[0, 0]; _pointsO[2].Y = -(float)Ox[0, 1];
            _pointsO[3].X = (float)Oy[0, 0]; _pointsO[3].Y = (float)Oy[0, 1];
            _pointsO[4].X = -(float)Oy[0, 0]; _pointsO[4].Y = -(float)Oy[0, 1];
            _pointsO[5].X = (float)Oz[0, 0]; _pointsO[5].Y = (float)Oz[0, 1];
            _pointsO[6].X = -(float)Oz[0, 0]; _pointsO[6].Y = -(float)Oz[0, 1];

            _pointsO[7].X = (float)tT[0, 0]; _pointsO[7].Y = (float)tT[0, 1];
            _pointsO[8].X = (float)tTx[0, 0]; _pointsO[8].Y = (float)tTx[0, 1];
            _pointsO[9].X = (float)tTy[0, 0]; _pointsO[9].Y = (float)tTy[0, 1];
            _pointsO[10].X = (float)tTz[0, 0]; _pointsO[10].Y = (float)tTz[0, 1];
            _pointsO[11].X = (float)tT1[0, 0]; _pointsO[11].Y = (float)tT1[0, 1];
            _pointsO[12].X = (float)tT2[0, 0]; _pointsO[12].Y = (float)tT2[0, 1];
            _pointsO[13].X = (float)tT3[0, 0]; _pointsO[13].Y = (float)tT3[0, 1];
        }

        public void calc_Central(Point4d[] points3d)
        {
            calc_CA(); //матрица А
            calc_CXYZ(points3d); //начало координат
            calc_tT(points3d);

            _pointsO[0] = O;
            _pointsO[1].X = (float)(Ox[0, 0] / Ox[0, 3]); _pointsO[1].Y = (float)(Ox[0, 1] / Ox[0, 3]);
            _pointsO[2].X = -(float)Ox[0, 0]; _pointsO[2].Y = -(float)Ox[0, 1];
            _pointsO[3].X = (float)(Oy[0, 0] / Oy[0, 3]); _pointsO[3].Y = (float)(Oy[0, 1] / Oy[0, 3]);
            _pointsO[4].X = -(float)Oy[0, 0]; _pointsO[4].Y = -(float)Oy[0, 1];
            _pointsO[5].X = (float)(Oz[0, 0] / Oz[0, 3]); _pointsO[5].Y = (float)(Oz[0, 1] / Oz[0, 3]);
            _pointsO[6].X = -(float)Oz[0, 0]; _pointsO[6].Y = -(float)Oz[0, 1];

            _pointsO[7].X = (float)(tT[0, 0] / tT[0, 3]); _pointsO[7].Y = (float)(tT[0, 1] / tT[0, 3]);
            _pointsO[8].X = (float)(tTx[0, 0] / tTx[0, 3]); _pointsO[8].Y = (float)(tTx[0, 1] / tTx[0, 3]);
            _pointsO[9].X = (float)(tTy[0, 0] / tTy[0, 3]); _pointsO[9].Y = (float)(tTy[0, 1] / tTy[0, 3]);
            _pointsO[10].X = (float)(tTz[0, 0] / tTz[0, 3]); _pointsO[10].Y = (float)(tTz[0, 1] / tTz[0, 3]);
            _pointsO[11].X = (float)(tT1[0, 0] / tT1[0, 3]); _pointsO[11].Y = (float)(tT1[0, 1] / tT1[0, 3]);
            _pointsO[12].X = (float)(tT2[0, 0] / tT2[0, 3]); _pointsO[12].Y = (float)(tT2[0, 1] / tT2[0, 3]);
            _pointsO[13].X = (float)(tT3[0, 0] / tT3[0, 3]); _pointsO[13].Y = (float)(tT3[0, 1] / tT3[0, 3]);
        }

        public void calc_Rz()
        {
            if (xc == 0 && yc == 0)
            {
                sinA = 1;
                cosA = 0;
            }
            else
            {
                cosA = yc / (float)(Math.Sqrt(xc * xc + yc * yc));
                sinA = xc / (float)(Math.Sqrt(xc * xc + yc * yc));
            }
            Rz[0, 0] = cosA; Rz[0, 1] = sinA; Rz[0, 2] = 0; Rz[0, 3] = 0;
            Rz[1, 0] = -sinA; Rz[1, 1] = cosA; Rz[1, 2] = 0; Rz[1, 3] = 0;
            Rz[2, 0] = 0; Rz[2, 1] = 0; Rz[2, 2] = 1; Rz[2, 3] = 0;
            Rz[3, 0] = 0; Rz[3, 1] = 0; Rz[3, 2] = 0; Rz[3, 3] = 1;
        }

        public void calc_Rx()
        {
            cosB = zc / (float)(Math.Sqrt(xc * xc + yc * yc + zc * zc));
            sinB = (float)(Math.Sqrt(xc * xc + yc * yc)) / (float)(Math.Sqrt(xc * xc + yc * yc + zc * zc));

            Rx[0, 0] = 1; Rx[0, 1] = 0; Rx[0, 2] = 0; Rx[0, 3] = 0;
            Rx[1, 0] = 0; Rx[1, 1] = cosB; Rx[1, 2] = sinB; Rx[1, 3] = 0;
            Rx[2, 0] = 0; Rx[2, 1] = -sinB; Rx[2, 2] = cosB; Rx[2, 3] = 0;
            Rx[3, 0] = 0; Rx[3, 1] = 0; Rx[3, 2] = 0; Rx[3, 3] = 1;
        }

        public void calc_M()
        {
            M[0, 0] = -1; M[0, 1] = 0; M[0, 2] = 0; M[0, 3] = 0;

            M[1, 0] = 0; M[1, 1] = 1; M[1, 2] = 0; M[1, 3] = 0;

            M[2, 0] = 0; M[2, 1] = 0; M[2, 2] = 1; M[2, 3] = 0;

            M[3, 0] = 0; M[3, 1] = 0; M[3, 2] = 0; M[3, 3] = 1;
        }

        public void calc_Pxy()
        {
            Pxy[0, 0] = 1; Pxy[0, 1] = 0; Pxy[0, 2] = 0; Pxy[0, 3] = 0;

            Pxy[1, 0] = 0; Pxy[1, 1] = 1; Pxy[1, 2] = 0; Pxy[1, 3] = 0;

            Pxy[2, 0] = 0; Pxy[2, 1] = 0; Pxy[2, 2] = 0; Pxy[2, 3] = 0;

            Pxy[3, 0] = 0; Pxy[3, 1] = 0; Pxy[3, 2] = 0; Pxy[3, 3] = 1;
        }

        public void calc_T()
        {
            pT[0, 0] = 1; pT[0, 1] = 0; pT[0, 2] = 0; pT[0, 3] = 0;
            pT[1, 0] = 0; pT[1, 1] = 1; pT[1, 2] = 0; pT[1, 3] = 0;
            pT[2, 0] = 0; pT[2, 1] = 0; pT[2, 2] = 1; pT[2, 3] = 0;
            pT[3, 0] = x0; pT[3, 1] = y0; pT[3, 2] = 0; pT[3, 3] = 1;
        }

        public void calc_P()
        {
            d = Math.Sqrt(xc * xc + yc * yc + zc * zc);
            P[0, 0] = 1; P[0, 1] = 0; P[0, 2] = 0; P[0, 3] = 0;

            P[1, 0] = 0; P[1, 1] = 1; P[1, 2] = 0; P[1, 3] = 0;

            P[2, 0] = 0; P[2, 1] = 0; P[2, 2] = 1; P[2, 3] = -1 / d;

            P[3, 0] = 0; P[3, 1] = 0; P[3, 2] = 0; P[3, 3] = 1;
        }

        public void calc_XYZ(Point4d[] points3d)
        {
            length = width / 2 - 10;

            Ox[0, 0] = points3d[1].X; Ox[0, 1] = 0; Ox[0, 2] = 0; Ox[0, 3] = 1;

            Oy[0, 0] = 0; Oy[0, 1] = points3d[3].Y; Oy[0, 2] = 0; Oy[0, 3] = 1;

            Oz[0, 0] = 0; Oz[0, 1] = 0; Oz[0, 2] = points3d[5].Z; Oz[0, 3] = 1;

            Ox *= A;
            Oy *= A;
            Oz *= A;
        }

        public void calc_CXYZ(Point4d[] points3d)
        {
            length = 15;
            if (points3d[7].X > 0)
            Ox[0, 0] = points3d[7].X + length;
            else
                Ox[0, 0] = points3d[7].X - length;
            Ox[0, 1] = 0; Ox[0, 2] = 0; Ox[0, 3] = 1;

            Oy[0, 0] = 0;
            if (points3d[7].Y > 0)
                Oy[0, 1] = points3d[7].Y + length; 
            else 
                Oy[0, 1] = points3d[7].Y - length;
            Oy[0, 2] = 0;
            Oy[0, 3] = 1;

            Oz[0, 0] = 0; Oz[0, 1] = 0;
            if (points3d[7].Z > 0)
                Oz[0, 2] = points3d[7].Z + length;
            else
                Oz[0, 2] = points3d[7].Z - length;
            Oz[0, 3] = 1;

            Ox *= A;
            Oy *= A;
            Oz *= A;
        }

        public void calc_tT(Point4d[] points3d)
        {
            tT[0, 0] = points3d[7].X; tT[0, 1] = points3d[7].Y; tT[0, 2] = points3d[7].Z; tT[0, 3] = 1;

            tTx[0, 0] = points3d[8].X; tTx[0, 1] = 0; tTx[0, 2] = 0; tTx[0, 3] = 1;

            tTy[0, 0] = 0; tTy[0, 1] = points3d[9].Y; tTy[0, 2] = 0; tTy[0, 3] = 1;

            tTz[0, 0] = 0; tTz[0, 1] = 0; tTz[0, 2] = points3d[10].Z; tTz[0, 3] = 1;

            tT1[0, 0] = points3d[11].X; tT1[0, 1] = points3d[11].Y; tT1[0, 2] = 0; tT1[0, 3] = 1;

            tT2[0, 0] = points3d[12].X; tT2[0, 1] = 0; tT2[0, 2] = points3d[12].Z; tT2[0, 3] = 1;

            tT3[0, 0] = 0; tT3[0, 1] = points3d[13].Y; tT3[0, 2] = points3d[13].Z; tT3[0, 3] = 1;            

            tT *= A;
            tTx *= A;
            tTy *= A;
            tTz *= A;
            tT1 *= A;
            tT2 *= A;
            tT3 *= A;            
        }

        public void calc_OA()//A для ортогонального
        {
            calc_Rz();
            calc_Rx();
            calc_M();
            calc_Pxy();
            calc_T();
            A = Rz * Rx * M * Pxy * pT;
        }

        public void calc_CA()//A для центрального
        {
            calc_Rz();
            calc_Rx();
            calc_M();
            calc_P();
            calc_Pxy();
            calc_T();
            A = Rz * Rx * M * P * Pxy * pT;
        }

        public void calc_C(Point4d[] points3d, Point4d[] points3c)//вычисления комплексного чертежа
        {
            _pointsC[0] = Convert1(points3d[0]); //начало координат
            _pointsC[1] = Convert1(points3d[1]); //начало оси Ox
            _pointsC[2] = Convert1(points3d[2]); //конец оси Ox
            _pointsC[3] = Convert1(points3d[3]); //начало оси Oy
            _pointsC[4] = Convert1(points3d[4]); //конец оси Oy
            _pointsC[5] = Convert1(points3d[8]); //точка Tx
            _pointsC[6] = Convert2(points3d[9]); //точка Ty
            _pointsC[7] = Convert1(points3d[9]); //точка Ty
            _pointsC[8] = Convert2(points3d[10]); //точка Tz
            _pointsC[9] = Convert1(points3d[11]); //точка T1
            _pointsC[10] = Convert3(points3d[12]); //точка T2
            _pointsC[11] = Convert2(points3d[13]); //точка T3

            _pointsC[12] = Convert1(points3c[8]); //точка Tx
            _pointsC[13] = Convert2(points3c[9]); //точка Ty
            _pointsC[14] = Convert1(points3c[9]); //точка Ty
            _pointsC[15] = Convert2(points3c[10]); //точка Tz
            _pointsC[16] = Convert1(points3c[11]); //точка T1
            _pointsC[17] = Convert3(points3c[12]); //точка T2
            _pointsC[18] = Convert2(points3c[13]); //точка T3
        }

        private Point Convert1(Point4d p)
        {
            return new Point((int)(-p.X + x0), (int)(p.Y + y0));
        }

        private Point Convert2(Point4d p)
        {
            return new Point((int)(p.Y + x0), (int)(-p.Z + y0));
        }

        private Point Convert3(Point4d p)
        {
            return new Point((int)(-p.X + x0), (int)(-p.Z + y0));
        }
    }

    public class Input
    {
        public float xt, yt, zt;
        public float xc, yc, zc;
        static int count = 14;
        public Point4d[] _points3D = new Point4d[count];
        public Point4d[] _points3C = new Point4d[count];

        public void Inp(int x1, int y1, int z1, int x2, int y2, int z2)
        {
            xt = x1;
            yt = y1;
            zt = z1;
            xc = x2;
            yc = y2;
            zc = z2;
        }

        public Input()
        {
            for (int i = 0; i < count; ++i)
                _points3D[i] = new Point4d();
        }

        public void InpPoints(int x1, int y1, int z1, int x2, int y2, int z2, int width)
        {
            xt = x1;
            yt = y1;
            zt = z1;
            xc = x2;
            yc = y2;
            zc = z2;

            double length = width / 2 - 10;
            _points3D[0] = new Point4d(0, 0, 0); //начало кординат
            _points3D[1] = new Point4d(length, 0, 0); //конец оси Х
            _points3D[2] = new Point4d(-length, 0, 0); //начало оси Х
            _points3D[3] = new Point4d(0, length, 0); //конец оси Y
            _points3D[4] = new Point4d(0, -length, 0); //начало оси Y
            _points3D[5] = new Point4d(0, 0, length); //конец оси Z
            _points3D[6] = new Point4d(0, 0, -length); //начало оси Z
            _points3D[7] = new Point4d(x1, y1, z1);
            _points3D[8] = new Point4d(x1, 0, 0); //проекция точки на ось X
            _points3D[9] = new Point4d(0, y1, 0); //проекция точки на ось Y
            _points3D[10] = new Point4d(0, 0, z1); //проекция точки на ось Z
            _points3D[11] = new Point4d(x1, y1, 0); // T1
            _points3D[12] = new Point4d(x1, 0, z1); // T2
            _points3D[13] = new Point4d(0, y1, z1); // T3

            _points3C[7] = new Point4d(x2, y2, z2);
            _points3C[8] = new Point4d(x2, 0, 0); //проекция точки на ось X
            _points3C[9] = new Point4d(0, y2, 0); //проекция точки на ось Y
            _points3C[10] = new Point4d(0, 0, z2); //проекция точки на ось Z
            _points3C[11] = new Point4d(x2, y2, 0); // T1
            _points3C[12] = new Point4d(x2, 0, z2); // T2
            _points3C[13] = new Point4d(0, y2, z2); // T3
        }
    }

    public class Point4d
    {
        public double X, Y, Z;

        public Point4d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point4d()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }
    }
}
