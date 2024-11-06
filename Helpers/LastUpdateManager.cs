using System.Text.Json;

namespace ApiBRD.Helpers
{
    public static class LastUpdateManager
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lastUpdate.json");

        // Estructura de datos para almacenar las fechas de actualización
        public class UpdateData
        {
            public DateTime Categorias { get; set; }
            public DateTime Productos { get; set; }
        }

        // Método para actualizar la fecha de actualización de categorías
        public static void UpdateCategorias()
        {
            var data = LoadData();
            data.Categorias = DateTime.Now;
            SaveData(data);
        }

        // Método para actualizar la fecha de actualización de productos
        public static void UpdateProductos()
        {
            var data = LoadData();
            data.Productos = DateTime.Now;
            SaveData(data);
        }

        // Método privado para cargar el JSON desde el archivo
        public static UpdateData LoadData()
        {
            if (!File.Exists(FilePath))
            {
                return new UpdateData { Categorias = DateTime.MinValue, Productos = DateTime.MinValue };
            }

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<UpdateData>(json) ?? new UpdateData();
        }

        // Método privado para guardar los datos en el archivo JSON
        private static void SaveData(UpdateData data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
    }
}
