using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Renocimiento_facial_Tu_Codigo
{
   public static class DA
    {



        private static OleDbConnection conn;
        public static string[] Name;
        private static byte[] face;
        public  static List<byte[]> Face = new List<byte[]>();
        public static int TotalUser;
        public static void DBCon()
        {
            //conn = new OleDbConnection("Provider = Microsoft.Jet.OLEDB.4.0; Data Source = UsersFace.mdb");
            conn = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = BD.accdb");

            conn.Open();
        }

        public static string cnx()
        {
            string m = "";
            conn = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = BD.accdb");
            try
            {
                conn.Open();
                m = "Conexion exitosa";
            }
            catch (Exception f)
            {
                m = f.ToString();
                
            }
            finally { conn.Close(); }
            return m;
        }
        public static bool GuardarImagen(string Name,  byte[] abImagen)
        {
            conn.Open();
            OleDbCommand comm = new OleDbCommand("INSERT INTO Caras (Nombre,Imagen) VALUES ('" + Name + "',?)", conn);
            OleDbParameter parImagen = new OleDbParameter("@Imagen", OleDbType.VarBinary, abImagen.Length);
            parImagen.Value = abImagen;
            comm.Parameters.Add(parImagen);
            int iResultado = comm.ExecuteNonQuery();
            conn.Close();
            return Convert.ToBoolean(iResultado);
        }

        public static DataTable ObtenerBytesImagen()
        {
            string sql = "SELECT Id,Nombre,Imagen FROM Caras";
            OleDbCommand comm = new OleDbCommand(sql, conn);

            OleDbDataAdapter adaptador = new OleDbDataAdapter(comm);
            DataTable dt = new DataTable();
            adaptador.Fill(dt);
            int cont = dt.Rows.Count;
            Name = new string[cont];

            for (int i = 0; i < cont; i++)
            {
                Name[i] = dt.Rows[i]["Nombre"].ToString();
                face = (byte[])dt.Rows[i]["Imagen"];
                Face.Add(face);
            }
            TotalUser = dt.Rows.Count;
            conn.Close();
            return dt;
        }

        
        public static  byte[] ConvertImgToBinary(Image Img)
        {
            Bitmap bmp = new Bitmap(Img);
            MemoryStream MyStream = new MemoryStream();
            bmp.Save(MyStream, System.Drawing.Imaging.ImageFormat.Bmp);

            byte[] abImagen = MyStream.ToArray();

            
            

            return abImagen;
        }

        public static  Image ConvertByteToImg(int con)
        {
            Image FetImg;
            byte[] img = Face[con];
            MemoryStream ms = new MemoryStream(img);
            FetImg = Image.FromStream(ms);
            ms.Close();
            return FetImg;

        }


        //////////////////////////
        public static void listaCaras(DataGridView data)
        {
            conn.Open();
            OleDbCommand comando = new OleDbCommand("SELECT * FROM Caras", conn);
            comando.Connection = conn;
            comando.ExecuteNonQuery();
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter(comando);
            da.Fill(dt);
            data.DataSource = dt;
            data.Columns[0].Width = 60;
            data.Columns[1].Width = 165;
            data.Columns[2].Width = 165;
            int cont= data.RowCount;
            int i;
            for (i = 0; i < cont; i++)
            {
                data.Rows[i].Height = 110;

            }
            

            conn.Close();
        }

        public static void eliminar(int Id)
        {
            DialogResult resultado = MessageBox.Show("¿Estas Seguro de Eliminar el Registro Seleccionado?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado == DialogResult.No)
            {
                return;
            }
            OleDbCommand cmd = new OleDbCommand ("DELETE FROM Caras WHERE ID ="+ Id, conn);


            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            MessageBox.Show("Borrado Exitoso", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        public static void EditarImagen(int id, string Name, byte[] abImagen)
        {
            DialogResult resultado = MessageBox.Show("¿Estas Seguro de Editar el Registro Seleccionado?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado == DialogResult.No)
            {
                return;
            }
            conn.Open();
            OleDbCommand comm = new OleDbCommand( string.Format("UPDATE caras set  Nombre='"+Name +"',Imagen=?  where id="+id+""), conn);
            OleDbParameter parImagen = new OleDbParameter("@Imagen", OleDbType.VarBinary, abImagen.Length);
            parImagen.Value = abImagen;
            comm.Parameters.Add(parImagen);
            try
            {
                comm.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Cambios guardados con Exito", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception)
            {

                throw;
               
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
