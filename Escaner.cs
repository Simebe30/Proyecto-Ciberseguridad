using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinalSimuladorSeguridadInformatica
{
    internal class Escaner
    {
        // ── Listas de referencia ─────────────────────────────────

        private readonly string[] DangerousExtensions = 
        {
            ".exe", ".bat", ".cmd", ".msi", ".scr", ".pif",
            ".com", ".vbs", ".vbe", ".js",  ".jse", ".wsf",
            ".wsh", ".ps1", ".psm1",".psd1",".reg",
            ".dll", ".sys", ".drv", ".lnk"
        };

        private readonly string[] SuspiciousExtensions =
        {
            ".zip", ".rar", ".7z",  ".tar", ".gz",  ".iso",
            ".dmg", ".pkg", ".deb", ".rpm", ".sh",  ".bash",
            ".py",  ".rb",  ".pl",  ".php", ".apk", ".app"
        };


        private readonly string[] DangerousKeywords =
        {
            "virus", "hack", "crack", "keygen", "trojan", "malware",
            "ransomware", "spyware", "rootkit", "exploit", "payload",
            "backdoor", "botnet", "worm", "keylogger", "phishing",
            "bypass", "inject", "overflow", "shellcode", "dropper",
            "loader", "stealer", "grabber", "rat", "crypter"
        };

        private readonly string[] SuspiciousKeywords = 
        {
            "patch", "activat", "serial", "license", "nulled",
            "leaked", "dump", "brute", "flood", "scanner",
            "crawler", "scraper", "autobot", "setup", "gen_"
        };
        private readonly string[] DangerousPaths = 
        {
            "C:\\Windows\\System32", "C:\\Program Files", "C:\\Program Files (x86)",
            "C:\\Users\\Public", "C:\\Users\\Default", "C:\\Users\\All Users",
            "C:\\Temp", "C:\\Windows\\Temp", "C:\\Windows\\Prefetch",
            "C:\\Windows\\SysWOW64", "C:\\Windows\\WinSxS"
        };
        private readonly string[] SuspiciousPaths = 
        {
            "C:\\Downloads", "C:\\Desktop", "C:\\Documents and Settings",
            "C:\\Users\\%USERNAME%\\Downloads", "C:\\Users\\%USERNAME%\\Desktop",
            "C:\\Users\\%USERNAME%\\Documents", "C:\\Users\\%USERNAME%\\AppData\\Local",
            "C:\\Users\\%USERNAME%\\AppData\\Roaming"
        };
        
        private string ruta;
        private string nombreDirectorio;
        private string nombreArchivo;
        private string extension;
        private long tamanoArchivo;
        private string logPath = "log_analisis.txt";
        private  StringBuilder logBuilder = new StringBuilder();
        private  int riesgoTotal = 0;

        public Escaner(string ruta)
        {
            FileInfo fileInfo = new FileInfo(ruta);
            this.ruta = ruta;
            this.nombreArchivo = fileInfo.Name;
            this.extension = fileInfo.Extension;
            this.tamanoArchivo = fileInfo.Length;
            this.nombreDirectorio = fileInfo.DirectoryName;
        }
        public string analizar()
        {
            riesgoTotal = 0;
            logBuilder.Clear();

            if (EscanearRuta())
            {
                EvaluarExtension();
                EvaluarnombreDirectorio();
                EvaluarNombre();
                EvaluarTamano();
            }

            return EvaluarSeguridad();

        }
        public bool EscanearRuta()
        {
            if(!File.Exists(ruta) && !Directory.Exists(ruta))
            {
                logBuilder.AppendLine($"[ERROR] La ruta '{ruta}' no existe.");
                return false;
            }
            return true;
        }
        public void EvaluarExtension()
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
        public void EvaluarnombreDirectorio()
        {
            string path = nombreDirectorio?.ToLower() ?? "";

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

        public void EvaluarNombre()
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
        public void EvaluarTamano()
        {
            if (tamanoArchivo > 100 * 1024 * 1024) // Mayor a 100 MB
            {
                logBuilder.AppendLine($"[ALTO] El archivo '{nombreArchivo}' es muy grande ({tamanoArchivo / (1024 * 1024)} MB)");
                riesgoTotal += 27;
            }
            else if (tamanoArchivo > 10 * 1024 * 1024 ) // Mayor a 10 MB
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
         public void GuardarLog()
        {
            File.WriteAllText(logPath, logBuilder.ToString());
        }
        public StringBuilder getLog()
        {
            return logBuilder;
        }
        public string EvaluarSeguridad()
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
