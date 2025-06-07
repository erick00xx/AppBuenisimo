using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBuenisimo.Utils
{
    /// <summary>
    /// Provee una abstracción sobre DateTime.Now para facilitar las pruebas unitarias.
    /// Permite "congelar" el tiempo a un valor específico durante la ejecución de las pruebas.
    /// </summary>
    public static class TimeProvider
    {
        // Esta variable guardará nuestro tiempo personalizado para las pruebas.
        // Si es 'null', usaremos el tiempo real.
        private static DateTime? _customDateTime = null;

        // Tu zona horaria sigue siendo relevante para cuando uses el tiempo real.
        private static readonly TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

        /// <summary>
        /// Obtiene la fecha y hora actual. Devuelve un valor personalizado si se ha establecido uno,
        /// de lo contrario, devuelve la hora real del sistema en la zona horaria configurada.
        /// </summary>
        public static DateTime Now
        {
            get
            {
                // Si hay un tiempo personalizado, devuélvelo. Si no, usa el tiempo real.
                return _customDateTime ?? TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            }
        }

        /// <summary>
        /// Obtiene solo la fecha de la propiedad Now.
        /// </summary>
        public static DateTime Today => Now.Date;

        /// <summary>
        /// (PARA USO EN PRUEBAS) - Fija el tiempo a un valor específico.
        /// </summary>
        /// <param name="customDateTime">La fecha y hora a la que se debe "congelar" el tiempo.</param>
        public static void SetCustomTime(DateTime customDateTime)
        {
            _customDateTime = customDateTime;
        }

        /// <summary>
        /// (PARA USO EN PRUEBAS) - Restaura el comportamiento normal, usando el tiempo real del sistema.
        /// Es crucial llamar a este método en el [TestCleanup] de tus pruebas.
        /// </summary>
        public static void ResetToDefault()
        {
            _customDateTime = null;
        }
    }
}