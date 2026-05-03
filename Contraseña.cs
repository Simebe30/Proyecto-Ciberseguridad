using System.Linq;

namespace ProyectoFinalSimuladorSeguridadInformatica
{
    public class Contraseña
    {
        //  Datos internos
        List<string> contraseñasDicc = new List<string>();

        //  Configuración del diccionario
        public void SetContraseñasDicc(List<string> contraseñas)
        {
            contraseñasDicc = contraseñas;
        }

        //  Generador de contraseñas
        public char[] Generador(int longitud)
        {
            string caracteres = "!@#$%^&*()_+-=[]{}|;':,.<>/?" +
                                "abcdefghijklmnopqrstuvwxyz0123456789";

            Random rnd = new Random();
            char[] contraseñaArray = new char[longitud];

            for (int i = 0; i < longitud; i++)
            {
                contraseñaArray[i] = caracteres[rnd.Next(caracteres.Length)];
            }

            return contraseñaArray;
        }

        //  Analizador principal
        public string Analizador(string password)
        {
            if (breakDiccionario(password) == 0 || string.IsNullOrEmpty(password))
                return "Débil";

            float puntos = fortalezaChar(password);
            return fortalezaEvaluacion(puntos);
        }

        public string NormalizarLeet(string password)
        {
            return password.ToLower()
                    .Replace("@", "a")
                    .Replace("4", "a")
                    .Replace("3", "e")
                    .Replace("1", "i")
                    .Replace("0", "o")
                    .Replace("$", "s")
                    .Replace("7", "t");
        }

        //  Reglas de diccionario
        public int breakDiccionario(string password)
        {
            password = NormalizarLeet(password);
            if(contraseñasDicc.Any(p=> password.ToLower().Contains(p)))
            {
                return 0;
            }
            return 1;
        }

        //  Cálculo de fortaleza
        public float fortalezaChar(string password)
        {
            float points = 1;

            // longitud
            if (password.Length >= 16) points += 3;
            else if (password.Length >= 12) points += 2;
            else if (password.Length >= 8) points += 1;

            // tipos de caracteres
            if (password.Any(char.IsLower)) points++;
            if (password.Any(char.IsUpper)) points++;
            if (password.Any(char.IsDigit)) points++;
            if (password.Any(c => !char.IsLetterOrDigit(c))) points++;

            // penalizaciones
            if (password.Any(char.IsWhiteSpace)) points--;
            if (password.Any(char.IsControl)) points--;

            if (char.IsUpper(password.First()) &&
                char.IsDigit(password.Last()))
                points--;

            // reglas avanzadas

            points = caracteresRepetidos(password, points);

            password = NormalizarLeet(password);

            points = tieneSecuencia(password, points);
            points = tieneAño(password, points);

            return points;
        }
        // Tiene año tipico
        public float tieneAño(string password, float points){
            string nums = new string(password.Where(char.IsDigit).ToArray());

            if (int.TryParse(nums, out int numero))
            {
                if (numero >= 1900 && numero <= DateTime.Now.Year)
                    points--;
            }
            return points;
        }


        //  Repetidos
        public float caracteresRepetidos(string password, float points)
        {
            float restar = 1.0f / password.Length;

            for (int i = 0; i < password.Length - 1; i++)
            {
                if (password[i] == password[i + 1])
                    points -= restar;
            }

            return points;
        }

        // Secuencias
        public float tieneSecuencia(string password, float points)
        {
            float restar = 1.0f / password.Length;
            bool asc, desc;

            for (int i = 0; i < password.Length - 2 ; i++)
            {
                asc =
                    password[i] + 1 == password[i + 1] &&
                    password[i + 1] + 1 == password[i + 2];

                desc =
                   password[i] == password[i + 1] + 1 &&
                    password[i + 1] == password[i + 2] + 1;

                if (asc || desc)
                {
                    points -= restar;
                }
               
            }

            return points;
        }

        // Evaluación final
        public string fortalezaEvaluacion(float puntuacion)
        {
            return puntuacion switch
            {
                <= 3 => "Débil",
                <= 6 => "Media",
                _ => "Fuerte"
            };
        }
    }
}