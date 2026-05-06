using System.Text;

namespace ProyectoFinalSimuladorSeguridadInformatica
{
    public class Escaner
    {
        // ─────────────────────────────────────────────
        // LISTAS DE REFERENCIA
        // Definen qué elementos se consideran peligrosos o sospechosos
        // ─────────────────────────────────────────────

        // Extensiones claramente peligrosas (ejecutables, scripts, etc.)
        private readonly string[] DangerousExtensions =
        {
            ".exe", ".bat", ".cmd", ".msi", ".scr", ".pif",
            ".com", ".vbs", ".vbe", ".js",  ".jse", ".wsf",
            ".wsh", ".ps1", ".psm1",".psd1",".reg",
            ".dll", ".sys", ".drv", ".lnk"
        };

        // Extensiones que pueden ser sospechosas (comprimidos o scripts)
        private readonly string[] SuspiciousExtensions =
        {
            ".zip", ".rar", ".7z",  ".tar", ".gz",  ".iso",
            ".dmg", ".pkg", ".deb", ".rpm", ".sh",  ".bash",
            ".py",  ".rb",  ".pl",  ".php", ".apk", ".app"
        };

        // Palabras clave claramente asociadas a malware
        private readonly string[] DangerousKeywords =
        {
            "virus", "hack", "crack", "keygen", "trojan", "malware",
            "ransomware", "spyware", "rootkit", "exploit", "payload",
            "backdoor", "botnet", "worm", "keylogger", "phishing",
            "bypass", "inject", "overflow", "shellcode", "dropper",
            "loader", "stealer", "grabber", "rat", "crypter"
        };

        // Palabras sospechosas (no necesariamente malware, pero dudosas)
        private readonly string[] SuspiciousKeywords =
        {
            "patch", "activat", "serial", "license", "nulled",
            "leaked", "dump", "brute", "flood", "scanner",
            "crawler", "scraper", "autobot", "setup", "gen_"
        };

        // Rutas críticas del sistema (alto riesgo si aparece un archivo ahí)
        private readonly string[] DangerousPaths =
        {
            "C:\\Windows\\System32", "C:\\Program Files", "C:\\Program Files (x86)",
            "C:\\Users\\Public", "C:\\Users\\Default", "C:\\Users\\All Users",
            "C:\\Temp", "C:\\Windows\\Temp", "C:\\Windows\\Prefetch",
            "C:\\Windows\\SysWOW64", "C:\\Windows\\WinSxS"
        };        // Rutas comunes donde suelen descargarse archivos sospechosos

        private readonly string[] SuspiciousPaths =
        {
            "C:\\Downloads", "C:\\Desktop", "C:\\Documents and Settings",
            "C:\\Users\\%USERNAME%\\Downloads", "C:\\Users\\%USERNAME%\\Desktop",
            "C:\\Users\\%USERNAME%\\Documents", "C:\\Users\\%USERNAME%\\AppData\\Local",
            "C:\\Users\\%USERNAME%\\AppData\\Roaming"
        };

        // ─────────────────────────────────────────────
        // DATOS DEL ARCHIVO A ANALIZAR
        // ─────────────────────────────────────────────
        private string ruta;
        private string nombreDirectorio;
        private string nombreArchivo;
        private string extension;
        private long tamanoArchivo;

        // Log donde se guarda todo el análisis paso a paso
        private string logPath = "log_analisis.txt";
        private StringBuilder logBuilder = new StringBuilder();

        // Variable acumulativa del riesgo total
        private int riesgoTotal = 0;

        // ─────────────────────────────────────────────
        // Extrae automáticamente los datos del archivo
        // ─────────────────────────────────────────────

        public void AddRuta(string ruta)
        {
            if (string.IsNullOrEmpty(ruta))
            {
                logBuilder.AppendLine("[ERROR] La ruta proporcionada está vacía o nula.");
                return;
            }
            FileInfo fileInfo = new FileInfo(ruta);

            this.ruta = ruta;
            nombreArchivo = fileInfo.Name;
            extension = fileInfo.Extension;
            tamanoArchivo = fileInfo.Length;
            nombreDirectorio = fileInfo.DirectoryName ?? ""; 
        }
        // ─────────────────────────────────────────────
        // MÉTODO PRINCIPAL DE ANÁLISIS
        // ─────────────────────────────────────────────
        public string Analizar()
        {
            riesgoTotal = 0;
            //Borramos la información del log anterior para empezar uno nuevo
            logBuilder.Clear();

            // Primero comprobamos si la ruta existe
            if (EscanearRuta())
            {
                EvaluarExtension();
                EvaluarNombreDirectorio();
                EvaluarNombre();
                EvaluarTamano();
            }

            // Devuelve resultado final
            return EvaluarSeguridad();

        }

        // ─────────────────────────────────────────────
        // ANÁLISIS DE Ruta
        // ─────────────────────────────────────────────
        protected bool EscanearRuta()
        {
            if (!File.Exists(ruta) && !Directory.Exists(ruta))
            {
                logBuilder.AppendLine($"[ERROR] La ruta '{ruta}' no existe.");
                return false;
            }
            return true;
        }
        // ─────────────────────────────────────────────
        // ANÁLISIS DE EXTENSIÓN
        // ─────────────────────────────────────────────
        protected void EvaluarExtension()
        {
            if (DangerousExtensions.Contains(extension.ToLower()))
            {
                riesgoTotal += 45;
                logBuilder.AppendLine($"[ALTO] Extensión peligrosa: {extension}");
            }
            else if (SuspiciousExtensions.Contains(extension.ToLower()))
            {
                riesgoTotal += 25;
                logBuilder.AppendLine($"[MEDIO] Extensión sospechosa: {extension}");
            }
            else
            {
                riesgoTotal += 2;
                logBuilder.AppendLine($"[BAJO] Extensión normal: {extension}");
            }
        }

        // ─────────────────────────────────────────────
        // ANÁLISIS DE RUTA
        // ─────────────────────────────────────────────
        protected void EvaluarNombreDirectorio()
        {
            //Comprobamos si la ruta del archivo es igual a null, si es así, asignamos una cadena vacía para evitar errores
            string path = nombreDirectorio?.ToLower() ?? "";

            //Comprobamos si nuestra ruta esta en alguno a de los string[], si es así, sumamos el riesgo correspondiente
            // y añadimos una línea al log indicando el resultado
            if (DangerousPaths.Any(p => path.Contains(p.ToLower())))
            {
                riesgoTotal += 35;
                logBuilder.AppendLine("[ALTO] Ruta crítica del sistema");
            }
            else if (SuspiciousPaths.Any(p => path.Contains(p.ToLower())))
            {
                riesgoTotal += 20;
                logBuilder.AppendLine("[MEDIO] Ruta sospechosa");
            }
            else
            {
                riesgoTotal += 2;
                logBuilder.AppendLine("[BAJO] Ruta normal");
            }
        }
        // ───────
        // ──────────────────────────────────────
        // ANÁLISIS DEL NOMBRE DEL ARCHIVO
        // Busca palabras clave típicas de malware
        // ─────────────────────────────────────────────
        protected void EvaluarNombre()
        {
            string lower = nombreArchivo.ToLower();

            bool peligroso = DangerousKeywords.Any(k => lower.Contains(k));
            bool sospechoso = SuspiciousKeywords.Any(k => lower.Contains(k));

            if (peligroso)
            {
                riesgoTotal += 35;
                logBuilder.AppendLine($"[ALTO] Nombre contiene malware keywords");
            }
            else if (sospechoso)
            {
                riesgoTotal += 20;
                logBuilder.AppendLine($"[MEDIO] Nombre sospechoso");
            }
            else
            {
                riesgoTotal += 3;
                logBuilder.AppendLine($"[BAJO] Nombre limpio");
            }
        }
        // ─────────────────────────────────────────────
        // ANÁLISIS DEL TAMAÑO
        // Detecta tamaños anómalos (muy grandes o muy pequeños)
        // ─────────────────────────────────────────────
        protected void EvaluarTamano()
        {
            if (tamanoArchivo > 100 * 1024 * 1024) // Mayor a 100 MB
            {
                logBuilder.AppendLine($"[ALTO] El archivo '{nombreArchivo}' es muy grande ({tamanoArchivo / (1024 * 1024)} MB)");
                riesgoTotal += 27;
            }
            else if (tamanoArchivo > 10 * 1024 * 1024) // Mayor a 10 MB
            {
                logBuilder.AppendLine($"[MEDIO] El archivo '{nombreArchivo}' es grande ({tamanoArchivo / (1024 * 1024)} MB)");
                riesgoTotal += 17;
            }
            else if (tamanoArchivo < 10) // Menor a 10 bytes
            {
                logBuilder.AppendLine($"[ALTO] El archivo '{nombreArchivo}' es muy pequeño ({tamanoArchivo} bytes)");
                riesgoTotal += 20;
            }
            else if (tamanoArchivo < 50) // Menor a 50 bytes
            {
                logBuilder.AppendLine($"[MEDIO] El archivo '{nombreArchivo}' es bastante pequeño ({tamanoArchivo} bytes)");
                riesgoTotal += 10;
            }
            else
            {
                logBuilder.AppendLine($"[BAJO] El archivo '{nombreArchivo}' tiene un tamaño común ({tamanoArchivo / (1024 * 1024)} MB)");
            }
        }

        // ─────────────────────────────────────────────
        // GUARDA EL LOG EN UN ARCHIVO
        // ─────────────────────────────────────────────
        protected void GuardarLog()
        {
            File.WriteAllText(logPath, logBuilder.ToString());
        }
        // Devuelve el log completo
        protected StringBuilder GetLog()
        {
            return logBuilder;
        }
        // ─────────────────────────────────────────────
        // RESULTADO FINAL
        // Clasifica el riesgo según la puntuación acumulada
        // ─────────────────────────────────────────────
        protected string EvaluarSeguridad()
        {

            if (riesgoTotal > 100)
                return $"RIESGO MUY ALTO (100%)";

            if (riesgoTotal < 30)
                return $"BAJO ({riesgoTotal}%)";

            if (riesgoTotal < 70)
                return $"MEDIO ({riesgoTotal}%)";

            return $"ALTO ({riesgoTotal}%)";
        }

    }
}
