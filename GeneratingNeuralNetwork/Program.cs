using System;
using System.Collections.Generic;

namespace GeneratingNeuralNetwork
{
	class Program
	{

		static void Main(string[] args)
		{
			int seed = new Random().Next();
			Random r = new Random(seed);
			Console.WriteLine("Seed: " + seed);

			Console.WriteLine("Creating world...");
			Creature[,] world = CreateWorld(r);
			Console.WriteLine("Hello World!");

			for (int gen = 0; gen < 1000; gen++)
			{

				for (int step = 0; step < 300; step++)
				{
					foreach (Creature c in world)
						if (c != null)
							c.SetCorectInputs(world);

					foreach (Creature c in world)
						if (c != null)
							c.ThinkFast();

					foreach (Creature c in world)
						if (c != null)
							c.DoActions(world);

				}
				if (gen % 10 == 0)
					PrintWorld(world);

				List<int[]> survivors = new List<int[]>();

				for (int i = 64; i < world.GetUpperBound(0); i++)
					for (int j = 0; j < world.GetUpperBound(1); j++)
						if (world[i, j] != null && world[i, j].isAlive)
							survivors.Add(world[i, j].genomes);

				float survivingRate = (float)survivors.Count / 1000;
				Console.WriteLine("(" + gen + ") Surviving rate = " + survivingRate * 100 + "% (" + survivors.Count + ")");

				world = CreateWorld(r, survivors);
			}

			Console.WriteLine("Finished!");
			Console.ReadLine();
		}

		static void PrintWorld(Creature[,] world)
		{
			for (int i = 0; i < world.GetUpperBound(0); i++)
			{
				for (int j = 0; j < world.GetUpperBound(1); j++)
					if (world[i, j] != null && world[i, j].isAlive)
					{
						Console.ForegroundColor = ConsoleColor.Green;
						Console.Write('#');
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.DarkGray;
						Console.Write('0');
					}
				Console.ResetColor();
				Console.WriteLine();
			}

		}

		static Creature[,] CreateWorld(Random r, List<int[]> genePool = null, int genomeCount = 256, int width = 128, int height = 128, int creatureCount = 1000)
		{
			Creature[,] world = new Creature[width, height];

			while (creatureCount > 0)
			{
				int x = r.Next(0, width);
				int y = r.Next(0, height);
				if (world[x, y] == null)
				{
					creatureCount -= 1;
					int[] genes = new int[genomeCount * 3];
					if (genePool != null && genePool.Count > 0)
					{
						genePool[r.Next(0, genePool.Count)].CopyTo(genes, 0);
						if (r.NextDouble() > 0.999)
							genes[r.Next(0, genes.Length)] = r.Next();
					}
					else
					{
						for (int i = 0; i < genes.Length; i++)
							genes[i] = r.Next();
					}
					world[x, y] = new Creature(x, y, genes);
				}
			}

			return world;
		}
	}
}
