using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label9.Visible = false;
            textBox4.Visible = false;


            radioButton1.CheckedChanged += radioButton1_CheckedChanged; 
            radioButton2.CheckedChanged += radioButton2_CheckedChanged; 
                                                                        
        }
      
        double f(double x, ref int k)
        {
            switch (k)
            {
                case 0: return x * x - 4;
                case 1:
                    if (x <= 0) return double.NaN;
                    return 3 * x - 4 * Math.Log(x) - 5;
                case 2: return Math.Exp(x) + x;
            }
            return 0;
        }

        double fp(double x, double d, ref int k) 
        {
            return (f(x + d, ref k) - f(x, ref k)) / d;
        }

        double f2p(double x, double d, ref int k) 
        {
            return (f(x + d, ref k) + f(x - d, ref k) - 2 * f(x, ref k)) / (d * d);
        }



        void MDP(double a, double b, double Eps, ref int k, out double root, out int L)
        {
            double c = 0, Fc;
            L = 0;

            while (b - a > Eps)
            {
                c = a + 0.5 * (b - a);
                L++;
                Fc = f(c, ref k);

                if (Math.Abs(Fc) < Eps)
                {
                    root = c;
                    return;
                }

                if (f(a, ref k) * Fc > 0) a = c;
                else b = c;
            }
            root = c;
        }

        void MN(double a, double b, double Eps, ref int k, int Kmax, out double root, out int L)
        {
            double x, Dx, D;
            D = Eps / 100.0;
            L = 0;

            x = b;
            if (f(x, ref k) * f2p(x, D, ref k) < 0)
                x = a;

            if (f(x, ref k) * f2p(x, D, ref k) < 0)
                MessageBox.Show("Для цього рівняння збіжність ітерацій не гарантована, вибрана інша початкова точка.");

            for (int i = 1; i <= Kmax; i++)
            {
                L = i;
                double f_prime = fp(x, D, ref k);

                if (Math.Abs(f_prime) < D)
                {
                    MessageBox.Show("Похідна близька до нуля. Метод Ньютона не застосовний.");
                    root = -2000.0;
                    return;
                }

                Dx = f(x, ref k) / f_prime;
                x = x - Dx;

                if (Math.Abs(Dx) < Eps)
                {
                    root = x;
                    return;
                }
            }

            MessageBox.Show("За задану кількість ітерацій кореня не знайдено");
            root = -1000.0;
        }


        
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox5.Clear(); 
                textBox6.Clear(); 

                label9.Visible = false; 
                textBox4.Visible = false; 
                textBox4.Clear();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                textBox5.Clear(); 
                textBox6.Clear(); 

                label9.Visible = true; 
                textBox4.Visible = true; 
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox5.Clear();
            textBox6.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int Kmax = 0, L = 0;
            double D, Eps, a, b, root = double.NaN;

            CultureInfo culture = CultureInfo.InvariantCulture;

            textBox5.Clear();
            textBox6.Clear();

            
            bool isMDP = radioButton1.Checked;
            bool isMN = radioButton2.Checked;

            if (!isMDP && !isMN)
            {
                MessageBox.Show("Оберіть метод!");
                return;
            }

            
            int equationIndex = comboBox1.SelectedIndex;
            if (equationIndex == -1)
            {
                MessageBox.Show("Оберіть рівняння!");
                comboBox1.Focus();
                return;
            }
            int k_ref = equationIndex;

          
            if (!double.TryParse(textBox1.Text, NumberStyles.Any, culture, out a) ||
                !double.TryParse(textBox2.Text, NumberStyles.Any, culture, out b) ||
                !double.TryParse(textBox3.Text, NumberStyles.Any, culture, out Eps))
            {
                MessageBox.Show("Будь ласка, введіть числові значення для a, b та Eps.");
                return;
            }

         
            if (a > b)
            {
                D = a;
                a = b;
                b = D;
                textBox1.Text = a.ToString(culture);
                textBox2.Text = b.ToString(culture);
            }

           
            if ((Eps > 1e-1) || (Eps <= 0))
            {
                Eps = 1e-4;
                textBox3.Text = Eps.ToString(culture);
            }

            

            if (isMDP) 
            {
               
                if (k_ref == 1 && (a <= 0 || b <= 0))
                {
                    MessageBox.Show("Для даного рівняння інтервал [a, b] має бути > 0 (D: x > 0).");
                    textBox1.Focus();
                    return;
                }
                
                if ((f(a, ref k_ref)) * (f(b, ref k_ref)) > 0)
                {
                    MessageBox.Show("Введіть правильний інтервал [a, b]! Знаки f(a) та f(b) повинні бути протилежними.");
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox1.Focus();
                    return;
                }

               
                MDP(a, b, Eps, ref k_ref, out root, out L);
            }
            else if (isMN) 
            {
           
                if (!int.TryParse(textBox4.Text, out Kmax))
                {
                    MessageBox.Show("Введіть ціле число в поле 'Kmax' (максимальна кількість ітерацій).");
                    textBox4.Focus();
                    return;
                }

                MN(a, b, Eps, ref k_ref, Kmax, out root, out L);
            }

        
            if (!double.IsNaN(root) && root > -1000.0)
            {
                textBox5.Text = root.ToString("G15", culture);
                textBox6.Text = L.ToString();
            }
            else if (root == -1000.0)
            {
            
                textBox6.Text = Kmax.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox4.Visible = false;


      
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            comboBox1.SelectedIndex = -1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
