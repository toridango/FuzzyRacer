using System;

public class DNA<T>
{
    public T[] m_genes { get; private set; }
    public float m_fitness { get; private set; }

    private Random random;
    private Func<T> getRandomGene;
    private Func<int, float> fitnessFunction;

    public DNA(int size, Random random, Func<T> getRandomGene, Func<int, float> fitnessFunction, bool shouldInitGenes = true)
    {
        m_genes = new T[size];
        this.random = random;
        this.getRandomGene = getRandomGene;
        this.fitnessFunction = fitnessFunction;

        if (shouldInitGenes)
        {
            for (int i = 0; i < m_genes.Length; i++)
            {
                m_genes[i] = getRandomGene();
            }
        }
    }

    public float CalculateFitness(int index)
    {
        m_fitness = fitnessFunction(index);
        return m_fitness;
    }

    public DNA<T> Crossover(DNA<T> otherParent)
    {
        DNA<T> child = new DNA<T>(m_genes.Length, random, getRandomGene, fitnessFunction, shouldInitGenes: false);

        for (int i = 0; i < m_genes.Length; i++)
        {
            child.m_genes[i] = random.NextDouble() < 0.5 ? m_genes[i] : otherParent.m_genes[i];
        }

        return child;
    }

    public void Mutate(float mutationRate)
    {
        for (int i = 0; i < m_genes.Length; i++)
        {
            if (random.NextDouble() < mutationRate)
            {
                m_genes[i] = getRandomGene();
            }
        }
    }
}
