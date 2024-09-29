


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
            if (base64String.Contains(","))
            {
                base64String = base64String.Split(',')[1];
            }

            // Convertir la cadena base64 en un array de bytes
            byte[] imageBytes = Convert.FromBase64String(base64String);

            string outputPath = $"{outputPathWithoutExtension}.webp";
            string directory = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using var image = Image.Load(imageBytes);
            image.Save(outputPath, new WebpEncoder());
            //// Crear un stream a partir de los bytes
            //using (var ms = new MemoryStream(imageBytes))
            //{
            //    // Crear una imagen a partir del stream
            //    using (Image image = Image.FromStream(ms))
            //    {
            //        // Obtener el formato de la imagen (JPEG, PNG, etc.)
            //        ImageFormat format = image.RawFormat;

            //        // Asignar la extensión correcta en función del formato
            //        string extension = GetImageExtension(format);
            //        if (extension == null)
            //        {
            //            throw new InvalidOperationException("No se pudo detectar el formato de la imagen.");
            //        }

            //        // Guardar la imagen en el formato detectado
            //        string outputPath = $"{outputPathWithoutExtension}.{extension}";
            //        string directory = Path.GetDirectoryName(outputPath);
            //        if (!Directory.Exists(directory))
            //        {
            //            Directory.CreateDirectory(directory);
            //        }
            //        File.WriteAllBytes(outputPath, imageBytes);
            //    }
            //}
        }

        // Método para obtener la extensión de imagen en base al formato
        private static string GetImageExtension(byte[] imageBytes)
        {
            // Utiliza la clase Image para cargar la imagen desde un stream de bytes
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                // Detecta el formato utilizando los decodificadores registrados
                IImageFormat format = Image.DetectFormat(ms);

                if (format == null)
                {
                    throw new InvalidOperationException("Formato de imagen no detectado.");
                }

                // Retorna el nombre del formato
                return format.Name; // Ejemplo: "JPEG", "PNG", "GIF", etc.
            }
        }

    }
}
