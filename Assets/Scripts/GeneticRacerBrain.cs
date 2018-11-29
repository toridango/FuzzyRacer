using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;





public class GeneticRacerBrain : MonoBehaviour
{

    public int populationSize = 20;
    public float mutationRate = 0.01f;
    public int elitism = 5;
    public GameObject m_raceline;
    public GameObject racer;
    public Text m_textGen;
    public Text m_textFit;


    private GeneticAlgorithm<float> ga;
    private System.Random random;
    private Vector3 m_linePos;
    private bool converged = false;
    private float t;

    private List<GameObject> m_instances = new List<GameObject>();


    // Use this for initialization
    void Start()
    {
        /**
         * Genetic Agorithm to learn the best values for the potential-based movement
         * 
         * DNA shape: float[4] = {A, B, n, m}
         * for force function: 
         * F = -dU/dr = -nA/r^n+1  +  mB/r^m+1
         * 
         **/
        random = new System.Random();
        ga = new GeneticAlgorithm<float>(populationSize, 4, random, GetRandomGene, FitnessFunction, elitism, mutationRate);
        if (!m_raceline) m_raceline = GameObject.Find("RaceLine");
        t = Time.time;

        for (int i = 0; i < populationSize; ++i)
        {
            GameObject instance = Instantiate(racer);
            m_instances.Add(instance);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        m_textGen.text = String.Format("Generation: {0, 4}", ga.m_generation);
        m_textFit.text = String.Format("Best Fitness: {0, 6}", ga.m_bestFitness);
        m_linePos = m_raceline.transform.position;
        if (ga.m_bestFitness != 1)
        {
            if (converged)
            {
                ga.NewGeneration();
                resetRacers();
                converged = false;
            }

            for (int i = 0; i < populationSize; ++i)
            {
                moveAccordingToGenes(i);
            }
            
            if(allDied())
            {
                converged = true;
            }
            else
            {
                if (ga.m_bestFitness == 1)
                    converged = true;

                if (Time.time - t > 1.0f)
                {
                    converged = true;
                    t = Time.time;
                }
            }

            

        }
        else
        {
            Debug.Log("Max fitness attained");
        }
        /*if (ga.BestFitness == 1)
        {
            this.enabled = false;
        }*/
    }

    private static double NextDouble(System.Random random)
    {
        double mantissa = (random.NextDouble() + 2.0) - 1.0;

        // choose -149 instead of -126 to also generate subnormal floats (*)
        double exponent = Math.Pow(2.0, random.Next(-126, 128));
        return (mantissa * exponent);
    }

    private static float NextFloat(System.Random random)
    {
        return (float)NextDouble(random);
    }

    private static float NextFloat(System.Random random, double minimum, double maximum)
    {
        double mantissa = (random.NextDouble() + 2.0) - 1.0;

        // choose -149 instead of -126 to also generate subnormal floats (*)
        double exponent = Math.Pow(2.0, random.Next(-126, 128));
        return (float)(NextDouble(random) * (maximum - minimum) + minimum);
    }


    private float GetRandomGene()
    {
        return NextFloat(random, -4.0f, 4.0f);
    }

    private float FitnessFunction(int index)
    {
        float score = 0;
        DNA<float> dna = ga.m_population[index];
        if (m_instances[index] != null)
        {
            Vector3 pos = m_instances[index].transform.position;
            Vector3 r = pos - m_linePos;
            score = Mathf.Exp(-r.magnitude);
        }

        return score;
    }

    private void resetRacers()
    {
        for (int i = 0; i < populationSize; ++i)
        {
            if (m_instances[i] != null)
            {
                m_instances[i].transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            }
            else
            {
                GameObject instance = Instantiate(racer);
                m_instances[i] = instance;
            }
        }
    }

    private bool allDied()
    {
        bool alldied = true;
        int i = 0;
        while(alldied && i < populationSize)
        {
            if (m_instances[i] != null)
                alldied = false;

            ++i;
        }
        return alldied;
    }

    private void moveAccordingToGenes(int i)
    {

        if(m_instances[i] != null)
        {
            float[] genes = ga.m_population[i].m_genes;

            Vector3 pos = m_instances[i].transform.position;
            Vector3 r = pos - m_linePos;
            Vector2 u = r.normalized;
            float dist = r.magnitude;
            float A = 100*genes[0];
            float B = 100*genes[1];
            float n = 2;
            float m = 2;

            if (dist > 1E-8)
            {
                Vector3 force = u * ((n * -A) / Mathf.Pow(dist, n + 1) + (m * B) / Mathf.Pow(dist, m + 1));

                // Mass is considered 1 if force is not divided by anything
                float xspeed = Time.deltaTime * force.x;
                pos.x += Time.deltaTime * xspeed + 0.5f * force.x * Mathf.Pow(Time.deltaTime, 2);

                if (float.IsNaN(pos.x) || pos.x > 200.0f || pos.x < -200.0f)
                {
                    Destroy(m_instances[i]);
                }
                else
                {
                    m_instances[i].transform.position = pos;
                }

            }

        }
    }



}
