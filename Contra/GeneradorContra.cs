using System;

namespace ProyectoFinalSimuladorSeguridadInformatica
{
    // Clase encargada únicamente de generar contraseñas
    public class GeneradorContra
    {
        // Una sola instancia de Random para toda la clase
        private Random rnd = new Random();

        // Método principal que genera la contraseña
        public string Generar(int longitud)
        {
            // Conjuntos de caracteres disponibles
            string minusculas = "abcdefghijklmnopqrstuvwxyz";
            string mayusculas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string numeros = "0123456789";
            string simbolos = "!@#$%^&*()_+-=[]{}|;':,.<>/?";

            // Unión de todos los caracteres
            string caracteres = minusculas + mayusculas + numeros + simbolos;

            // Validación de longitud mínima
            if (longitud < 8)
            {
                return "Error: la longitud mínima recomendada es de 8 caracteres";
            }

            // Array donde se construirá la contraseña
            char[] password = new char[longitud];

            // Garantiza variedad:
            password[0] = LetraAleatoria(minusculas);
            password[1] = LetraAleatoria(mayusculas);
            password[2] = LetraAleatoria(numeros);
            password[3] = LetraAleatoria(simbolos);

            // Completa el resto de caracteres aleatoriamente
            for (int i = 4; i < longitud; i++)
            {
                password[i] = caracteres[rnd.Next(caracteres.Length)];
            }

            // Mezcla el orden de los caracteres
            Random.Shared.Shuffle(password);

            // Convierte el array en string y lo devuelve
            return new string(password);
        }

        // Devuelve un carácter aleatorio de un conjunto dado
        public char LetraAleatoria(string caracteres)
        {
            return caracteres[rnd.Next(caracteres.Length)];
        }
    }
}