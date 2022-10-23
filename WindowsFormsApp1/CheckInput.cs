using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1 {
    public class CheckInput {
        public static void OnlyDigit(object sender, KeyPressEventArgs e) {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && number != 8) {
                e.Handled = true;
            }
        }
    }
}
