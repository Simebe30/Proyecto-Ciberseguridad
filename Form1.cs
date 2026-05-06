using System.Text.Json;

namespace ProyectoFinalSimuladorSeguridadInformatica
{
    public partial class Form1 : Form
    {
        public Escaner Escaner { get; set; }
        public Form1()
        {
            InitializeComponent();
            Escaner = new Escaner();
            Execute();
        }

        private void Execute()
        {


            string jsonString = File.ReadAllText("contrasTipicas.json");
            List<string> diccionario = JsonSerializer.Deserialize<List<string>>(jsonString);

            if (diccionario == null || diccionario.Count == 0)
            {
                Console.WriteLine("Aviso: no hay diccionario cargado"); return;
            }

            ReglasSeguridad reglas = new(diccionario);
            AnalizadorContra analizador = new(reglas);
            GeneradorContra generador = new();

            // Generar
            string password = generador.Generar(12);
            Console.WriteLine("Generada: " + password);

            // Analizar
            string resultado = analizador.Analizar(password);
            Console.WriteLine("Resultado: " + resultado);



            //Escaner
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Seleccionar archivo",
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string rutaArchivo = openFileDialog.FileName;
                Escaner.AddRuta(rutaArchivo);
                Console.WriteLine(Escaner.Analizar());
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

}



