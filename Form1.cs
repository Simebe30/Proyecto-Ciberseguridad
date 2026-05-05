using System.IO;
using System.Text.Json;

namespace ProyectoFinalSimuladorSeguridadInformatica
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            execute();

        }

        private void execute()
        {
            Contrasena contrasena = new Contrasena();
            string contra = new string(contrasena.Generador(30));

            string jsonString = File.ReadAllText("contrasTipicas.json");
            List<string> contrasenasTipicas = JsonSerializer.Deserialize<List<string>>(jsonString);

            if (contrasenasTipicas == null)
            {
                Console.WriteLine("No se pudieron deserializar las contrasenas tnpicas.");
            }
            else
            {
                contrasena.SetContrasenasDicc(contrasenasTipicas);
            }

            contrasena.Analizador("p@ss");

            Console.WriteLine(contrasena.Analizador("cba"));
            Console.WriteLine(contrasena.Analizador("12345"));
            Console.WriteLine(contrasena.Analizador("mfadsiof8492htwe0p9g8psd"));
            contrasena.Analizador("11111");
            contrasena.Analizador("password");
            contrasena.Analizador(contra);

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Seleccionar archivo",            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string rutaArchivo = openFileDialog.FileName;

                Escaner escaner = new Escaner(rutaArchivo);
                escaner.analizar();
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

}



