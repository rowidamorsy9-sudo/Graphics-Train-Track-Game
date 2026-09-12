using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace game
{
    public class Circle
    {
        public int Rad;
        public int XC;
        public int YC;
        public float thRadian;
        public float st, end;
        public void Drawcircle(Graphics g)
        {
            for (float i = st; i <= end; i += 1.0f)
            {
                thRadian = (float)((i * Math.PI) / 180);
                float x = (float)(Rad * Math.Cos(thRadian));
                float y = (float)(Rad * Math.Sin(thRadian));

                x += XC;
                y += YC;

                g.FillEllipse(Brushes.Black, x, y, 5, 5);
            }
            PointF tempst = Getnextpoint((int)st);
            PointF tempend = Getnextpoint((int)end);
            g.DrawLine(Pens.Black, XC, YC, tempst.X, tempst.Y);
            g.DrawLine(Pens.Black, XC, YC, tempend.X, tempend.Y);
        }
        public PointF Getnextpoint(int theta)
        {
            PointF p = new PointF();
            thRadian = (float)(theta * Math.PI / 180);
            p.X = (float)(Rad * Math.Cos(thRadian)) + XC;
            p.Y = (float)(Rad * Math.Sin(thRadian)) + YC;
            return p;
        }
    }

    public class DDA
    {
        public float Xst, Yst;
        public float Xend, Yend;
        float dy, dx, m;
        public float cx, cy;
        int speed = 4;
        public void calc()
        {
            dy = Yend - this.Yst;
            dx = Xend - Xst;
            m = dy / dx;
            cx = Xst;
            cy = Yst;
        }
        public bool CalcNextPoint()
        {
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                if (Xst < Xend)
                {
                    cx += speed;
                    cy += m * speed;
                    if (cx >= Xend) return false;
                }
                else
                {
                    cx -= speed;
                    cy -= m * speed;
                    if (cx <= Xend) return false;
                }
            }
            else
            {
                if (Yst < Yend)
                {
                    cy += speed;
                    cx += 1 / m * speed;
                    if (cy >= Yend) return false;
                }
                else
                {
                    cy -= speed;
                    cx -= 1 / m * speed;
                    if (cy <= Yend) return false;
                }
            }
            return true;
        }
    }

    public class BezierCurve
    {
        public List<Point> ControlPoints;
        public float t_inc = 0.001f;
        public Color cl = Color.Red;
        public Color clr1 = Color.Blue;
        public Color ftColor = Color.Black;

        public BezierCurve()
        {
            ControlPoints = new List<Point>();
        }

        private float Factorial(int n)
        {
            float res = 1.0f;
            for (int i = 2; i <= n; i++)
                res *= i;
            return res;
        }

        private float C(int n, int i)
        {
            float res = Factorial(n) / (Factorial(i) * Factorial(n - i));
            return res;
        }

        private double Calc_B(float t, int i)
        {
            int n = ControlPoints.Count - 1;
            double res = C(n, i) *
                            Math.Pow((1 - t), (n - i)) *
                            Math.Pow(t, i);
            return res;
        }

        public Point GetPoint(int i)
        {
            return ControlPoints[i];
        }

        public PointF CalcCurvePointAtTime(float t)
        {
            PointF pt = new PointF();
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                float B = (float)Calc_B(t, i);
                pt.X += B * ControlPoints[i].X;
                pt.Y += B * ControlPoints[i].Y;
            }
            return pt;
        }

        public int isCtrlPoint(int XMouse, int YMouse)
        {
            Rectangle rc;
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                rc = new Rectangle(ControlPoints[i].X - 5, ControlPoints[i].Y - 5, 10, 10);
                if (XMouse >= rc.Left && XMouse <= rc.Right && YMouse >= rc.Top && YMouse <= rc.Bottom)
                    return i;
            }
            return -1;
        }

        public void ModifyCtrlPoint(int i, int XMouse, int YMouse)
        {
            Point p = ControlPoints[i];
            p.X = XMouse;
            p.Y = YMouse;
            ControlPoints[i] = p;
        }

        public void SetControlPoint(Point pt)
        {
            ControlPoints.Add(pt);
        }

        private void DrawCurvePoints(Graphics g)
        {
            if (ControlPoints.Count < 2)
                return;

            float railOffset = 13f;
            float tieSpacing = 20f;

            Pen railPen = new Pen(Color.FromArgb(40, 40, 40), 10);
            Pen tiePen = new Pen(Color.FromArgb(40, 40, 40), 10);

            PointF prevOuter = new PointF();
            PointF prevInner = new PointF();

            bool first = true;
            float distCounter = 0;

            PointF prevCurvePoint = CalcCurvePointAtTime(0);

            for (float t = 0.0f; t <= 1.0f; t += t_inc)
            {
                PointF curr = CalcCurvePointAtTime(t);

                float dx = curr.X - prevCurvePoint.X;
                float dy = curr.Y - prevCurvePoint.Y;
                float len = (float)Math.Sqrt(dx * dx + dy * dy);

                if (len == 0) continue;

                float px = -dy / len;
                float py = dx / len;

                PointF outer = new PointF(curr.X + px * railOffset, curr.Y + py * railOffset);
                PointF inner = new PointF(curr.X - px * railOffset, curr.Y - py * railOffset);

                if (!first)
                {
                    g.DrawLine(railPen, prevOuter, outer);
                    g.DrawLine(railPen, prevInner, inner);
                }

                distCounter += len;
                if (distCounter >= tieSpacing)
                {
                    g.DrawLine(tiePen, outer, inner);
                    distCounter = 0;
                }

                prevOuter = outer;
                prevInner = inner;
                prevCurvePoint = curr;
                first = false;
            }
        }

        public void DrawCurve(Graphics g)
        {
            DrawCurvePoints(g);
        }
    }

    class cMultipleimages
    {
        public int x, y, speed = 0;
        public List<Bitmap> images = new List<Bitmap>();
        public int dx = 1, dy = 1;
        public int dir = 1;
        public int F = 1;
    }

    public class LineSegment
    {
        public PointF ptS, ptE;

        public void DrawYourSelf(Graphics g)
        {
            g.DrawLine(Pens.Black, ptS.X, ptS.Y, ptE.X, ptE.Y);
            g.FillEllipse(Brushes.Red, ptS.X - 5, ptS.Y - 5, 10, 10);
            g.FillEllipse(Brushes.Red, ptE.X - 5, ptE.Y - 5, 10, 10);
        }
    }

    public class Transformation
    {
        public LineSegment Rotate(LineSegment L, float xRef, float yRef, float rad)
        {
            L.ptS.X -= xRef;
            L.ptS.Y -= yRef;
            L.ptE.X -= xRef;
            L.ptE.Y -= yRef;

            double xn = L.ptS.X * Math.Cos(rad) - L.ptS.Y * Math.Sin(rad);
            double Yn = L.ptS.X * Math.Sin(rad) + L.ptS.Y * Math.Cos(rad);
            L.ptS.X = (float)xn;
            L.ptS.Y = (float)Yn;

            xn = L.ptE.X * Math.Cos(rad) - L.ptE.Y * Math.Sin(rad);
            Yn = L.ptE.X * Math.Sin(rad) + L.ptE.Y * Math.Cos(rad);
            L.ptE.X = (float)xn;
            L.ptE.Y = (float)Yn;

            L.ptS.X += xRef;
            L.ptS.Y += yRef;
            L.ptE.X += xRef;
            L.ptE.Y += yRef;

            return L;
        }
        public Point RotatePoint(Point pt, Point pivot, float angle)
        {
            float x = pt.X - pivot.X;
            float y = pt.Y - pivot.Y;

            int newX = (int)(x * Math.Cos(angle) - y * Math.Sin(angle)) + pivot.X;
            int newY = (int)(x * Math.Sin(angle) + y * Math.Cos(angle)) + pivot.Y;

            return new Point(newX, newY);
        }
    }

    public partial class Form1 : Form
    {
        Bitmap off;
        List<cMultipleimages> Lhome = new List<cMultipleimages>();

        System.Windows.Forms.Timer tt = new System.Windows.Forms.Timer();
        float xRef;
        float yRef;
        int Yst = 100;
        int Xst = 0;
        int flag = 0, ct = 0;

        LineSegment previewSegment = new LineSegment();
        Transformation trans = new Transformation();
        bool gameStarted = false;
        int score = 0;
        int scorect = 0;

        Rectangle startBtn;
        Rectangle exitBtn;

        List<DDA> ddaLines = new List<DDA>();
        DDA previewDDA = new DDA();
        float ddaLength = 150f;

        float currentX = 0f;
        float currentY = 0f;
        float currentAngle = 0f;

        List<Circle> circleList = new List<Circle>();
        Circle previewCircle = new Circle();
        int circleRadius = 80;
        bool segmentConfirmed = false;

        List<BezierCurve> curveList = new List<BezierCurve>();
        BezierCurve previewCurve = new BezierCurve();
        bool curveDrawing = false;

        int activeSegment = 1;
        List<int> segmentHistory = new List<int>();
        List<int> segmentIndex = new List<int>();
        List<PointF> trackPoints = new List<PointF>();

        //CAR MOVEMENT 
        bool carMoving = false;
        Bitmap carImage;
        int f = 0;
        int draggingIndex = -1;
        int curvect = 0;
        int movecar = 0;
        float current_curve_Angle = 0f;
        int drag = 0;
        int cty=0;

        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;
            this.Load += new EventHandler(Form1_Load);
            this.Paint += new PaintEventHandler(Form1_Paint);
            KeyDown += new KeyEventHandler(Form1_KeyDown);
            tt.Tick += new EventHandler(tt_Tick);
            this.MouseDown += Form1_MouseDown;

            tt.Interval = 10;
            this.KeyPreview = true;
        }



        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && !e.Shift)
            {
                if (activeSegment == 3 && curveDrawing && previewCurve.ControlPoints.Count > 1)
                {
                    Point p = previewCurve.ControlPoints[previewCurve.ControlPoints.Count - 1];
                    p.Y -= 10;
                    p.Y = Math.Max(0, p.Y);  
                    previewCurve.ControlPoints[previewCurve.ControlPoints.Count - 1] = p;
                }
            }
            if (e.KeyCode == Keys.Space && e.Shift)
            {
                if (activeSegment == 3 && curveDrawing && previewCurve.ControlPoints.Count > 1)
                {
                    Point p = previewCurve.ControlPoints[previewCurve.ControlPoints.Count - 1];
                    p.Y += 10;
                    p.Y = Math.Min(this.ClientSize.Height, p.Y); 
                    previewCurve.ControlPoints[previewCurve.ControlPoints.Count - 1] = p;
                }
            }

            if (gameStarted == true)
            {
                if (e.KeyCode == Keys.P)
                {
                    if (carMoving)
                    {
                        carMoving = false;

                    }
                    else
                    {
                    BuildTrackPoints();
                    movecar = 0;
                    carMoving = true;
                    tt.Start();
                    }

                }

                if (e.KeyCode == Keys.Right)
                {
                    for (int i = 0; i < curveList.Count; i++)
                        for (int j = 0; j < curveList[i].ControlPoints.Count; j++)
                        {
                            Point p = curveList[i].ControlPoints[j];
                            p.X += 10;
                            curveList[i].ControlPoints[j] = p;
                        }
                    for (int i = 0; i < circleList.Count; i++) circleList[i].XC += 10;
                    for (int i = 0; i < ddaLines.Count; i++)
                    {
                        ddaLines[i].Xst += 10;
                        ddaLines[i].Xend += 10;
                        ddaLines[i].calc();
                    }
                    for(int i =0; i< trackPoints.Count-1;i++)
                    {

                        trackPoints[i]= new PointF(trackPoints[i].X + 10, trackPoints[i].Y);
                    }
                    setnewstart();
                }

                if (e.KeyCode == Keys.Left)
                {
                    for (int i = 0; i < curveList.Count; i++)
                        for (int j = 0; j < curveList[i].ControlPoints.Count; j++)
                        {
                            Point p = curveList[i].ControlPoints[j];
                            p.X -= 10;
                            curveList[i].ControlPoints[j] = p;
                        }
                    for (int i = 0; i < circleList.Count; i++) circleList[i].XC -= 10;
                    for (int i = 0; i < ddaLines.Count; i++)
                    {
                        ddaLines[i].Xst -= 10;
                        ddaLines[i].Xend -= 10;
                        ddaLines[i].calc();
                    }
                    for (int i = 0; i < trackPoints.Count - 1; i++)
                    {

                        trackPoints[i] = new PointF(trackPoints[i].X - 10, trackPoints[i].Y);
                    }
                    setnewstart();
                }

                if (e.KeyCode == Keys.Up)
                {
                    for (int i = 0; i < curveList.Count; i++)
                        for (int j = 0; j < curveList[i].ControlPoints.Count; j++)
                        {
                            Point p = curveList[i].ControlPoints[j];
                            p.Y -= 10;
                            curveList[i].ControlPoints[j] = p;
                        }
                    for (int i = 0; i < circleList.Count; i++) circleList[i].YC -= 10;
                    for (int i = 0; i < ddaLines.Count; i++)
                    {
                        ddaLines[i].Yst -= 10;
                        ddaLines[i].Yend -= 10;
                        ddaLines[i].calc();
                    }
                    for (int i = 0; i < trackPoints.Count - 1; i++)
                    {

                        trackPoints[i] = new PointF(trackPoints[i].X , trackPoints[i].Y-10);
                    }
                    setnewstart();
                }

                if (e.KeyCode == Keys.Down)
                {
                    for (int i = 0; i < curveList.Count; i++)
                        for (int j = 0; j < curveList[i].ControlPoints.Count; j++)
                        {
                            Point p = curveList[i].ControlPoints[j];
                            p.Y += 10;
                            curveList[i].ControlPoints[j] = p;
                        }
                    for (int i = 0; i < circleList.Count; i++) circleList[i].YC += 10;
                    for (int i = 0; i < ddaLines.Count; i++)
                    {
                        ddaLines[i].Yst += 10;
                        ddaLines[i].Yend += 10;
                        ddaLines[i].calc();
                    }
                    for (int i = 0; i < trackPoints.Count - 1; i++)
                    {

                        trackPoints[i] = new PointF(trackPoints[i].X , trackPoints[i].Y+ 10);
                    }
                    setnewstart();
                }

                if (e.KeyCode == Keys.D1) activeSegment = 1;

                if (e.KeyCode == Keys.D2)
                {
                    activeSegment = 2;
                    activeSegment = 2;
                    segmentConfirmed = false;
                    previewCircle.XC = (int)currentX;
                    previewCircle.YC = (int)currentY - circleRadius;
                    previewCircle.Rad = circleRadius;
                    previewCircle.st = 270;
                    previewCircle.end = 630;
                }

                if (e.KeyCode == Keys.D3)
                {
                    activeSegment = 3;
                    previewCurve = new BezierCurve();
                    previewCurve.SetControlPoint(new Point((int)currentX, (int)currentY));
                    curveDrawing = true;

                }

                if (e.KeyCode == Keys.A && !segmentConfirmed)
                {
                    if (activeSegment == 1)
                    {
                        DDA confirmed = new DDA();
                        segmentHistory.Add(1);
                        segmentIndex.Add(ddaLines.Count - 1);
                        confirmed.Xst = previewDDA.Xst;
                        confirmed.Yst = previewDDA.Yst;
                        confirmed.Xend = previewDDA.Xend;
                        confirmed.Yend = previewDDA.Yend;
                        confirmed.calc();
                        currentAngle = 0f;
                        ddaLines.Add(confirmed);
                        currentX = previewDDA.Xend;
                        currentY = previewDDA.Yend;
                    }

                    if (activeSegment == 2)
                    {
                        Circle confirmed = new Circle();
                        segmentHistory.Add(2);
                        segmentIndex.Add(ddaLines.Count - 1);
                        confirmed.XC = previewCircle.XC;
                        confirmed.YC = previewCircle.YC;
                        confirmed.Rad = previewCircle.Rad;
                        confirmed.st = previewCircle.st;
                        confirmed.end = previewCircle.end;
                        circleList.Add(confirmed);
                        currentX = confirmed.XC;
                        currentY = confirmed.YC + confirmed.Rad;
                    }

                    if (activeSegment == 3)
                    {
                        if (previewCurve.ControlPoints.Count >= 2)
                        {
                            BezierCurve saved = new BezierCurve();
                            for (int i = 0; i < previewCurve.ControlPoints.Count; i++)
                                saved.SetControlPoint(previewCurve.ControlPoints[i]);

                            curveList.Add(saved);
                            segmentHistory.Add(3);
                            segmentIndex.Add(ddaLines.Count - 1);

                            Point last = previewCurve.ControlPoints[previewCurve.ControlPoints.Count - 1];
                            currentX = last.X;
                            currentY = last.Y;
                            curveDrawing = false;
                        }
                        curvect = 0;
                    }

                    previewSegment.ptS.X = currentX;
                    previewSegment.ptS.Y = currentY;
                    previewSegment.ptE.X = currentX + ddaLength;
                    previewSegment.ptE.Y = currentY;

                    previewDDA.Xst = currentX;
                    previewDDA.Yst = currentY;
                    previewDDA.Xend = currentX + ddaLength;
                    previewDDA.Yend = currentY;
                    previewDDA.calc();

                    previewCircle.XC = (int)currentX;
                    previewCircle.YC = (int)currentY - circleRadius;
                    previewCircle.Rad = circleRadius;
                    previewCircle.st = 270;
                    previewCircle.end = 630;
                    BuildTrackPoints();
                }

                if (e.KeyCode == Keys.R && !e.Shift && activeSegment == 1)
                {
                    if (currentAngle < 1.5f)
                    {
                        currentAngle += 0.05f;
                        previewSegment = trans.Rotate(previewSegment, currentX, currentY, 0.05f);
                        segmentConfirmed = false;
                        previewDDA.Xst = previewSegment.ptS.X;
                        previewDDA.Yst = previewSegment.ptS.Y;
                        previewDDA.Xend = previewSegment.ptE.X;
                        previewDDA.Yend = previewSegment.ptE.Y;
                        previewDDA.calc();
                    }
                }

                if (e.KeyCode == Keys.R && e.Shift && activeSegment == 1)
                {
                    if (currentAngle > -1.5f)
                    {
                        currentAngle -= 0.05f;
                        previewSegment = trans.Rotate(previewSegment, currentX, currentY, -0.05f);
                        segmentConfirmed = false;
                        previewDDA.Xst = previewSegment.ptS.X;
                        previewDDA.Yst = previewSegment.ptS.Y;
                        previewDDA.Xend = previewSegment.ptE.X;
                        previewDDA.Yend = previewSegment.ptE.Y;
                        previewDDA.calc();
                    }
                }

                if (e.KeyCode == Keys.L && e.Shift && activeSegment == 1)
                {
                    ddaLength += 10f;
                    if (ddaLength > 200) ddaLength = 200f;
                    float dx = previewSegment.ptE.X - previewSegment.ptS.X;
                    float dy = previewSegment.ptE.Y - previewSegment.ptS.Y;
                    float len = (float)Math.Sqrt(dx * dx + dy * dy);
                    previewSegment.ptE.X = previewSegment.ptS.X + (dx / len) * ddaLength;
                    previewSegment.ptE.Y = previewSegment.ptS.Y + (dy / len) * ddaLength;
                    previewDDA.Xend = previewSegment.ptE.X;
                    previewDDA.Yend = previewSegment.ptE.Y;
                    previewDDA.calc();
                }

                if (e.KeyCode == Keys.L && !e.Shift && activeSegment == 1)
                {
                    ddaLength -= 10f;
                    if (ddaLength < 50f) ddaLength = 50f;
                    float dx = previewSegment.ptE.X - previewSegment.ptS.X;
                    float dy = previewSegment.ptE.Y - previewSegment.ptS.Y;
                    float len = (float)Math.Sqrt(dx * dx + dy * dy);
                    previewSegment.ptE.X = previewSegment.ptS.X + (dx / len) * ddaLength;
                    previewSegment.ptE.Y = previewSegment.ptS.Y + (dy / len) * ddaLength;
                    previewDDA.Xend = previewSegment.ptE.X;
                    previewDDA.Yend = previewSegment.ptE.Y;
                    previewDDA.calc();
                }

                if (e.KeyCode == Keys.O && e.Shift)
                {
                    segmentConfirmed = false;
                    circleRadius += 10;
                    if (circleRadius > 250) circleRadius = 250;
                    previewCircle.Rad = circleRadius;
                    previewCircle.XC = (int)currentX;
                    previewCircle.YC = (int)currentY - circleRadius;
                }

                if (e.KeyCode == Keys.O && !e.Shift)
                {
                    segmentConfirmed = false;
                    circleRadius -= 10;
                    if (circleRadius < 70) circleRadius = 70;
                    previewCircle.Rad = circleRadius;
                    previewCircle.XC = (int)currentX;
                    previewCircle.YC = (int)currentY - circleRadius;
                }
                if (e.KeyCode == Keys.R && !e.Shift && activeSegment == 3)
                {
                    if (current_curve_Angle < 1.5f)
                    {
                        current_curve_Angle += 0.05f;
                        RotateCurve(0.05f);
                    }
                }

                if (e.KeyCode == Keys.R && e.Shift && activeSegment == 3)
                {
                    if (current_curve_Angle > -1.5f)
                    {
                        current_curve_Angle -= 0.05f;
                        RotateCurve(-0.05f);
                    }
                }

                if (e.KeyCode == Keys.D)
                {
                    if (segmentHistory.Count > 0)
                    {
                        int lastType = segmentHistory[segmentHistory.Count - 1];
                        segmentHistory.RemoveAt(segmentHistory.Count - 1);
                        segmentIndex.RemoveAt(segmentIndex.Count - 1);

                        if (lastType == 1 && ddaLines.Count > 0)
                        {
                            DDA last = ddaLines[ddaLines.Count - 1];
                            currentX = last.Xst;
                            currentY = last.Yst;
                            ddaLines.RemoveAt(ddaLines.Count - 1);
                        }
                        else if (lastType == 2 && circleList.Count > 0)
                        {
                            Circle last = circleList[circleList.Count - 1];
                            PointF startPt = last.Getnextpoint((int)last.st);
                            currentX = startPt.X;
                            currentY = startPt.Y;
                            circleList.RemoveAt(circleList.Count - 1);
                        }
                        else if (lastType == 3 && curveList.Count > 0)
                        {
                            BezierCurve last = curveList[curveList.Count - 1];
                            Point first = last.ControlPoints[0];
                            currentX = first.X;
                            currentY = first.Y;
                            curveList.RemoveAt(curveList.Count - 1);
                        }

                        previewSegment.ptS.X = currentX;
                        previewSegment.ptS.Y = currentY;
                        previewSegment.ptE.X = currentX + ddaLength;
                        previewSegment.ptE.Y = currentY;

                        previewDDA.Xst = currentX;
                        previewDDA.Yst = currentY;
                        previewDDA.Xend = currentX + ddaLength;
                        previewDDA.Yend = currentY;
                        previewDDA.calc();

                        previewCircle.XC = (int)currentX;
                        previewCircle.YC = (int)currentY - circleRadius;
                        previewCircle.Rad = circleRadius;
                        previewCircle.st = 270;
                        previewCircle.end = 630;
                        segmentConfirmed = false;
                    }
                }
            }

            DrawDBuff(this.CreateGraphics());
        }

        void Form1_MouseDown(object sender, MouseEventArgs e)
        {

            if (startBtn.Contains(e.Location))
            {
                gameStarted = true;
                f = 1;
                tt.Start();
            }
            if (f == 0)
            {
                if (exitBtn.Contains(e.Location))
                {
                    this.Close();
                }
            }

            if (gameStarted)
            {
                if (activeSegment == 3 && curveDrawing && curvect < 3)
                {
                    previewCurve.SetControlPoint(new Point(e.X, e.Y));
                    curvect++;


                }
            }
        }

        void tt_Tick(object sender, EventArgs e)
        {
            // CAR MOVEMENT: advance one step each tick 
            if (carMoving && trackPoints.Count > 0)
            {
                if (movecar + 1 < trackPoints.Count - 1)
                {
                    movecar += 2;
                    if (scorect % 3 == 0)
                    {
                    score += 1; 
                    }
                    scorect++;
                }
                else
                {
                    carMoving = false;   // reached the end of the track}

                }

            }

            DrawDBuff(this.CreateGraphics());
        }
        void RotateCurve(float rad)
        {
            if (previewCurve.ControlPoints.Count < 2) return;

            Point p = previewCurve.ControlPoints[0]; // = (currentX, currentY), stays fixed

            for (int i = 1; i < previewCurve.ControlPoints.Count; i++)
            {
                previewCurve.ControlPoints[i] = trans.RotatePoint(previewCurve.ControlPoints[i], p, rad);
            }
        }
        void BuildTrackPoints()
        {
            trackPoints.Clear();

            int ddaIndex = 0;
            int circleIndex = 0;
            int curveIndex = 0;

            for (int i = 0; i < segmentHistory.Count; i++)
            {
                if (segmentHistory[i] == 1)
                {
                    DDA d = ddaLines[ddaIndex];
                    d.calc();
                    while (d.CalcNextPoint())
                        trackPoints.Add(new PointF(d.cx, d.cy));
                    ddaIndex++;
                }
                else if (segmentHistory[i] == 2)
                {
                    Circle c = circleList[circleIndex];

                    for (float angle = 90.0f; angle > 0.0f; angle -= 5.0f)
                    {
                        PointF p = c.Getnextpoint((int)angle);
                        trackPoints.Add(p);
                    }
                    for (float angle = 360.0f; angle > 90.0f; angle -= 5.0f)
                    {
                        PointF p = c.Getnextpoint((int)angle);
                        trackPoints.Add(p);
                    }
                    circleIndex++;
                }
                else if (segmentHistory[i] == 3)
                {
                    BezierCurve bc = curveList[curveIndex];
                    for (float t = 0f; t <= 1f; t += 0.05f)
                    {
                        PointF p = bc.CalcCurvePointAtTime(t);
                        trackPoints.Add(p);
                    }
                    curveIndex++;
                }
            }
        }

        float GetCarAngle(int index)
        {
            if (index + 1 >= trackPoints.Count) return 0;
            float dx = trackPoints[index + 1].X - trackPoints[index].X;
            float dy = trackPoints[index + 1].Y - trackPoints[index].Y;
            return (float)(Math.Atan2(dy, dx) * 180 / Math.PI);
        }

        void setnewstart()
        {
            if (segmentHistory.Count > 0)
            {
                int lastType = segmentHistory[segmentHistory.Count - 1];

                if (lastType == 1 && ddaLines.Count > 0)
                {
                    DDA last = ddaLines[ddaLines.Count - 1];
                    currentX = last.Xend;
                    currentY = last.Yend;
                }
                else if (lastType == 2 && circleList.Count > 0)
                {
                    Circle last = circleList[circleList.Count - 1];
                    currentX = last.XC;
                    currentY = last.YC + last.Rad;
                }
                else if (lastType == 3 && curveList.Count > 0)
                {
                    BezierCurve last = curveList[curveList.Count - 1];
                    Point lastPt = last.ControlPoints[last.ControlPoints.Count - 1];
                    currentX = lastPt.X;
                    currentY = lastPt.Y;
                }

                previewSegment.ptS.X = currentX;
                previewSegment.ptS.Y = currentY;
                previewSegment.ptE.X = currentX + ddaLength;
                previewSegment.ptE.Y = currentY;

                previewDDA.Xst = currentX;
                previewDDA.Yst = currentY;
                previewDDA.Xend = currentX + ddaLength;
                previewDDA.Yend = currentY;
                previewDDA.calc();

                previewCircle.XC = (int)currentX;
                previewCircle.YC = (int)currentY - circleRadius;
                previewCircle.st = 270;
                previewCircle.end = 630;
            }
        }

        void draw_background()
        {
            cMultipleimages pnn = new cMultipleimages();
            pnn.images = new List<Bitmap>();
            Bitmap img2 = new Bitmap("imgg.png");
            img2.MakeTransparent(img2.GetPixel(0, 0));
            pnn.images.Add(img2);
            pnn.x = 100;
            pnn.y = 100;
            Lhome.Add(pnn);
        }

        void ggui(Graphics g)
        {
            Font uiFont = new Font("Arial", 14);
            Font btnFont = new Font("Arial", 18, FontStyle.Bold);
            Brush white = Brushes.White;

            g.DrawString("MY GAME", new Font("Arial", 32, FontStyle.Bold), Brushes.White,
                this.ClientSize.Width / 2 - 120, 40);

            Brush panelBrush = new SolidBrush(Color.FromArgb(120, 0, 0, 0));
            g.FillRectangle(panelBrush, 20, this.ClientSize.Height - 120,
                this.ClientSize.Width - 40, 80);

            g.DrawString("R = Rotate | L = Longer | l = Shorter | A = Add Segment",
                uiFont, white, 40, this.ClientSize.Height - 100);
            g.DrawString("DEL = Undo | 1 = DDA | 2 = Circle | 3 = Curve | P = Drive Car",
                uiFont, white, 40, this.ClientSize.Height - 75);

            if (!gameStarted)
            {
                LinearGradientBrush startBrush = new LinearGradientBrush(startBtn,
                    Color.FromArgb(0, 200, 255), Color.FromArgb(0, 120, 255), 45f);
                g.FillRectangle(startBrush, startBtn);
                g.DrawRectangle(Pens.White, startBtn);
                g.DrawString("START", btnFont, Brushes.White, startBtn.X + 50, startBtn.Y + 15);

                LinearGradientBrush exitBrush = new LinearGradientBrush(exitBtn,
                    Color.FromArgb(255, 80, 120), Color.FromArgb(200, 30, 60), 45f);
                g.FillRectangle(exitBrush, exitBtn);
                g.DrawRectangle(Pens.White, exitBtn);
                g.DrawString("EXIT", btnFont, Brushes.White, exitBtn.X + 65, exitBtn.Y + 15);
            }

            if (gameStarted)
            {
                for (int i = 0; i < Lhome.Count; i++)
                    for (int j = 0; j < Lhome[i].images.Count; j++)
                        g.DrawImage(Lhome[i].images[j], 0, 0,
                            this.ClientSize.Width, this.ClientSize.Height);

                panelBrush = new SolidBrush(Color.FromArgb(120, 0, 0, 0));
                g.FillRectangle(panelBrush, 20, this.ClientSize.Height - 120,
                    this.ClientSize.Width - 40, 80);

                g.DrawString("R = Rotate | L = Longer | l = Shorter | A = Add Segment",
                    uiFont, white, 40, this.ClientSize.Height - 100);
                g.DrawString("DEL = Undo | 1 = DDA | 2 = Circle | 3 = Curve | P = Drive Car",
                    uiFont, white, 40, this.ClientSize.Height - 75);

                g.DrawString("Score: " + score, uiFont, white, 50, 50);
            }
        }

        void DrawDDA(Graphics g, DDA dda, Color color)
        {
            float railOffset = 13f;
            float tieSpacing = 20f;
            float tieLength = 14f;

            float dx = dda.Xend - dda.Xst;
            float dy = dda.Yend - dda.Yst;
            float len = (float)Math.Sqrt(dx * dx + dy * dy);
            if (len == 0) return;

            float px = -dy / len;
            float py = dx / len;

            Pen railPen = new Pen(Color.FromArgb(40, 40, 40), 10);
            Pen tiePen = new Pen(Color.FromArgb(40, 40, 40), 10);

            g.DrawLine(railPen,
                dda.Xst + px * railOffset, dda.Yst + py * railOffset,
                dda.Xend + px * railOffset, dda.Yend + py * railOffset);

            g.DrawLine(railPen,
                dda.Xst - px * railOffset, dda.Yst - py * railOffset,
                dda.Xend - px * railOffset, dda.Yend - py * railOffset);

            float steps = len / tieSpacing;
            for (float t = 0; t <= 1f; t += 1f / steps)
            {
                float tx = dda.Xst + dx * t;
                float ty = dda.Yst + dy * t;
                g.DrawLine(tiePen,
                    tx + px * tieLength, ty + py * tieLength,
                    tx - px * tieLength, ty - py * tieLength);
            }
        }

        void DrawCircle(Graphics g, Circle c, Color color)
        {
            float railOffset = 13f;
            float tieSpacing = 20f;

            Pen railPen = new Pen(Color.FromArgb(40, 40, 40), 10);
            Pen tiePen = new Pen(Color.FromArgb(40, 40, 40), 10);

            PointF prevOuter = new PointF();
            PointF prevInner = new PointF();
            bool first = true;

            for (float i = c.st; i <= c.end; i += 1.0f)
            {
                PointF curr = c.Getnextpoint((int)i);
                float angle = (float)(i * Math.PI / 180);
                float px = (float)Math.Cos(angle);
                float py = (float)Math.Sin(angle);

                PointF outer = new PointF(curr.X + px * railOffset, curr.Y + py * railOffset);
                PointF inner = new PointF(curr.X - px * railOffset, curr.Y - py * railOffset);

                if (!first)
                {
                    g.DrawLine(railPen, prevOuter, outer);
                    g.DrawLine(railPen, prevInner, inner);
                }

                if (i % tieSpacing < 1.0f)
                    g.DrawLine(tiePen, outer, inner);

                prevOuter = outer;
                prevInner = inner;
                first = false;
            }
        }

        void DrawScene(Graphics g)
        {
            g.Clear(Color.SkyBlue);
            ggui(g);

            if (gameStarted)
            {
                for (int i = 0; i < ddaLines.Count; i++)
                    DrawDDA(g, ddaLines[i], Color.White);

                for (int i = 0; i < circleList.Count; i++)
                    DrawCircle(g, circleList[i], Color.White);

                for (int i = 0; i < curveList.Count; i++)
                    curveList[i].DrawCurve(g);

                if (activeSegment == 1) DrawDDA(g, previewDDA, Color.Yellow);
                if (activeSegment == 2) DrawCircle(g, previewCircle, Color.Yellow);
                if (activeSegment == 3) previewCurve.DrawCurve(g);

                // DRAW CAR 
                if (carMoving && trackPoints.Count > 0 && movecar < trackPoints.Count)
                {
                    PointF pos = trackPoints[movecar];
                    float angle = GetCarAngle(movecar);

                    // save the current transform
                    System.Drawing.Drawing2D.Matrix savedMatrix = g.Transform;

                    // move origin to car position, rotate to match track direction
                    g.TranslateTransform(pos.X, pos.Y);
                    g.RotateTransform(angle);

                    // draw car image centered on the track point
                    g.DrawImage(carImage,
                        -carImage.Width / 2,
                        -carImage.Height / 2,
                        carImage.Width,
                        carImage.Height);

                    // restore transform
                    g.Transform = savedMatrix;
                }
            }
        }

        PointF DoRotate(PointF pMe, PointF pRef, float th)
        {
            PointF me2 = new PointF();
            me2.X = pMe.X - pRef.X;
            me2.Y = pMe.Y - pRef.Y;

            PointF me3 = new PointF();
            me3.X = (float)(me2.X * Math.Cos(th) - me2.Y * Math.Sin(th));
            me3.Y = (float)(me2.X * Math.Sin(th) + me2.Y * Math.Cos(th));

            pMe.X = me3.X + pRef.X;
            pMe.Y = me3.Y + pRef.Y;
            return pMe;
        }

        Bitmap LoadCar(string path)
        {
            Bitmap src = new Bitmap(path);
            src.MakeTransparent(src.GetPixel(0, 0));  // transparent background
            int w = src.Width / 20;
            int h = src.Height / 20;
            Bitmap resized = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(resized))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(src, 0, 0, w, h);
            }
            return resized;
        }

        void DrawDBuff(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }

        void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDBuff(e.Graphics);
        }

        void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            carImage = LoadCar("x.jpeg");

            draw_background();

            startBtn = new Rectangle(this.ClientSize.Width / 2 - 100, 250, 200, 60);
            exitBtn = new Rectangle(this.ClientSize.Width / 2 - 100, 330, 200, 60);

            currentX = 10;
            currentY = this.ClientSize.Height / 2f;
            ddaLength = 150f;

            previewSegment.ptS.X = currentX;
            previewSegment.ptS.Y = currentY;
            previewSegment.ptE.X = currentX + ddaLength;
            previewSegment.ptE.Y = currentY;

            previewDDA.Xst = previewSegment.ptS.X;
            previewDDA.Yst = previewSegment.ptS.Y;
            previewDDA.Xend = previewSegment.ptE.X;
            previewDDA.Yend = previewSegment.ptE.Y;
            previewDDA.calc();

            previewCircle.XC = (int)currentX;
            previewCircle.YC = (int)currentY - circleRadius;
            previewCircle.Rad = circleRadius;
            previewCircle.st = 270;
            previewCircle.end = 630;
        }
    }
}