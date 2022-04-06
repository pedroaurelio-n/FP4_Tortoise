using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDialogueVariableTest : MonoBehaviour
{
    [SerializeField] private string pokemonName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterController player))
        {
            if (pokemonName == "Bulbasaur" || pokemonName == "Charmander" || pokemonName == "Squirtle")
            {
                Ink.Runtime.Object obj = new Ink.Runtime.StringValue(pokemonName);
                DialogueManager.SetVariableState("pokemon_name", obj);
            }
            else
            {
                Debug.LogWarning("Unsupported name. Try Bulbasaur, Charmander or Squirtle.");
            }
        }
    }
}
