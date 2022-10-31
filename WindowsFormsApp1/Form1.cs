using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using org.mariuszgromada.math.mxparser;
using org.mariuszgromada.math.mxparser.mathcollection;
using org.mariuszgromada.math.mxparser.parsertokens;
using org.mariuszgromada.math.mxparser.syntaxchecker;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1 {

    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            if (tbInput.Text == string.Empty || tbRightBorder.Text == string.Empty) {
                MessageBox.Show("Пожалуйста, введите данные");
                return;
            }
            chart1.ChartAreas[0].AxisX.Minimum = Convert.ToInt32(tbLeftBorder.Text);
            chart1.ChartAreas[0].AxisX.Maximum = Convert.ToInt32(tbRightBorder.Text);
            
            DrawChart();
            DrawDichotomy();
        }

        #region Нахождение дихотомии


        private double eps;
        private double dichotomyLeftBorder;
        private double dicphotomyRightBorder;
        private bool isInfinity = false;

        private double Dichotomy() {
            eps = 0.01;

            double dichotomyPoint = 0;
            double diffDichotomy = 0;
            dichotomyLeftBorder = Convert.ToDouble(tbLeftBorder.Text);
            dicphotomyRightBorder = Convert.ToDouble(tbRightBorder.Text);
            double a = 0;
            double b = 0;

            do {
                dichotomyPoint = ( dichotomyLeftBorder + dicphotomyRightBorder ) / 2;
                diffDichotomy = parseDerivate(dichotomyPoint);
                a = parseDerivate(dichotomyLeftBorder);
                b = parseDerivate(dicphotomyRightBorder);

                if (b <= -200) {
                    isInfinity = true;
                    return dicphotomyRightBorder;
                }

                for (double index = dichotomyLeftBorder; index < dichotomyLeftBorder + 2; index += 0.1) {
                    if (parseDerivate(index) <= -200) {
                        isInfinity = true;
                        return index;
                    }
                }

                if (a * diffDichotomy < 0) {
                    dicphotomyRightBorder = dichotomyPoint;
                } else if (a * diffDichotomy > 0) {
                    dichotomyLeftBorder = dichotomyPoint;
                } else {
                    dichotomyLeftBorder++;
                }
            } while (Math.Abs(dicphotomyRightBorder - dichotomyLeftBorder) > eps);
            return dichotomyPoint;
        }
        private double maxDichotomy() {
            eps = 0.01;

            double dichotomyPoint = 0;
            double diffDichotomy = 0;
            dichotomyLeftBorder = Convert.ToDouble(tbLeftBorder.Text);
            dicphotomyRightBorder = Convert.ToDouble(tbRightBorder.Text);
            double a = 0;
            double b = 0;

            do {
                dichotomyPoint = ( dichotomyLeftBorder + dicphotomyRightBorder ) / 2;
                diffDichotomy = parseDerivate(dichotomyPoint);
                a = parseDerivate(dichotomyLeftBorder);
                b = parseDerivate(dicphotomyRightBorder);

                if (b >= 200) {
                    return dicphotomyRightBorder;
                }

                for (double index = dichotomyLeftBorder; index < dichotomyLeftBorder + 2; index += 0.1) {
                    if (parseDerivate(index) >= 200) {
                        return index;
                    }
                }

                if (b * diffDichotomy < 0) {
                    dichotomyLeftBorder = dichotomyPoint;
                } else if (b * diffDichotomy > 0) {
                    dicphotomyRightBorder = dichotomyPoint;
                } else {
                    dichotomyLeftBorder++;
                }
            } while (Math.Abs(dicphotomyRightBorder - dichotomyLeftBorder) > eps);
            return dichotomyPoint;
        }
        #endregion

        #region Отрисовывание дихотомии
        private void DrawDichotomy() {
            List<double> dichotomyPointX = new List<double>();
            dichotomyPointX.Add(Dichotomy());
            double axisYMin = parseMath(dichotomyPointX[0]);
            double axisYMax = parseMath(maxDichotomy());

            this.chart1.ChartAreas[0].AxisY.Minimum = axisYMin;
            this.chart1.ChartAreas[0].AxisY.Maximum = axisYMax;
            //Найти y дихотомии

            this.chart1.Series[1].Points.Clear();
            this.chart1.Series[1].Points.AddXY(dichotomyPointX[0], axisYMin);
            
            if (isInfinity){ MessageBox.Show($"В точке:{dichotomyPointX[0]} стремится к бесконечности"); } else
                { MessageBox.Show($"Точка окупа: {dichotomyPointX[0]}"); }
            dichotomyPointX.Clear();
        }
        #endregion

        #region построение графика

        private double leftBorder;
        private double rightBorder;
        private double step = 0.1;
        private double x;
        private double y = 0;

        private void DrawChart() {
            leftBorder = Convert.ToDouble(tbLeftBorder.Text);
            rightBorder = Convert.ToDouble(tbRightBorder.Text);//Правая граница
            x = leftBorder;

            //Очистка графика
            this.chart1.Series[0].Points.Clear();

            while (x <= rightBorder) {
                y = parseMath(x);
                this.chart1.Series[0].Points.AddXY(x, y);
                x += step;
            }
        }
        #endregion

        #region парсинг
        private double parseMath(double point) {
            Argument x = new Argument("x");
            x.setArgumentValue(point);
            Expression expression = new Expression(tbInput.Text.ToString(), x);
            return expression.calculate();
        }
        #endregion

        #region Производная вычислять
        private double parseDerivate(double point) {
            Argument x = new Argument("x");
            x.setArgumentValue(point);
            string formula = tbInput.Text.ToString();
            string derivateFormula = "der(" + formula + ",x)";
            Expression expression = new Expression(derivateFormula, x);
            return expression.calculate();
        }
        
        #endregion

        private void tbRightBorder_KeyPress(object sender, KeyPressEventArgs e) {
            CheckInput.OnlyDigit(sender, e);
        }

        private void вычислитьToolStripMenuItem_Click(object sender, EventArgs e) {
            button1_Click(sender, e);
        }

        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e) {
            tbInput.Text = null;
            tbRightBorder.Text = null;
            this.chart1.Series[0].Points.Clear();
        }
    }
}
