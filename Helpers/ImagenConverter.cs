


using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;

namespace ApiBRD.Helpers
{
    public static class ImagenConverter
    {
        public static void ConvertBase64ToImage(string base64String, string outputPathWithoutExtension)
        {
            // Eliminar encabezado data:image si está presente
            if (base64String.Contains(','))
            {
                base64String = base64String.Split(',')[1];
            }

            // Convertir la cadena base64 en un array de bytes
            byte[] imageBytes = Convert.FromBase64String(base64String);

            string outputPath = $"{outputPathWithoutExtension}.webp";
            string directory = Path.GetDirectoryName(outputPath)??throw new Exception("Error getting image directory");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using var image = Image.Load(imageBytes);
            image.Save(outputPath, new WebpEncoder());
        }
    }
}
