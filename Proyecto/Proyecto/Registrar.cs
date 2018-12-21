using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Data.OleDb;
using System.Runtime.InteropServices;

namespace Renocimiento_facial_Tu_Codigo
{
    public partial class Registrar : Form
    {

        

        public int heigth, width;

        public string[] Labels;
       
        int con = 0, ini = 0, fin;
        //DECLARANDO TODAS LAS VARIABLES, vectores y  haarcascades
        Image<Bgr, Byte> currentFrame;
        Capture grabber;
        HaarCascade face;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        Image<Gray, byte> result, TrainedFace = null;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> labels1 = new List<string>();
        List<string> NamePersons = new List<string>();
        int ContTrain, NumLabels, t, id;

        private void btnTomar_Click(object sender, EventArgs e)
        {
            if (btnTomar.Text == "Iniciar")
            {
                INICIARCACTURA();
                pictureBox1.Visible = false;
                btnTomar.Text = "Tomar";


                
            }
            else
            {
                try
                {
                    pictureBox2.Visible = false;
                    pictureBox2.Image = null;
                    //Trained face counter
                    ContTrain = ContTrain + 1;

                    //Get a gray frame from capture device
                    gray = grabber.QueryGrayFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

                    //Face Detector
                    MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(face, 1.5, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));

                    //Action for each element detected
                    foreach (MCvAvgComp f in facesDetected[0])
                    {
                        TrainedFace = currentFrame.Copy(f.rect).Convert<Gray, byte>();
                        break;
                    }

                    //resize face detected image for force to compare the same size with the 
                    //test image with cubic interpolation type method
                    TrainedFace = result.Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                    trainingImages.Add(TrainedFace);


                    //Show face added in gray scale
                    imageBox1.Image = TrainedFace;
                }
                catch 
                {

                    
                }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Application.Idle -= new EventHandler(FrameGrabber);//Detenemos el evento de captura
                grabber.Dispose();//Dejamos de usar la clase para capturar usar los dispositivos
                imageBox2.ImageLocation = "img/Imagen.png";//reiniciamos la imagen del control
                btnTomar.Text = "Iniciar";

                frmReconocedorF f = new frmReconocedorF();
                f.Show();
                this.Close();
            }
            catch 
            {
                frmReconocedorF f = new frmReconocedorF();
                f.Show();
                this.Close();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            DA.eliminar(int.Parse(dataGridView1.CurrentRow.Cells["id"].Value.ToString()));
            DA.listaCaras(dataGridView1);

        }
        void limpiar()
        {
            imageBox1.Image = null;
            pictureBox2.Image = null;
            txtNombre.Clear();
            BtnRegistrar.Text = "Registrar";

        }
        private void btncancelar_Click(object sender, EventArgs e)
        {
            limpiar();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
          pictureBox2.Visible = true;
            imageBox1.Image = null;
            BtnRegistrar.Text = "Guardar";
            btncancelar.Visible = true;
          id=  int.Parse(dataGridView1.CurrentRow.Cells["id"].Value.ToString());
          txtNombre.Text = dataGridView1.CurrentRow.Cells["Nombre"].Value.ToString();
            //Obtener la posicion de la imagen 
          int P= dataGridView1.CurrentRow.Index; 
          pictureBox2.Image= DA.ConvertByteToImg(P);

            
        }

        private void BtnRegistrar_Click(object sender, EventArgs e)
        {
            
            try
            {
                

                
                if (BtnRegistrar.Text == "Guardar")
                {
                    
                    if (txtNombre.Text != "")

                    {
                        if (pictureBox2.Image != null)
                        {
                            DA.EditarImagen(id, txtNombre.Text, DA.ConvertImgToBinary(pictureBox2.Image));
                            
                          
                        }
                        else
                        {
                            DA.EditarImagen(id, txtNombre.Text, DA.ConvertImgToBinary(imageBox1.Image.Bitmap));
                            
                            

                        }

                        limpiar();
                    }
                    

                }
                else {

                    labels.Add(txtNombre.Text);
                    DA.GuardarImagen(txtNombre.Text,DA.ConvertImgToBinary(imageBox1.Image.Bitmap));
                                   
                    MessageBox.Show("Agregado correctamente", "Capturado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtNombre.Text = "";
                    imageBox1.Image = null;
                }
                DA.listaCaras(dataGridView1);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        string name;

        public Registrar()
        {
            InitializeComponent();
            heigth = this.Height; width = this.Width;
            //GARGAMOS LA DETECCION DE LAS CARAS POR  haarcascades 
            face = new HaarCascade("haarcascade_frontalface_default.xml");
            try
            {
                DA.ObtenerBytesImagen();//carga de caras previus trainned y etiquetas para cada imagen                
                Labels = DA.Name; //Labelsinfo.Split('%');//separo los nombres de los usuarios 
                NumLabels = DA.TotalUser;// Convert.ToInt32(Labels[0]);//extraigo el total de usuarios registrados
                ContTrain = NumLabels;


                for (int tf = 0; tf < NumLabels; tf++)//recorro el numero de nombres registrados
                {
                    con = tf;
                    Bitmap bmp = new Bitmap(DA.ConvertByteToImg(con));
                    //LoadFaces = "face" + tf + ".bmp";
                    trainingImages.Add(new Image<Gray, byte>(bmp));//cargo la foto con ese nombre
                    labels.Add(Labels[tf]);//cargo el nombre que se encuentre en la posicion del tf

                }
            }
            catch (Exception e)
            {//Si la variable NumLabels es 0 me presenta el msj
                MessageBox.Show(e + " No hay ningún rostro en la Base de Datos, por favor añadir por lo menos una cara", "Cragar caras en tu Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public void INICIARCACTURA()
        {
            try
            {
                //Inicia la Captura            
                grabber = new Capture();
                grabber.QueryFrame();

                //Inicia el evento FrameGraber
                Application.Idle += new EventHandler(FrameGrabber);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void FrameGrabber(object sender, EventArgs e)
        {
            lblCantidad.Text = "0";
            NamePersons.Add("");
            try
            {

                //Obtener la secuencia del dispositivo de captura
                try
                {
                    currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                }
                catch (Exception)
                {
                    imageBox2.Image = null;
                }

                //Convertir a escala de grises
                gray = currentFrame.Convert<Gray, Byte>();

                //Detector de Rostros
                MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(face, 1.5, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));

                //Accion para cada elemento detectado
                foreach (MCvAvgComp f in facesDetected[0])
                {
                    t = t + 1;
                    result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(640, 480, INTER.CV_INTER_CUBIC);
                    //Dibujar el cuadro para el rostro
                    currentFrame.Draw(f.rect, new Bgr(Color.Blue), 1);

                    NamePersons[t - 1] = name;
                    NamePersons.Add("");
                    //Establecer el nùmero de rostros detectados
                    lblCantidad.Text = facesDetected[0].Length.ToString();
                    //lblNadie.Text = name;

                }
                t = 0;

                //Mostrar los rostros procesados y reconocidos
                imageBox2.Image = currentFrame;
                name = "";
                //Borrar la lista de nombres            
                NamePersons.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Registrar_Load(object sender, EventArgs e)
        {
           
            DA.listaCaras(dataGridView1);


        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
