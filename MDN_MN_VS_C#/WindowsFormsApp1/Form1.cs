using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
       
            label7.Visible = false;
            textBox4.Visible = false;
        }

        
        double f(double x, ref int k1) 
        {
            switch (k1)
            {
                case 0: return x * x - 4;
                case 1: return 3 * x - 4 * Math.Log(x) - 5;
            }
            return 0;
        }

        double fp(double x, double d, ref int k1) // перша похідна (чисельно)
        {
            return (f(x + d, ref k1) - f(x, ref k1)) / d;
        }

        double f2p(double x, double d, ref int k1) // друга похідна (чисельно)
        {
            return (f(x + d, ref k1) + f(x - d, ref k1) - 2 * f(x, ref k1)) / (d * d);
        }

       

        double MDP(double a, double b, double Eps, ref int k1, ref int L) // Метод ділення навпіл (МДН)
        {
            double c = 0, Fc;
            while (b - a > Eps)
            {
                c = 0.5 * (b - a) + a;
                L++; // лічильник кількості поділів
                Fc = f(c, ref k1);

                if (Math.Abs(Fc) < Eps) // перевірка поблизу кореня
                    return c;

                if (f(a, ref k1) * Fc > 0) a = c;
                else b = c;
            }
            return c;
        }

        double MN(double a, double b, double Eps, ref int k1, int Kmax, ref int L) // Метод Ньютона (МН)
        {
            double x, Dx, D;
            D = Eps / 100.0;

            // Вибір початкового наближення x0 = b або x0 = a
            x = b;
            if (f(x, ref k1) * f2p(x, D, ref k1) < 0)
                x = a;

            // Перевірка умови збіжності
            if (f(x, ref k1) * f2p(x, D, ref k1) < 0)
                MessageBox.Show("Для цього рівняння збіжність ітерацій не гарантована, вибрана інша початкова точка.");

            for (int i = 1; i <= Kmax; i++)
            {
                // Формула Ньютона: x(n+1) = x(n) - f(x(n))/f'(x(n))
                double f_prime = fp(x, D, ref k1);
                if (Math.Abs(f_prime) < D) // Захист від ділення на нуль
                {
                    MessageBox.Show("Похідна близька до нуля. Метод Ньютона не застосовний.");
                    return -2000.0; // Інша ознака помилки
                }

                Dx = f(x, ref k1) / f_prime;
                x = x - Dx;

                if (Math.Abs(Dx) < Eps)
                {
                    L = i;
                    return x; // Успішне завершення
                }
            }

            MessageBox.Show("За задану кількість ітерацій кореня не знайдено");
            return -1000.0; // Ознака: корінь не знайдено
        }

      
      

        private void button3_Click(object sender, EventArgs e) // Кнопка "Закрити"
        {
            Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            int L = 0, k = -1, Kmax = 0, m = -1;
            double D, Eps, a, b;

            textBox5.Clear();
            textBox6.Clear();

            // 1. ВИБІР МЕТОДУ
            m = comboBox1.SelectedIndex;
            if (m == -1)
            {
                MessageBox.Show("Оберіть метод!");
                comboBox1.Focus();
                return;
            }

            // 2. ВИБІР РІВНЯННЯ
            k = comboBox2.SelectedIndex;
            if (k == -1)
            {
                MessageBox.Show("Оберіть рівняння!");
                comboBox2.Focus();
                return;
            }

            // 3. ВВЕДЕННЯ ДАНИХ (a, b, Eps)

            if (!double.TryParse(textBox1.Text, out a) ||
                !double.TryParse(textBox2.Text, out b) ||
                !double.TryParse(textBox3.Text, out Eps))
            {
                MessageBox.Show("Будь ласка, введіть числові значення для a, b та Eps.");
                return;
            }

            // Обмін a і b, якщо a > b
            if (a > b)
            {
                D = a;
                a = b;
                b = D;
                textBox1.Text = Convert.ToString(a);
                textBox2.Text = Convert.ToString(b);
            }

            // Перевірка та коригування Eps
            if ((Eps > 1e-1) || (Eps <= 0))
            {
                Eps = 1e-4; // Задаємо примусово Eps = 0.0001
                textBox3.Text = Convert.ToString(Eps);
            }

            // 4. СПЕЦИФІЧНІ ПЕРЕВІРКИ ТА ВИКЛИК МЕТОДІВ

            if (m == 0) // Метод ділення навпіл (МДН)
            {
                // Перевірка, чи є корінь на інтервалі [a, b]
                if ((f(a, ref k)) * (f(b, ref k)) > 0)
                {
                    MessageBox.Show("Введіть правильний інтервал [a, b]! Знаки f(a) та f(b) повинні бути протилежними.");
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox1.Focus();
                    return;
                }

                // Перевірка, чи межі інтервалу [a, b] не є поблизу кореня
                if (Math.Abs(f(a, ref k)) < Eps)
                {
                    textBox5.Text = Convert.ToString(a);
                    textBox6.Text = Convert.ToString(L);
                    return;
                }
                if (Math.Abs(f(b, ref k)) < Eps)
                {
                    textBox5.Text = Convert.ToString(b);
                    textBox6.Text = Convert.ToString(L);
                    return;
                }

                // Виклик методу
                double root = MDP(a, b, Eps, ref k, ref L);
                textBox5.Text = Convert.ToString(root);
                textBox6.Text = Convert.ToString(L);
                // label10.Text = "К-ть поділів ="; // Якщо є така мітка
            }
            else // Метод Ньютона (МН)
            {
                // Kmax
                if (!int.TryParse(textBox4.Text, out Kmax))
                {
                    MessageBox.Show("Введіть ціле число в поле 'Kmax' (максимальна кількість ітерацій).");
                    textBox4.Focus();
                    return;
                }

                // Виклик методу
                double root = MN(a, b, Eps, ref k, Kmax, ref L);

                if (root > -1000.0) // Якщо корінь знайдено
                {
                    textBox5.Text = Convert.ToString(root);
                    textBox6.Text = Convert.ToString(L);
                }
                else // Якщо корінь не знайдено (MN повернув -1000.0)
                {
                    // textBox5 залишиться пустим, повідомлення вже показано в MN
                    textBox6.Text = Convert.ToString(Kmax);
                }
                // label10.Text = "К-ть ітерац.="; // Якщо є така мітка
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
            // Скидаємо видимість Kmax, як при виборі МДН
            label7.Visible = false;
            textBox4.Visible = false;
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Керування видимістю поля Kmax (textBox4) та мітки (label7)
            textBox5.Clear();
            textBox6.Clear();

            switch (comboBox1.SelectedIndex)
            {
                case 0: // Метод ділення навпіл
                    {
                        label7.Visible = false;
                        textBox4.Visible = false;
                        textBox4.Clear();
                    }
                    break;
                case 1: // Метод Ньютона
                    {
                        label7.Visible = true;
                        textBox4.Visible = true;
                    }
                    break;
            }
        }
    }
}