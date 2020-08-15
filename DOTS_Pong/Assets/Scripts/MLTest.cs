using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TESTING MACHINE LEARNING");
        // Declare some training data
        int[][] inputs = new int[][]
        {
            new int[] { 0,1,1,0 },   // Class 0
            new int[] { 0,0,1,0 },   // Class 0
            new int[] { 0,1,1,1,0 }, // Class 0
            new int[] { 0,1,0 },     // Class 0

            new int[] { 1,0,0,1 },   // Class 1
            new int[] { 1,1,0,1 },   // Class 1
            new int[] { 1,0,0,0,1 }, // Class 1
            new int[] { 1,0,1 },     // Class 1

            new int[] { 0,0,0,0,1,0 },
        };

        int[] outputs = new int[]
        {
            0,0,0,0, // First four sequences are of class 0
            1,1,1,1, // Last four sequences are of class 1
            2,
        };


        // We are trying to predict two different classes
        int classes = 3;

        // Each sequence may have up to two symbols (0 or 1)
        int symbols = 2;

        // Nested models will have two states each
        int[] states = new int[] { 3,3,3 };

        // Creates a new Hidden Markov Model Classifier with the given parameters
        HiddenMarkovClassifier classifier = new HiddenMarkovClassifier(classes, states, symbols);

        // Create a new learning algorithm to train the sequence classifier
        var teacher = new HiddenMarkovClassifierLearning(classifier,

            // Train each model until the log-likelihood changes less than 0.001
            modelIndex => new BaumWelchLearning(classifier.Models[modelIndex])
            {
                Tolerance = 0.001,
                MaxIterations = 1000
            }
        );

        // Train the sequence classifier using the algorithm
        teacher.Learn(inputs, outputs);

        // Compute the classifier answers for the given inputs
        int[] answers = classifier.Decide(inputs);
        foreach (var item in answers)
        {
            Debug.Log(item);
        }
  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
