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

namespace WindowsFormsApp1 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            DrawChart();
            DrawDichotomy();
        }

        #region Нахождение дихотомии
        private double eps = 0.001;
        private double dichotomyLeftBorder = 0;
        private double dicphotomyRightBorder;
        
        private double Dichotomy() {
            double dichotomyPoint;
            dicphotomyRightBorder = Convert.ToDouble(tbRightBorder.Text);

            do {
                dichotomyPoint = ( dichotomyLeftBorder + dicphotomyRightBorder ) / 2.0;

                if(parseMath(dichotomyLeftBorder) * parseMath(dichotomyPoint) < 0) {
                    dicphotomyRightBorder = dichotomyPoint;
                }
                else if (parseMath(dicphotomyRightBorder) * parseMath(dichotomyPoint) < 0) {
                    dichotomyLeftBorder = dichotomyPoint;
                } else {
                    MessageBox.Show("Точка дихотомии не найдена");
                    return 0;
                }
            } while (Math.Abs(dicphotomyRightBorder - dichotomyLeftBorder) > eps);

            return dichotomyPoint;
        }
        #endregion

        #region Отрисовывание дихотомии
        private void DrawDichotomy() {
            double dichotomyPointX = Dichotomy();
            this.chart1.Series[1].Points.Clear();
            for (int i = -4; i < 5; i++) {
                this.chart1.Series[1].Points.AddXY(dichotomyPointX, i);
            }
        }
        #endregion

        #region построение графика

        private double leftBorder = 0;
        private double rightBorder;
        private double step = 0.1;
        private double x;
        private double y = 0;

        private void DrawChart() {
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

        private void tbRightBorder_KeyPress(object sender, KeyPressEventArgs e) {
            char number = e.KeyChar;

            if (!Char.IsDigit(number)) {
                e.Handled = true;
            }
        }

        private void вычислитьToolStripMenuItem_Click(object sender, EventArgs e) {
            button1_Click(sender, e);
        }

        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e) {
            tbInput.Text = null;
            tbRightBorder.Text = null;
        }
    }
}
