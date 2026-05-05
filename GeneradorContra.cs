using System;

namespace ProyectoFinalSimuladorSeguridadInformatica
{
    // Se encarga SOLO de generar contraseñas
    public class GeneradorContrasena
    {
         public string Generador(int longitud)
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

            return new string(contrasenaArray);
        }
    }
}