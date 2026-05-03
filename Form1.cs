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
            Contraseña contraseña = new Contraseña();
            string contra = new string(contraseña.Generador(30));

            string jsonString = File.ReadAllText("contrasTipicas.json");
            List<string> contraseñasTipicas = JsonSerializer.Deserialize<List<string>>(jsonString);

            if (contraseñasTipicas == null)
            {
                Console.WriteLine("No se pudieron deserializar las contraseñas típicas.");
            }
            else
            {
                contraseña.SetContraseñasDicc(contraseñasTipicas);
            }

            contraseña.Analizador("p@ss");

            Console.WriteLine(contraseña.Analizador("cba"));
            Console.WriteLine(contraseña.Analizador("12345"));
            contraseña.Analizador("11111");
            contraseña.Analizador("password");
            contraseña.Analizador(contra);

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



