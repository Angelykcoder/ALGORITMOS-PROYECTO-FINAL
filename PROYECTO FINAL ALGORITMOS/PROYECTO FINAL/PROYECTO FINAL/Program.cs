using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PROYECTO_FINAL_PROFE_PASEME_USTED_ES_MI_FAV
{
    //DEFINIMOS LA CLASE PRODUCTOS
    class PRODUCTOS
    {
        public string Nombre;
        public double precio;
        public int cantidad;

        public string ToCsvLine()
        {
            return $"{Nombre};{precio.ToString(CultureInfo.InvariantCulture)};{cantidad}";
        }

        public override string ToString()
        {
            return $"Nombre: {Nombre} | Precio: {precio.ToString("F2", CultureInfo.InvariantCulture)} | Cantidad: {cantidad}";
        }
    }
    //CREAMOS EL PROGRAMA
    internal class Program
    {
        //AQUI CREO EL ARCHIVO Y LO DEFINO
        //USTED DEBE INDICAR LA RUTA EN QUE DESEA QUE SE CREE EL ARCHIVO PUEDE SER C:\Users\NOMBREDEUSUARIO\Desktop
        static string archivoInventario = "C:\\Users\\memef\\Desktop\\PRUEBAINVENTARIO.txt";

        static void Main(string[] args)
        {
            List<PRODUCTOS> inventario = CargarInventario();

            bool continuar = true;
            while (continuar)
            //EL MENÚ
            {
                Console.Clear();
                Console.WriteLine("BIENVENIDO AL MENÚ - PROYECTO FINAL ALGORITMOS");
                Console.WriteLine("Selecciona la opción que deseas:");
                Console.WriteLine("1. Ingresar nuevo producto y precio (agrega/suma si ya existe)");
                Console.WriteLine("2. Buscar producto (por nombre)");
                Console.WriteLine("3. Salidas de producto (descontar stock)");
                Console.WriteLine("4. Ingreso de producto (sumar stock)");
                Console.WriteLine("5. Inventario actual (mostrar todo)");
                Console.WriteLine("6. Ordenar registros (SelectionSort / QuickSort)");
                Console.WriteLine("7. Modificar producto (nombre/precio/cantidad)");
                Console.WriteLine("8. Eliminar producto");
                Console.WriteLine("9. Ordenar el inventario de forma: (mayor/menor/promedio)");
                Console.WriteLine("10. Inventario Final (desde archivo)");
                Console.WriteLine("0. Salir");
                Console.Write("Opción: ");
                string opcion = Console.ReadLine();
                //SELECTIVA
                switch (opcion)
                {
                    case "1":
                        AgregarProducto(inventario);
                        GuardarInventario(inventario);
                        break;
                    case "2":
                        BuscarProducto(inventario);
                        break;
                    case "3":
                        SalidaProducto(inventario);
                        GuardarInventario(inventario);
                        break;
                    case "4":
                        INGRESOProducto(inventario);
                        GuardarInventario(inventario);
                        break;
                    case "5":
                        MostrarInventario(inventario);
                        break;
                    case "6":
                        MenuOrdenamiento(inventario);
                        GuardarInventario(inventario);
                        break;
                    case "7":
                        ModificarProducto(inventario);
                        GuardarInventario(inventario);
                        break;
                    case "8":
                        EliminarProducto(inventario);
                        GuardarInventario(inventario);
                        break;
                    case "9":
                        ReporteResumen(inventario);
                        break;
                    case "10":
                        MostrarInventarioFinal();
                        break;
                    case "0":
                        continuar = false;
                        GuardarInventario(inventario);
                        Console.WriteLine("Guardado final realizado. Saliendo...");
                        break;
                    default:
                        Console.WriteLine("Opción inválida. Intenta de nuevo.");
                        break;
                }

                if (continuar)
                {
                    Console.WriteLine("\nPresiona cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }
        //CARGAR PRODUCTO
        static List<PRODUCTOS> CargarInventario()
        {
            var lista = new List<PRODUCTOS>();

            if (!File.Exists(archivoInventario))
                return lista;

            string[] lineas = File.ReadAllLines(archivoInventario);
            foreach (var linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                var partes = linea.Split(';');
                if (partes.Length != 3) continue;

                if (double.TryParse(partes[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double precioParsed)
                    && int.TryParse(partes[2], out int cantidadParsed))
                {
                    lista.Add(new PRODUCTOS
                    {
                        Nombre = partes[0],
                        precio = precioParsed,
                        cantidad = cantidadParsed
                    });
                }
            }

            return lista;
        }

        static void GuardarInventario(List<PRODUCTOS> inventario)
        {
            var lineas = inventario.Select(p => p.ToCsvLine()).ToArray();
            File.WriteAllLines(archivoInventario, lineas);
            Console.WriteLine("Inventario guardado en archivo.");
        }

        static void AgregarProducto(List<PRODUCTOS> inventario)
        {
            Console.WriteLine("\n--- AGREGAR PRODUCTO ---");
            Console.Write("¿Cuántos productos deseas registrar? ");
            if (!int.TryParse(Console.ReadLine(), out int m) || m <= 0)
            {
                Console.WriteLine("Número inválido. Regresando al menú.");
                return;
            }

            for (int i = 0; i < m; i++)
            {
                Console.WriteLine($"\nProducto #{i + 1}:");
                Console.Write("Nombre: ");
                string nombre = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(nombre))
                {
                    Console.WriteLine("Nombre inválido. Se omite este producto.");
                    i--;
                    continue;
                }

                Console.Write("Precio: ");
                if (!double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out double precio))
                {
                    Console.WriteLine("Precio inválido. Intenta de nuevo.");
                    i--;
                    continue;
                }

                Console.Write("Cantidad inicial: ");
                if (!int.TryParse(Console.ReadLine(), out int cantidad))
                {
                    Console.WriteLine("Cantidad inválida. Intenta de nuevo.");
                    i--;
                    continue;
                }

                var existente = inventario.FirstOrDefault(p => p.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
                if (existente != null)
                {
                    Console.WriteLine("El producto ya existe en inventario:");
                    Console.WriteLine(existente.ToString());
                    Console.Write("¿Deseas (1) sumar la cantidad y actualizar precio o (2) solo sumar cantidad? [1/2]: ");
                    string resp = Console.ReadLine();
                    if (resp == "1")
                    {
                        existente.cantidad += cantidad;
                        existente.precio = precio;
                    }
                    else
                    {
                        existente.cantidad += cantidad;
                    }
                    Console.WriteLine("Producto actualizado.");
                }
                else
                {
                    inventario.Add(new PRODUCTOS { Nombre = nombre, precio = precio, cantidad = cantidad });
                    Console.WriteLine("Producto agregado.");
                }
            }
        }
        // BUSCAR PRODUCTO
        static void BuscarProducto(List<PRODUCTOS> inventario)
        {
            Console.WriteLine("\n--- BUSCAR PRODUCTO ---");
            Console.Write("Ingresa el nombre (o parte del nombre) del producto: ");
            string buscado = Console.ReadLine().Trim().ToLower();
            if (string.IsNullOrEmpty(buscado))
            {
                Console.WriteLine("Búsqueda vacía.");
                return;
            }

            var encontrados = inventario.Where(p => p.Nombre != null && p.Nombre.ToLower().Contains(buscado)).ToList();

            if (encontrados.Count == 0)
            {
                Console.WriteLine("No se encontraron productos con ese criterio.");
            }
            else
            {
                Console.WriteLine($"\nSe encontraron {encontrados.Count} producto(s):");
                foreach (var p in encontrados)
                {
                    Console.WriteLine(p.ToString());
                }
            }
        }

        //FUNCIÓN PARA SACAR PRODUCTO
        static void SalidaProducto(List<PRODUCTOS> inventario)
        {
            Console.WriteLine("\n--- SALIDA DE PRODUCTO (DESCONTAR) ---");
            Console.Write("Nombre exacto del producto: ");
            string nombre = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(nombre)) { Console.WriteLine("Nombre inválido."); return; }

            var producto = inventario.FirstOrDefault(p => p.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
            if (producto == null) { Console.WriteLine("Producto no encontrado."); return; }

            Console.WriteLine($"Stock actual: {producto.cantidad}");
            Console.Write("Cantidad a descontar: ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0) { Console.WriteLine("Cantidad inválida."); return; }

            if (producto.cantidad < qty) { Console.WriteLine("No hay suficiente stock. Operación cancelada."); return; }

            producto.cantidad -= qty;
            Console.WriteLine("Salida realizada. Stock actualizado.");
        }
        // FUNCIÓN PARA INGRESAR PRODUCTO
        static void INGRESOProducto(List<PRODUCTOS> inventario)
        {
            Console.WriteLine("\n--- INGRESO DE PRODUCTO (SUMAR) ---");
            Console.Write("Nombre exacto del producto: ");
            string nombre = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(nombre)) { Console.WriteLine("Nombre inválido."); return; }

            var producto = inventario.FirstOrDefault(p => p.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
            if (producto == null) { Console.WriteLine("Producto no encontrado. Usa opción 1 para agregar nuevos."); return; }

            Console.WriteLine($"Stock actual: {producto.cantidad}");
            Console.Write("Cantidad a agregar: ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0) { Console.WriteLine("Cantidad inválida."); return; }

            producto.cantidad += qty;
            Console.WriteLine("Ingreso realizado. Stock actualizado.");
        }
        //MÉTODO PARA MOSTRAR INVENTARIO
        static void MostrarInventario(List<PRODUCTOS> inventario)
        {
            Console.WriteLine("\n--- INVENTARIO ---");
            if (inventario.Count == 0) { Console.WriteLine("El inventario está vacío."); return; }

            foreach (var p in inventario)
                Console.WriteLine(p.ToString());
        }

        //FUNCIÓN PARA ORDENAR MENU
        static void MenuOrdenamiento(List<PRODUCTOS> inventario)
        {
            Console.WriteLine("\n--- ORDENAMIENTO DE REGISTROS ---");
            if (inventario.Count == 0) { Console.WriteLine("Inventario vacío."); return; }

            Console.WriteLine("Elige algoritmo:");
            Console.WriteLine("1. Selection Sort (por precio)  [iterativo]");
            Console.WriteLine("2. QuickSort (por nombre)      [recursivo]");
            Console.Write("Opción: ");
            string op = Console.ReadLine();
            if (op == "1")
            {
                SelectionSortPorPrecio(inventario);
                Console.WriteLine("Ordenamiento por precio (SelectionSort) realizado.");
            }
            else if (op == "2")
            {
                QuickSortPorNombre(inventario, 0, inventario.Count - 1);
                Console.WriteLine("Ordenamiento por nombre (QuickSort) realizado.");
            }
            else Console.WriteLine("Opción inválida.");
        }

        static void SelectionSortPorPrecio(List<PRODUCTOS> arr)
        {
            int n = arr.Count;
            for (int i = 0; i < n - 1; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < n; j++)
                    if (arr[j].precio < arr[minIndex].precio) minIndex = j;

                if (minIndex != i)
                {
                    var temp = arr[i];
                    arr[i] = arr[minIndex];
                    arr[minIndex] = temp;
                }
            }
        }

        static void QuickSortPorNombre(List<PRODUCTOS> arr, int left, int right)
        {
            if (left >= right) return;
            int mid = left + (right - left) / 2;
            string pivot = arr[mid].Nombre.ToLower();
            int i = left, j = right;

            while (i <= j)
            {
                while (string.Compare(arr[i].Nombre, pivot, StringComparison.OrdinalIgnoreCase) < 0) i++;
                while (string.Compare(arr[j].Nombre, pivot, StringComparison.OrdinalIgnoreCase) > 0) j--;

                if (i <= j)
                {
                    var temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;
                    i++; j--;
                }
            }

            if (left < j) QuickSortPorNombre(arr, left, j);
            if (i < right) QuickSortPorNombre(arr, i, right);
        }

        //FUNCIÓN PARA ORDENAR PRODUCTO
        static void ModificarProducto(List<PRODUCTOS> inventario)
        {
            Console.WriteLine("\n--- MODIFICAR PRODUCTO ---");
            Console.Write("Nombre exacto del producto a modificar: ");
            string nombre = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(nombre)) { Console.WriteLine("Nombre inválido."); return; }

            var producto = inventario.FirstOrDefault(p => p.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
            if (producto == null) { Console.WriteLine("Producto no encontrado."); return; }

            Console.WriteLine("Registro actual:");
            Console.WriteLine(producto.ToString());

            Console.Write("Nuevo nombre (ENTER para dejar igual): ");
            string nuevoNombre = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevoNombre)) producto.Nombre = nuevoNombre.Trim();

            Console.Write("Nuevo precio (ENTER para dejar igual): ");
            string precioStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(precioStr) && double.TryParse(precioStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double nuevoPrecio))
                producto.precio = nuevoPrecio;

            Console.Write("Nueva cantidad (ENTER para dejar igual): ");
            string cantidadStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(cantidadStr) && int.TryParse(cantidadStr, out int nuevaCantidad))
                producto.cantidad = nuevaCantidad;

            Console.WriteLine("Producto modificado.");
        }
        //FUNCIÓN PARA ELIMINAR PRODUCTO
        static void EliminarProducto(List<PRODUCTOS> inventario)
        {
            Console.WriteLine("\n--- ELIMINAR PRODUCTO ---");
            Console.Write("Nombre exacto del producto a eliminar: ");
            string nombre = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(nombre)) { Console.WriteLine("Nombre inválido."); return; }

            var producto = inventario.FirstOrDefault(p => p.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
            if (producto == null) { Console.WriteLine("Producto no encontrado."); return; }

            Console.Write("¿Estás seguro que deseas eliminar este producto? (s/n): ");
            string resp = Console.ReadLine().Trim().ToLower();
            if (resp == "s" || resp == "si") { inventario.Remove(producto); Console.WriteLine("Producto eliminado."); }
            else Console.WriteLine("Operación cancelada.");
        }

        //MÉTODO DE REPORTE
        static void ReporteResumen(List<PRODUCTOS> inventario)
        {
            Console.WriteLine("\n--- REPORTE RESUMEN ---");
            if (inventario.Count == 0) { Console.WriteLine("Inventario vacío."); return; }

            var mayorPrecio = inventario.OrderByDescending(p => p.precio).First();
            var menorPrecio = inventario.OrderBy(p => p.precio).First();
            double promedioPrecio = inventario.Average(p => p.precio);
            int totalArticulos = inventario.Sum(p => p.cantidad);

            Console.WriteLine($"Producto con MAYOR precio: {mayorPrecio.Nombre} -> {mayorPrecio.precio.ToString("F2", CultureInfo.InvariantCulture)}");
            Console.WriteLine($"Producto con MENOR precio: {menorPrecio.Nombre} -> {menorPrecio.precio.ToString("F2", CultureInfo.InvariantCulture)}");
            Console.WriteLine($"Precio PROMEDIO: {promedioPrecio.ToString("F2", CultureInfo.InvariantCulture)}");
            Console.WriteLine($"Total de unidades en inventario (sumatoria de cantidades): {totalArticulos}");
        }

        //MÉTODO PRA MOSTRAR INVENTARIO
        static void MostrarInventarioFinal()
        {
            Console.WriteLine("\n--- INVENTARIO FINAL (DESDE ARCHIVO) ---");
            if (!File.Exists(archivoInventario)) { Console.WriteLine("No hay archivo de inventario. Está vacío."); return; }

            string[] lineas = File.ReadAllLines(archivoInventario);
            if (lineas.Length == 0) { Console.WriteLine("El archivo de inventario está vacío."); return; }

            foreach (var linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                var partes = linea.Split(';');
                if (partes.Length != 3) continue;

                Console.WriteLine($"Nombre: {partes[0]} | Precio: {partes[1]} | Cantidad: {partes[2]}");
            }
        }
    }
}