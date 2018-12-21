using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Speech.Synthesis;
using System.Runtime.InteropServices;
using AForge.Video.DirectShow;
using AForge.Video;



namespace Renocimiento_facial_Tu_Codigo
{
    public partial class frmReconocedorF : Form
    {
        //CODIGO PARA VER WEBCAM
        private bool ExisteDispositivo = false;
        private FilterInfoCollection DispositivoDeVideo;
        private VideoCaptureDevice FuenteDeVideo = null;

        MJPEGStream strem;



        public int heigth, width;
        public string[] Labels;
        int con = 0;        
        SpeechSynthesizer vos = new SpeechSynthesizer();
        //DECLARANDO TODAS LAS VARIABLES, vectores y  haarcascades
        Image<Bgr, Byte> currentFrame;
        Capture grabber;
        HaarCascade face;
        HaarCascade eye;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.4d, 0.4d);
        Image<Gray, byte> result = null;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> NamePersons = new List<string>();
        int ContTrain, NumLabels, t;
        string name, names = null;

        
        public frmReconocedorF()
        {
            InitializeComponent();
            
            heigth = this.Height; width = this.Width;
            //GARGAMOS LA DETECCION DE LAS CARAS POR  haarcascades 
            face = new HaarCascade("haarcascade_frontalface_default.xml");
            try
            {
              DA.ObtenerBytesImagen();
                //carga de caras y etiquetas para cada imagen               
                string[] Labels = DA.Name;
                NumLabels = DA.TotalUser;
                ContTrain = NumLabels;


                for (int tf = 0; tf < NumLabels; tf++)
                {
                    con = tf;
                    Bitmap bmp = new Bitmap(DA.ConvertByteToImg(con));
                    //LoadFaces = "face" + tf + ".bmp";
                    trainingImages.Add(new Image<Gray, byte>(bmp));//cargo la foto con ese nombre
                    labels.Add(Labels[tf]);//cargo el nombre que se encuentre en la posicion del tf
                   
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e + "No hay ningun rosto registrado).", "Cargar rostros", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void Reconocer()
        {
            try
            {
                //Iniciar el dispositivo de captura
                
                grabber = new Capture();
                grabber.QueryFrame();
                //Iniciar el evento FrameGraber
                Application.Idle += new EventHandler(FrameGrabber);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Application.Idle -= new EventHandler(FrameGrabber);//Detenemos el evento de captura
                grabber.Dispose();//Dejamos de usar la clase para capturar usar los dispositivos
                                 
            }
            catch 
            {
               
            }
            Application.Exit();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void btnRegistrar_Click_1(object sender, EventArgs e)
        {



            Registrar f = new Registrar();
            try
            {
                Application.Idle -= new EventHandler(FrameGrabber);//Detenemos el evento de captura
                grabber.Dispose();//Dejamos de usar la clase para capturar usar los dispositivos
                                  //imageBox1.BackgroundImage = a.Image=  Properties.Resources.Tu_Codigo;//reiniciamos la imagen del control

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            f.Show();
            this.Close();


        }

        private void cbxDispositivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            
            
           

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void FrameGrabber(object sender, EventArgs e)
        {
            lblCantidad.Text = "0";
            NamePersons.Add("");
            try
            {

                currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                //Convertir a escala de grises
                gray = currentFrame.Convert<Gray, Byte>();

                //Detector de Rostros
                MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(face, 1.5, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));
                //1.2
                //Accion para cada elemento detectado
                foreach (MCvAvgComp f in facesDetected[0])
                {
                    t = t + 1;
                    result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, INTER.CV_INTER_CUBIC);
                    //Dibujar el cuadro para el rostro
                    currentFrame.Draw(f.rect, new Bgr(Color.Blue), 1);

                    if (trainingImages.ToArray().Length != 0)
                    {
                        //Clase para reconocimiento con el nùmero de imagenes
                        MCvTermCriteria termCrit = new MCvTermCriteria(ContTrain, 0.88);
                        //0.001
                        //Clase Eigen para reconocimiento de rostro
                        EigenObjectRecognizer recognizer = new EigenObjectRecognizer(trainingImages.ToArray(), labels.ToArray(), ref termCrit);
                        var fa = new Image<Gray, byte>[trainingImages.Count];
                         
                        name = recognizer.Recognize(result);
                        
                        //Dibujar el nombre para cada rostro detectado y reconocido
                        currentFrame.Draw(name, ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.Blue));
                    }



                    NamePersons[t - 1] = name;
                    NamePersons.Add("");
                    //Establecer el nùmero de rostros detectados
                    lblCantidad.Text = facesDetected[0].Length.ToString();
                    lblNombre.Text = name;

                }
                t = 0;

                //Nombres concatenados de todos los rostros reconocidos
                for (int nnn = 0; nnn < facesDetected[0].Length; nnn++)
                {
                    names = names + NamePersons[nnn] + ", ";
                }
                
                //Mostrar los rostros procesados y reconocidos
                imageBox1.Image = currentFrame;
                //lblNombre.Text = names;
                name = "";
                //Borrar la lista de nombres            
                NamePersons.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

      
        private void frmReconocedorF_Load(object sender, EventArgs e)
        {
            
      
            
            Reconocer();
        }
        
        
    }
}
