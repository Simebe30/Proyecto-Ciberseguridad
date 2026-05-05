using System.Linq;

namespace ProyectoFinalSimuladorSeguridadInformatica
{
    public class Contrasena
    {
        // ─────────────────────────────────────────────
        // LISTA INTERNA DE CONTRASEÑAS DEL DICCIONARIO
        // Se usa para detectar contraseñas débiles basadas en palabras comunes
        // ─────────────────────────────────────────────
        List<string> contrasenasDicc = new List<string>();

        // ─────────────────────────────────────────────
        // CARGA DEL DICCIONARIO
        // Permite asignar una lista externa de contraseñas débiles conocidas
        // ─────────────────────────────────────────────
        public void SetContrasenasDicc(List<string> contrasenas)
        {
            contrasenasDicc = contrasenas;
        }

        // ─────────────────────────────────────────────
        // GENERADOR DE CONTRASEÑAS ALEATORIAS
        // Crea una contraseña aleatoria a partir de caracteres, números y símbolos
        // ─────────────────────────────────────────────
        public char[] Generador(int longitud)
        {
            string caracteres = "!@#$%^&*()_+-=[]{}|;':,.<>/?" +
                                "abcdefghijklmnopqrstuvwxyz0123456789";

            Random rnd = new Random();
            char[] contrasenaArray = new char[longitud];

            // Rellena la contraseña carácter a carácter de forma aleatoria
            for (int i = 0; i < longitud; i++)
            {
                contrasenaArray[i] = caracteres[rnd.Next(caracteres.Length)];
            }

            return contrasenaArray;
        }

        // ─────────────────────────────────────────────
        // ANALIZADOR PRINCIPAL DE CONTRASEÑAS
        // Decide si una contraseña es débil, media o fuerte
        // ─────────────────────────────────────────────
        public string Analizador(string password)
        {
            // Si es nula, vacía o aparece en el diccionario → es débil directamente
            if (breakDiccionario(password) == 0 || string.IsNullOrEmpty(password))
                return "Débil";

            // Calcula puntuación de seguridad
            float puntos = fortalezaChar(password);

            // Convierte la puntuación en categoría final
            return fortalezaEvaluacion(puntos);
        }

        // ─────────────────────────────────────────────
        // NORMALIZACIÓN LEET SPEAK
        // Convierte símbolos comunes en letras (ej: @ → a)
        // para evitar que se “camuflen” palabras débiles
        // ─────────────────────────────────────────────
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

        // ─────────────────────────────────────────────
        // COMPROBACIÓN DE DICCIONARIO
        // Detecta si la contraseña contiene palabras débiles conocidas
        // ─────────────────────────────────────────────
        public int breakDiccionario(string password)
        {
            password = NormalizarLeet(password);

            if (contrasenasDicc.Any(p => password.ToLower().Contains(p)))
            {
                return 0; // contraseña insegura
            }

            return 1; // contraseña aceptable
        }

        public float fortalezaChar(string password)
        {
            float points = 1;

            // ─────────────────────────────────────────────
            // LONGITUD DE LA CONTRASEÑA
            // Más longitud = más seguridad (más difícil de adivinar o atacar)
            // ─────────────────────────────────────────────
            if (password.Length >= 16) points += 3;
            else if (password.Length >= 12) points += 2;
            else if (password.Length >= 8) points += 1;

            // ─────────────────────────────────────────────
            // LETRAS MINÚSCULAS
            // Aumentan la variedad de caracteres
            // ─────────────────────────────────────────────
            if (password.Any(char.IsLower)) points++;

            // ─────────────────────────────────────────────
            // LETRAS MAYÚSCULAS
            // Añaden complejidad a la contraseña
            // ─────────────────────────────────────────────
            if (password.Any(char.IsUpper)) points++;

            // ─────────────────────────────────────────────
            // NÚMEROS
            // Incrementan la dificultad de predicción
            // ─────────────────────────────────────────────
            if (password.Any(char.IsDigit)) points++;

            // ─────────────────────────────────────────────
            // CARACTERES ESPECIALES (símbolos)
            // Ej: @, #, !, %, etc.
            // Hacen la contraseña mucho más difícil de romper
            // ─────────────────────────────────────────────
            if (password.Any(c => !char.IsLetterOrDigit(c))) points++;

            // ─────────────────────────────────────────────
            // ESPACIOS EN BLANCO
            // Normalmente no se usan en contraseñas seguras
            // Pueden indicar errores o mala práctica
            // ─────────────────────────────────────────────
            if (password.Any(char.IsWhiteSpace)) points--;

            // ─────────────────────────────────────────────
            // CARACTERES DE CONTROL
            // Ej: saltos de línea, tabs, etc.
            // No aportan seguridad y pueden ser anomalías
            // ─────────────────────────────────────────────
            if (password.Any(char.IsControl)) points--;

            // ─────────────────────────────────────────────
            // PATRÓN TÍPICO DÉBIL
            // Ejemplo: "Password1"
            // Empieza en mayúscula y termina en número
            // Es un formato muy común y fácil de adivinar
            // ─────────────────────────────────────────────
            if (char.IsUpper(password.First()) &&
                char.IsDigit(password.Last()))
                points--;

            // ─────────────────────────────────────────────
            // REGLAS AVANZADAS
            // Análisis más profundo de patrones débiles
            // ─────────────────────────────────────────────

            // Penaliza repeticiones seguidas (aaaa, 1111)
            points = caracteresRepetidos(password, points);

            password = NormalizarLeet(password);

            // Detecta secuencias tipo abc, 123, cba
            points = tieneSecuencia(password, points);

            // Detecta si contiene un año (ej: 1998, 2020)
            points = tieneAno(password, points);

            return points;
        }

        // ─────────────────────────────────────────────
        // DETECCIÓN DE AÑOS
        // Penaliza si aparece un año típico (1900 - año actual)
        // ─────────────────────────────────────────────
        public float tieneAno(string password, float points)
        {
            string nums = new string(password.Where(char.IsDigit).ToArray());

            if (int.TryParse(nums, out int numero))
            {
                if (numero >= 1900 && numero <= DateTime.Now.Year)
                    points--;
            }

            return points;
        }

        // ─────────────────────────────────────────────
        // DETECCIÓN DE CARACTERES REPETIDOS
        // Penaliza repeticiones consecutivas (ej: "aaaa", "1111")
        // ─────────────────────────────────────────────
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

        // ─────────────────────────────────────────────
        // DETECCIÓN DE SECUENCIAS
        // Penaliza patrones como "abc", "123" o "cba"
        // ─────────────────────────────────────────────
        public float tieneSecuencia(string password, float points)
        {
            float restar = 1.0f / password.Length;
            bool asc, desc;

            for (int i = 0; i < password.Length - 2; i++)
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

        // ─────────────────────────────────────────────
        // EVALUACIÓN FINAL
        // Convierte la puntuación numérica en categoría de seguridad
        // ─────────────────────────────────────────────
        public string fortalezaEvaluacion(float puntuacion)
        {
            return puntuacion switch
            {
                <= 3 => "Debil",
                <= 6 => "Media",
                _ => "Fuerte"
            };
        }
    }
}