
using System.Drawing;
using System.Drawing.Imaging;

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

            // Crear un stream a partir de los bytes
            using (var ms = new MemoryStream(imageBytes))
            {
                // Crear una imagen a partir del stream
                using (Image image = Image.FromStream(ms))
                {
                    // Obtener el formato de la imagen (JPEG, PNG, etc.)
                    ImageFormat format = image.RawFormat;

                    // Asignar la extensión correcta en función del formato
                    string extension = GetImageExtension(format);
                    if (extension == null)
                    {
                        throw new InvalidOperationException("No se pudo detectar el formato de la imagen.");
                    }

                    // Guardar la imagen en el formato detectado
                    string outputPath = $"{outputPathWithoutExtension}.{extension}";
                    string directory = Path.GetDirectoryName(outputPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    image.Save(outputPath);
                }
            }
        }

        // Método para obtener la extensión de imagen en base al formato
        private static string GetImageExtension(ImageFormat format)
        {
            if (ImageFormat.Jpeg.Equals(format))
            {
                return "jpg";
            }
            else if (ImageFormat.Png.Equals(format))
            {
                return "png";
            }
            else if (ImageFormat.Gif.Equals(format))
            {
                return "gif";
            }
            else if (ImageFormat.Bmp.Equals(format))
            {
                return "bmp";
            }
            else if (ImageFormat.Tiff.Equals(format))
            {
                return "tiff";
            }
            else
            {
                return null; // Formato desconocido
            }
        }

    }
}
