using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace Renocimiento_facial_Tu_Codigo
{
    public partial class Form1 : Form
    {SoundPlayer media = new SoundPlayer();
        public Form1()
        {
            InitializeComponent();
        }

       

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            media.Play();



        }
        int cont = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            cont += 1;
            label1.Text = label1.Text + ".";
            if (cont == 5)
            {
                
                timer1.Stop();
                if (DA.cnx() == "Conexion exitosa")
                {
                    frmReconocedorF f = new frmReconocedorF();

                    f.Show();
                    this.Hide();
                }


            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
