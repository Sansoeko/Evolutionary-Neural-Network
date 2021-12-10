using System;

namespace GeneratingNeuralNetwork
{
	class Creature
	{
		Random r = new Random();
		public bool isAlive = true;
		float[,] neuralNetwork;
		public int[] genomes;
		int age;
		int neuronsCount;
		int memoryNeurons;

		int x;
		int y;

		float[] inputs; // sensory neurons
		float[] outputs; // actions neurons
		float[] neurons; // internal neurons

		public Creature(int x, int y, int[] genomes, int neuronsCount = 64, int memoryNeurons = 18)
		{
			this.genomes = genomes;
			this.x = x;
			this.y = y;

			this.neuronsCount = neuronsCount;
			this.memoryNeurons = memoryNeurons;

			inputs = new float[7 + memoryNeurons];
			outputs = new float[7 + memoryNeurons];
			neurons = new float[neuronsCount];

			//neuralNetwork = new float[inputs.Length + neurons.Length, outputs.Length + neurons.Length];

			age = 0;

			// Start oscillator
			inputs[6] = 1.0f;

			// Create neural network array
			//for (int i = 0; i < genomes.Length; i += 3)
			//	neuralNetwork[genomes[i] % neuralNetwork.GetUpperBound(0), genomes[i + 1] % neuralNetwork.GetUpperBound(1)] = (float)genomes[i + 2] / (int.MaxValue / 2) - 1.0f;

			// Simplify genomes
			for (int i = 0; i < genomes.Length; i+=3)
			{
				// For ThinkFast()
				genomes[i] = genomes[i] % (inputs.Length + neurons.Length);
				genomes[i + 1] = genomes[i + 1] % (outputs.Length + neurons.Length);
			}
		}

		// See
		public void SetCorectInputs(Creature[,] world)
		{
			if (!isAlive)
				return;
			
			// age
			inputs[0] = age;

			// Vission
			inputs[1] = Check(x + 1, y, world);
			inputs[2] = Check(x - 1, y, world);
			inputs[3] = Check(x, y + 1, world);
			inputs[4] = Check(x, y - 1, world);

			// Memory neurons
			for (int i = 0; i < memoryNeurons; i++)
				inputs[5 + i] = outputs[5 + i]; // memory neuron

			// Oscilator
			inputs[5 + memoryNeurons] = inputs[5 + memoryNeurons] * -1;
			// random
			inputs[6 + memoryNeurons] = (float)r.NextDouble();
		}

		// see if place is empty
		private static float Check(int toCheckX, int toCheckY, Creature[,] world)
		{
			if (toCheckX >= world.GetUpperBound(0) || toCheckX < 0 || toCheckY >= world.GetUpperBound(1) || toCheckY < 0)
				return 2.0f; // Not reachable
			if (world[toCheckX, toCheckY] != null)
				return 1.0f; // Not empty
			return 0f; // empty
		}

		public void ThinkFast()
		{
			if (!isAlive)
				return;

			neurons = new float[neurons.Length];
			outputs = new float[outputs.Length];

			for (int i = 0; i < genomes.Length; i += 3)
			{
				int from = genomes[i];
				int to = genomes[i + 1];
				float weight = (float)genomes[i + 2] / (int.MaxValue / 2) - 1.0f;
				if (from >= inputs.Length && to >= outputs.Length)
					neurons[to - outputs.Length] += weight * neurons[from - inputs.Length];
				else if (to >= outputs.Length)
					neurons[to - outputs.Length] += weight * inputs[from];
				else if (from >= inputs.Length)
					outputs[to] += weight * neurons[from - inputs.Length];
				else
					outputs[to] += weight * inputs[from];

			}

			return;
		}

		// Think
		public void Think()
		{
			if (!isAlive)
				return;

			for (int i = 0; i < neurons.Length; i++)
				neurons[i] = 0;
			for (int i = 0; i < outputs.Length; i++)
				outputs[i] = 0;

			// This is something that should be done in a compute shader I think.
			for (int i = 0; i < inputs.Length; i++)
				for (int j = 0; j < neurons.Length; j++)
					neurons[j] += neuralNetwork[i, j + outputs.Length] * inputs[i];

			for (int i = 0; i < neurons.Length; i++)
				neurons[i] = Sigma(neurons[i]);

			for (int i = 0; i < inputs.Length; i++)
				for (int j = 0; j < outputs.Length; j++)
					outputs[j] += neuralNetwork[i, j] * inputs[i];

			for (int i = 0; i < neurons.Length; i++)
				for (int j = 0; j < outputs.Length; j++)
					outputs[j] += neuralNetwork[i + inputs.Length, j] * neurons[i];

			for (int i = 0; i < outputs.Length; i++)
				outputs[i] = Sigma(outputs[i]);
		}

		private static float Sigma(float x)
		{
			return 1f / (1f + (float)Math.Pow(Math.E, x));
		}

		// Do
		public void DoActions(Creature[,] world)
		{
			if (!isAlive)
				return;

			age++;

			int highestIndex = 0;
			float highestValue = 0;

			// not memory neuron Find output neuron that is not a memory neuron
			for (int i = 0; i < 5; i++)
			{
				if (outputs[i] > highestValue) 
				{
					highestIndex = i;
					highestValue = outputs[i];
				}
			}

			world[x, y] = null;

			switch (highestIndex)
			{
				case 0: // Do nothing
				default:
					return;
				case 1: // move right
					if (Check(x + 1, y, world) == 0)
						x += 1;
					break;
				case 2: // move left
					if (Check(x - 1, y, world) == 0)
						x -= 1;
					break;
				case 3: // move up
					if (Check(x, y + 1, world) == 0)
						y += 1;
					break;
				case 4: // move down
					if (Check(x, y - 1, world) == 0)
						y -= 1;
					break;
				case 5: // move random
					switch (r.Next(0, 4))
					{
						case 0: // move right
							if (Check(x + 1, y, world) == 0)
								x += 1;
							break;
						case 1: // move left
							if (Check(x - 1, y, world) == 0)
								x -= 1;
							break;
						case 2: // move up
							if (Check(x, y + 1, world) == 0)
								y += 1;
							break;
						case 3: // move down
							if (Check(x, y - 1, world) == 0)
								y -= 1;
							break;
					}
					break;
				case 6:
					isAlive = false;
					break;
			}

			if (world[x, y] != null)
				world[x, y].isAlive = false;

			world[x, y] = this;
		}
	}
}
