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
            DrawChart();
            DrawDichotomy();
        }

        #region Нахождение дихотомии


        private double eps;
        private double dichotomyLeftBorder;
        private double dicphotomyRightBorder;
        private bool found;
        private int Attempts;
        
        private double Dichotomy() {
            eps = 0.01;

            double dichotomyPoint = 0;
            dichotomyLeftBorder = 0;
            dicphotomyRightBorder = 2;
            found = false;
            Attempts = 0;

            //из минуса в плюс
            while (!found) {

                do {
                    dichotomyPoint = ( dichotomyLeftBorder + dicphotomyRightBorder ) / 2;
                    if(parseDerivate(dichotomyLeftBorder) * parseDerivate(dichotomyPoint) < 0) {
                        dicphotomyRightBorder = dichotomyPoint;
                    } else if(parseDerivate(dichotomyLeftBorder) * parseDerivate(dichotomyPoint) > 0) {
                        dichotomyLeftBorder = dichotomyPoint;
                    } else {
                        dicphotomyRightBorder++;
                        dichotomyLeftBorder++;
                        Attempts++;
                        break;
                    }
                } while (Math.Abs(dicphotomyRightBorder - dichotomyLeftBorder) > eps * 2);

                if(parseMath(dichotomyPoint) <= 0 && parseDerivate(dichotomyPoint - eps * 100) <= 0 && parseDerivate(dichotomyPoint + eps * 100) >= 0) {
                    found = true;
                }
                else if(Attempts > 300) {
                    MessageBox.Show("Не найдено");
                    return 0;
                }
                dicphotomyRightBorder++;
                dichotomyLeftBorder++;

                #region мусор но нужный
                //do {
                //    dichotomyPoint = ( dichotomyLeftBorder + dicphotomyRightBorder ) / 2.0;

                //    if (parseMath(dichotomyLeftBorder) > parseMath(dichotomyPoint)) {
                //        dicphotomyRightBorder = dichotomyPoint;
                //    } else if (parseMath(dicphotomyRightBorder) * parseMath(dichotomyPoint) < 0 ){
                //        dichotomyLeftBorder = dichotomyPoint;
                //    } else {
                //        dicphotomyRightBorder++;
                //        Attempts++;
                //        break;
                //    }
                //} while (Math.Abs(dicphotomyRightBorder - dichotomyLeftBorder) > eps * 2);

                //if(parseMath(x1) >= parseMath((a + b) / 2) && parseMath(x2) >= parseMath((a + b) / 2)) {
                //    found = true;
                //}
                //else if(Attempts > 550) {
                //    return 0;
                //}
                #endregion
            }
            #region тот же мусор
            //while (!found) {
            //    do {
            //        dichotomyPoint = ( dichotomyLeftBorder + dicphotomyRightBorder ) / 2.0;

            //        if (parseMath(dichotomyLeftBorder) * parseMath(dichotomyPoint) < 0) {
            //            dicphotomyRightBorder = dichotomyPoint;
            //        } else if (parseMath(dicphotomyRightBorder) * parseMath(dichotomyPoint) < 0 ){
            //            dichotomyLeftBorder = dichotomyPoint;
            //        } else {
            //            dicphotomyRightBorder++;
            //            Attempts++;
            //            break;
            //        }
            //    } while (Math.Abs(dicphotomyRightBorder - dichotomyLeftBorder) > eps);

            //    if(parseMath(dichotomyPoint) >= eps * -100 && parseMath(dichotomyPoint)  <=  eps * 100) {
            //        found = true;
            //    }
            //    else if(Attempts > 550) {
            //        return 0;
            //    }
            //}
            #endregion да
            return dichotomyPoint;
        }
        #endregion

        #region Отрисовывание дихотомии
        private void DrawDichotomy() {
            //double dichotomyPointX = Dichotomy();
            List<double> dichotomyPointX = new List<double>();
            dichotomyPointX.Add(Dichotomy());

            this.chart1.Series[1].Points.Clear();
            for (int i = -4; i < 5; i++) {
                this.chart1.Series[1].Points.AddXY(dichotomyPointX[0], i);
            }
            MessageBox.Show($"Точка окупа: {dichotomyPointX[0]}");
            dichotomyPointX.Clear();
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
