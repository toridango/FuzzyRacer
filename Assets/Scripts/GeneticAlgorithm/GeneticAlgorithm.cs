using System;
using System.Collections.Generic;

public class GeneticAlgorithm<T>
{
    public List<DNA<T>> m_population { get; private set; }
    public int m_generation { get; private set; }
    public float m_bestFitness { get; private set; }
    public T[] m_bestGenes { get; private set; }

    public int m_elitism;
    public float m_mutationRate;

    private Random random;
    private List<DNA<T>> m_newPopulation;
    private float m_fitnessSum;
    private int m_dnaSize;
    private Func<T> getRandomGene;
    private Func<int, float> fitnessFunction;

    public GeneticAlgorithm(int populationSize, int dnaSize, Random random, Func<T> getRandomGene, Func<int, float> fitnessFunction, int elitism, float mutationRate = 0.01f)
    {
        m_generation = 1;
        m_elitism = elitism;
        m_mutationRate = mutationRate;
        m_population = new List<DNA<T>>(populationSize);
        m_newPopulation = new List<DNA<T>>(populationSize);
        this.random = random;
        this.m_dnaSize = dnaSize;
        this.getRandomGene = getRandomGene;
        this.fitnessFunction = fitnessFunction;

        m_bestGenes = new T[dnaSize];

        for (int i = 0; i < populationSize; i++)
        {
            m_population.Add(new DNA<T>(dnaSize, random, getRandomGene, fitnessFunction, shouldInitGenes: true));
        }
    }

    public void NewGeneration(int numNewDNA = 0, bool crossoverNewDNA = false)
    {
        int finalCount = m_population.Count + numNewDNA;

        if (finalCount <= 0)
        {
            return;
        }

        if (m_population.Count > 0)
        {
            CalculateFitness();
            m_population.Sort(CompareDNA);
        }
        m_newPopulation.Clear();

        for (int i = 0; i < m_population.Count; i++)
        {
            if (i < m_elitism && i < m_population.Count)
            {
                m_newPopulation.Add(m_population[i]);
            }
            else if (i < m_population.Count || crossoverNewDNA)
            {
                DNA<T> parent1 = ChooseParent();
                DNA<T> parent2 = ChooseParent();

                // NotSetToReferenceOfObject when ChooseParent returns null
                DNA<T> child = parent1.Crossover(parent2);

                child.Mutate(m_mutationRate);

                m_newPopulation.Add(child);
            }
            else
            {
                m_newPopulation.Add(new DNA<T>(m_dnaSize, random, getRandomGene, fitnessFunction, shouldInitGenes: true));
            }
        }

        List<DNA<T>> tmpList = m_population;
        m_population = m_newPopulation;
        m_newPopulation = tmpList;

        m_generation++;
    }

    private int CompareDNA(DNA<T> a, DNA<T> b)
    {
        if (a.m_fitness > b.m_fitness)
        {
            return -1;
        }
        else if (a.m_fitness < b.m_fitness)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private void CalculateFitness()
    {
        m_fitnessSum = 0;
        DNA<T> best = m_population[0];

        for (int i = 0; i < m_population.Count; i++)
        {
            m_fitnessSum += m_population[i].CalculateFitness(i);

            if (m_population[i].m_fitness > best.m_fitness)
            {
                best = m_population[i];
            }
        }

        m_bestFitness = best.m_fitness;
        best.m_genes.CopyTo(m_bestGenes, 0);
    }

    private DNA<T> ChooseParent()
    {
        double randomNumber = random.NextDouble() * m_fitnessSum;

        for (int i = 0; i < m_population.Count; i++)
        {
            if (randomNumber < m_population[i].m_fitness)
            {
                return m_population[i];
            }

            randomNumber -= m_population[i].m_fitness;
        }

        return null;
    }
}