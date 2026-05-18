namespace ProyectoFinalSimuladorSeguridadInformatica
{
    // Se encarga SOLO de evaluar la contraseña
    public class AnalizadorContra
    {
        private ReglasSeguridad reglas;

        public AnalizadorContra(ReglasSeguridad reglas)
        {
            this.reglas = reglas;
        }
        public string Analizar(string password)
        {
            if (string.IsNullOrEmpty(password))
                return "Error: la contraseña no puede estar vacía";

            if (reglas.ContieneDiccionario(password))
                return "Débil: contiene palabras comunes";

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
            points = reglas.Repeticiones(password, points);

            password = reglas.NormalizarLeet(password);

            // Detecta secuencias tipo abc, 123, cba
            points = reglas.Secuencias(password, points);

            // Detecta si contiene un año (ej: 1998, 2020)
            points = reglas.Ano(password, points);

            return Evaluar(points);
        }

        // ─────────────────────────────────────────────
        // EVALUACIÓN FINAL
        // Convierte la puntuación numérica en categoría de seguridad
        // ─────────────────────────────────────────────
        public string Evaluar(float puntuacion)
        {
            return puntuacion switch
            {
                <= 2 => "Muy Débil: contiene patrones obvios o poca longitud",
                <= 4 => "Débil: poca variedad de caracteres",
                <= 6 => "Media: contraseña aceptable pero mejorable",
                <= 8 => "Fuerte: buena longitud y variedad de caracteres",
                _ => "Muy Fuerte: contraseña muy segura, difícil de adivinar"
            };
        }
    }
}