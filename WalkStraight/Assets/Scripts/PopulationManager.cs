using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] private GameObject _botPrefab;
    [SerializeField] private int _populationSize = 50;
    [SerializeField] private float _trialTime = 5f;

    private List<GameObject> _population = new List<GameObject>();

    private int _generation = 1;

    public static float Elapsed { get; set; } = 0;

    private void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle();

        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;

        GUI.BeginGroup(new Rect(10, 10, 250, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + _generation, guiStyle);
        GUI.Label(new Rect(10, 50, 200, 30), string.Format("Time: {0:0.00}", Elapsed), guiStyle);
        GUI.Label(new Rect(10, 75, 200, 30), "Population: " + _population.Count, guiStyle);
        GUI.EndGroup();
    }

    // Start is called before the first frame update
    private void Start()
    {
        for (int i = 0; i < _populationSize; i++)
        {
            Vector3 startingPosition = new Vector3(
                transform.position.x + Random.Range(-2, 2),
                transform.position.y,
                transform.position.z + Random.Range(-2, 2));

            GameObject firstBot = Instantiate(_botPrefab, startingPosition, transform.rotation);

            firstBot.GetComponent<Brain>().Init();
            _population.Add(firstBot);
        }
    }

    private GameObject Breed(GameObject parent1, GameObject parent2)
    {
        Vector3 startingPosition = new Vector3(
           transform.position.x + Random.Range(-2, 2),
           transform.position.y,
           transform.position.z + Random.Range(-2, 2));

        GameObject offSpring = Instantiate(_botPrefab, startingPosition, transform.rotation);

        Brain brain = offSpring.GetComponent<Brain>();

        if (Random.Range(0, 100) == 1)
        {
            brain.Init();
            brain.DNA.Mutate();
        }
        else
        {
            brain.Init();
            brain.DNA.Combine(parent1.GetComponent<Brain>().DNA, parent2.GetComponent<Brain>().DNA);
        }

        return offSpring;
    }

    private void BreedNewPopulation()
    {
        List<GameObject> sortedPopulation = _population.OrderBy(p => p.GetComponent<Brain>().TimeAlive).ToList();

        _population.Clear();

        // Breed upper half of sorted population
        for (int i = (int)(sortedPopulation.Count / 2.0f) - 1; i < sortedPopulation.Count - 1; i++)
        {
            _population.Add(Breed(sortedPopulation[i], sortedPopulation[i + 1]));
            _population.Add(Breed(sortedPopulation[i + 1], sortedPopulation[i]));
        }

        // Destroy all parents and previous population
        for (int i = 0; i < sortedPopulation.Count; i++)
        {
            Destroy(sortedPopulation[i]);
        }

        _generation++;
    }

    // Update is called once per frame
    private void Update()
    {
        Elapsed += Time.deltaTime;

        if (Elapsed >= _trialTime)
        {
            BreedNewPopulation();
            Elapsed = 0;
        }
    }
}
