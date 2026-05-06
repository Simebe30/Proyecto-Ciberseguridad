using System;
using System.Linq;
using System.Collections.Generic;

namespace ProyectoFinalSimuladorSeguridadInformatica
{
    // Se encarga SOLO de reglas de seguridad
    public class ReglasSeguridad
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
        public ReglasSeguridad(List<string> contrasenas)
        {
            contrasenasDicc = contrasenas;
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
                    .Replace("€", "e")
                    .Replace("1", "i")
                    .Replace("!", "i")
                    .Replace("0", "o")
                    .Replace("9", "g")
                    .Replace("5", "s")
                    .Replace("$", "s")
                    .Replace("7", "t");
        }

        // ─────────────────────────────────────────────
        // COMPROBACIÓN DE DICCIONARIO
        // Detecta si la contraseña contiene palabras débiles conocidas
        // ─────────────────────────────────────────────
        public bool ContieneDiccionario(string password)
        {
            password = NormalizarLeet(password);

            return contrasenasDicc.Any(p => password.ToLower().Contains(p));
        }


        // ─────────────────────────────────────────────
        // DETECCIÓN DE AÑOS
        // Penaliza si aparece un año típico (1900 - año actual)
        // ─────────────────────────────────────────────
        public float Ano(string password, float points)
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
        public float Repeticiones(string password, float points)
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
        public float Secuencias(string password, float points)
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
    }
}
