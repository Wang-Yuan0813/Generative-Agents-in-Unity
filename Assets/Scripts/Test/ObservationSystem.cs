using UnityEngine;
using System.Collections.Generic;

public class ObservationSystem : MonoBehaviour
{
    //==================================================
    // OBSERVATIONS
    //==================================================

    public List<string> GetObservations(WayPoint currentNode)
    {
        List<string> observations = new List<string>();

        //----------------------------------------
        // Current Location
        //----------------------------------------

        observations.Add("Current location: " + currentNode.name);

        //----------------------------------------
        // Current Node Events
        //----------------------------------------
        if(currentNode.events.Count != 0)
            foreach (Event evt in currentNode.events)
                observations.Add(evt.GetObservationText());
        else
            observations.Add("There is nothing interesting around " + currentNode.name);


        //----------------------------------------
        // Neighbor Nodes
        //----------------------------------------

        foreach (WayPoint neighbor in currentNode.neighbors)
        {
            observations.Add("Nearby location: " + neighbor.name);

            // Neighbor Events
            if (neighbor.events.Count != 0)
                foreach (Event evt in neighbor.events)
                    observations.Add(evt.GetObservationText());
            else
                observations.Add("There is nothing interesting around " + neighbor.name);
        }

        return observations;
    }


    //==================================================
    // BUILD PROMPT
    //==================================================

    public string BuildObservationPrompt(WayPoint wayPoint)
    {
        List<string> observations = GetObservations(wayPoint);

        if (observations.Count == 0)
        {
            return "Nothing interesting nearby.";
        }

        string result = "";

        foreach (string observation in observations)
        {
            result += "- " + observation + "\n";
        }

        return result;
    }
}
