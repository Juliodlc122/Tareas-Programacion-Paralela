using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
//Julio Antonio Ruiz de la Cruz | Matrícula: 2024-2009 | Desarrollo de Software
namespace SimulacionFlynn_0506
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Simulación de la Taxonomía de Flynn ===");
            Console.WriteLine("Desarrollador: Julio Antonio Ruiz de la Cruz | Matrícula: 2024-2009");
            Console.WriteLine("Carrera: Desarrollo de Software");
            Console.WriteLine("==================================================\n");

            int dataSize = 100_000_000;
            int[] dataOriginal = new int[dataSize];

            // Inicializar arreglo con datos (el número 2 en todas las posiciones)
            for (int i = 0; i < dataSize; i++)
            {
                dataOriginal[i] = 2;
            }

            //1. SIMULACIÓN SISD
            int[] dataSISD = new int[dataSize];
            Array.Copy(dataOriginal, dataSISD, dataSize);
            Console.WriteLine("-> Ejecutando modelo SISD (Procesamiento Secuencial)...");

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < dataSISD.Length; i++)
            {
                // Operación: multiplicar por 5 (un solo flujo de instrucciones, un dato a la vez)
                dataSISD[i] = dataSISD[i] * 5;
            }
            sw.Stop();
            Console.WriteLine($"SISD Finalizado. Tiempo: {sw.ElapsedMilliseconds} ms. Muestra [0]: {dataSISD[0]}\n");


            //2. SIMULACIÓN SIMD
            int[] dataSIMD = new int[dataSize];
            Array.Copy(dataOriginal, dataSIMD, dataSize);
            Console.WriteLine("-> Ejecutando modelo SIMD (Procesamiento Vectorial paralelo)...");

            sw.Restart();
            int vectorSize = Vector<int>.Count;
            int iSimd = 0;
            Vector<int> multiplicador = new Vector<int>(5);

            // Procesa múltiples datos simultáneamente usando registros vectoriales del hardware
            for (; iSimd <= dataSIMD.Length - vectorSize; iSimd += vectorSize)
            {
                Vector<int> vectorData = new Vector<int>(dataSIMD, iSimd);
                Vector<int> resultadoVector = Vector.Multiply(vectorData, multiplicador);
                resultadoVector.CopyTo(dataSIMD, iSimd);
            }
            // Procesar los elementos restantes que no completan un vector
            for (; iSimd < dataSIMD.Length; iSimd++)
            {
                dataSIMD[iSimd] = dataSIMD[iSimd] * 5;
            }
            sw.Stop();
            Console.WriteLine($"SIMD Finalizado. Tiempo: {sw.ElapsedMilliseconds} ms. Muestra [0]: {dataSIMD[0]}\n");


            //3. SIMULACIÓN MIMD
            int[] dataMIMD = new int[dataSize];
            Array.Copy(dataOriginal, dataMIMD, dataSize);
            Console.WriteLine("-> Ejecutando modelo MIMD (Múltiples Tareas, Múltiples Operaciones)...");

            sw.Restart();
            int midPoint = dataSize / 2;

            // Tarea 1: procesa la primera mitad elevando al cuadrado (instrucción A, Datos A)
            Task tarea1 = Task.Run(() =>
            {
                for (int i = 0; i < midPoint; i++)
                {
                    dataMIMD[i] = dataMIMD[i] * dataMIMD[i];
                }
            });

            // Tarea 2: procesa la segunda mitad multiplicando por 10 (Instrucción B, Datos B)
            Task tarea2 = Task.Run(() =>
            {
                for (int i = midPoint; i < dataSize; i++)
                {
                    dataMIMD[i] = dataMIMD[i] * 10;
                }
            });

            Task.WaitAll(tarea1, tarea2);
            sw.Stop();
            Console.WriteLine($"MIMD Finalizado. Tiempo: {sw.ElapsedMilliseconds} ms.");
            Console.WriteLine($"Muestra Tarea 1 [0] (Elevado al cuadrado): {dataMIMD[0]}");
            Console.WriteLine($"Muestra Tarea 2 [{midPoint}] (Multiplicado por 10): {dataMIMD[midPoint]}\n");

            Console.WriteLine("Simulación completada con éxito. Presiona enter para salir.");
            Console.ReadLine();
        }
    }
}